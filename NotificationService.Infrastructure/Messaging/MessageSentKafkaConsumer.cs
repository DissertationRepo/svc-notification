using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.Abstract_Services;
using NotificationService.Application.Models;

namespace NotificationService.Infrastructure.Messaging
{
    public sealed class MessageSentKafkaConsumer : BackgroundService
    {
        private readonly KafkaSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MessageSentKafkaConsumer> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public MessageSentKafkaConsumer(
            IOptions<KafkaSettings> settings,
            IServiceScopeFactory scopeFactory,
            ILogger<MessageSentKafkaConsumer> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Run the consume loop on a dedicated thread so we do not block hosted service startup.
            return Task.Run(() => Consume(stoppingToken), stoppingToken);
        }

        private async Task Consume(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.GroupId,
                AutoOffsetReset = Enum.TryParse<AutoOffsetReset>(_settings.AutoOffsetReset, true, out var offset)
                    ? offset
                    : AutoOffsetReset.Earliest,
                EnableAutoCommit = _settings.EnableAutoCommit
            };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
                .Build();

            consumer.Subscribe(_settings.MessageSentTopic);
            _logger.LogInformation("Subscribed to Kafka topic '{Topic}'.", _settings.MessageSentTopic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<string, string>? result;
                    try
                    {
                        result = consumer.Consume(stoppingToken);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming Kafka message: {Reason}", ex.Error.Reason);
                        continue;
                    }

                    if (result?.Message?.Value is null)
                    {
                        continue;
                    }

                    try
                    {
                        var evt = JsonSerializer.Deserialize<MessageSentEvent>(result.Message.Value, JsonOptions);
                        if (evt is null)
                        {
                            _logger.LogWarning("Received empty MessageSentEvent payload.");
                            continue;
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        await notificationService.HandleMessageSentAsync(evt, stoppingToken);

                        if (!_settings.EnableAutoCommit)
                        {
                            consumer.Commit(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process MessageSentEvent at offset {Offset}.", result.Offset);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // graceful shutdown
            }
            finally
            {
                try
                {
                    consumer.Close();
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}
