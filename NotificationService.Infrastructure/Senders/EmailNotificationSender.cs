using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Application.Abstract_Services;
using NotificationService.Domain.Entities.Aggregates;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Infrastructure.Senders
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly ILogger<EmailNotificationSender> _logger;
        private readonly SmtpSettings _settings;

        public EmailNotificationSender(
            ILogger<EmailNotificationSender> logger,
            IOptions<SmtpSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public Domain.ValueObjects.NotificationType Channel => Domain.ValueObjects.NotificationType.Email;

        public async Task SendAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            if (notification.Recipient.Email is null)
            {
                throw new InvalidOperationException("Recipient email is required to send an email notification.");
            }

            if (string.IsNullOrWhiteSpace(_settings.Host))
            {
                throw new InvalidOperationException("SMTP host is not configured. Set 'Smtp:Host' in configuration.");
            }

            if (string.IsNullOrWhiteSpace(_settings.FromAddress))
            {
                throw new InvalidOperationException("SMTP from address is not configured. Set 'Smtp:FromAddress' in configuration.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName ?? string.Empty, _settings.FromAddress));
            message.To.Add(MailboxAddress.Parse(notification.Recipient.Email.Value));
            message.Subject = notification.MessageSubject.Value;
            message.Body = new TextPart("plain")
            {
                Text = notification.MessageBody.Value
            };

            using var client = new SmtpClient();

            var socketOptions = _settings.UseStartTls
                ? SecureSocketOptions.StartTls
                : (_settings.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);

            await client.ConnectAsync(_settings.Host, _settings.Port, socketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_settings.Username))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password ?? string.Empty, cancellationToken);
            }

            try
            {
                var response = await client.SendAsync(message, cancellationToken);
                _logger.LogInformation(
                    "Email sent to {Email} (subject: {Subject}). SMTP response: {Response}",
                    notification.Recipient.Email.Value,
                    notification.MessageSubject.Value,
                    response);
            }
            finally
            {
                await client.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}

