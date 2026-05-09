using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.Abstract_Services;
using NotificationService.Domain.Entities.Aggregates;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationService.Infrastructure.Senders
{
    public class SmsNotificationSender : INotificationSender
    {
        private readonly ILogger<SmsNotificationSender> _logger;
        private readonly TwilioSettings _settings;
        private static int _initialized;

        public SmsNotificationSender(
            ILogger<SmsNotificationSender> logger,
            IOptions<TwilioSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public Domain.ValueObjects.NotificationType Channel => Domain.ValueObjects.NotificationType.Sms;

        public async Task SendAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            if (notification.Recipient.PhoneNumber is null)
            {
                throw new InvalidOperationException("Recipient phone number is required to send an SMS notification.");
            }

            if (string.IsNullOrWhiteSpace(_settings.AccountSid) ||
                string.IsNullOrWhiteSpace(_settings.AuthToken) ||
                string.IsNullOrWhiteSpace(_settings.FromNumber))
            {
                throw new InvalidOperationException(
                    "Twilio settings are not configured. Set 'Twilio:AccountSid', 'Twilio:AuthToken', and 'Twilio:FromNumber'.");
            }

            EnsureInitialized();

            var to = new PhoneNumber(notification.Recipient.PhoneNumber.Value);
            var body = $"{notification.MessageSubject.Value}{Environment.NewLine}{notification.MessageBody.Value}";

            var options = _settings.FromNumber.StartsWith("MG", StringComparison.OrdinalIgnoreCase)
                ? new CreateMessageOptions(to) { MessagingServiceSid = _settings.FromNumber, Body = body }
                : new CreateMessageOptions(to) { From = new PhoneNumber(_settings.FromNumber), Body = body };

            var message = await MessageResource.CreateAsync(options);

            _logger.LogInformation(
                "SMS dispatched to {Phone}. Twilio SID: {Sid}, Status: {Status}",
                notification.Recipient.PhoneNumber.Value,
                message.Sid,
                message.Status);
        }

        private void EnsureInitialized()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
            {
                TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
            }
        }
    }
}

