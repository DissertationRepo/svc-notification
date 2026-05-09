using NotificationService.Application.Abstract_Services;
using NotificationService.Application.Models;
using NotificationService.Domain.Entities.Aggregates;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IReadOnlyDictionary<Domain.ValueObjects.NotificationType, INotificationSender> _sendersByChannel;

        public NotificationService(
            INotificationRepository notificationRepository,
            IEnumerable<INotificationSender> senders)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _sendersByChannel = senders?.ToDictionary(s => s.Channel)
                ?? throw new ArgumentNullException(nameof(senders));
        }

        public async Task<Notification> CreateAndSendAsync(NotificationModel notification, CancellationToken cancellationToken = default)
        {
            var domainNotification = BuildNotification(notification);
            await _notificationRepository.AddNotification(domainNotification);
            await DispatchAsync(domainNotification, cancellationToken);
            return domainNotification;
        }

        public Task<Notification?> GetNotificationByIdAsync(Guid id)
        {
            return _notificationRepository.GetNotificationById(id);
        }

        public async Task HandleMessageSentAsync(MessageSentEvent message, CancellationToken cancellationToken = default)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var subject = $"New message in conversation {message.ConversationId}";
            var body = $"From {message.SenderId} at {message.Timestamp:u}:{Environment.NewLine}{message.Content}";

            var channel = !string.IsNullOrWhiteSpace(message.Email)
                ? Domain.ValueObjects.NotificationType.Email
                : Domain.ValueObjects.NotificationType.Sms;

            foreach (var recipientId in message.RecipientIds)
            {
                if (string.IsNullOrWhiteSpace(recipientId))
                {
                    continue;
                }

                var model = new NotificationModel
                {
                    RecipientId = recipientId,
                    Email = message.Email,
                    PhoneNumber = message.PhoneNumber,
                    MessageSubject = subject,
                    MessageBody = body,
                    NotificationType = channel.ToString()
                };

                await CreateAndSendAsync(model, cancellationToken);
            }
        }

        private async Task DispatchAsync(Notification notification, CancellationToken cancellationToken)
        {
            if (!_sendersByChannel.TryGetValue(notification.NotificationType, out var sender))
            {
                notification.MarkAsFailed($"No sender registered for channel '{notification.NotificationType}'.");
                await _notificationRepository.UpdateNotification(notification);
                return;
            }

            try
            {
                await sender.SendAsync(notification, cancellationToken);
                notification.MarkAsSent();
            }
            catch (Exception ex)
            {
                notification.MarkAsFailed(ex.Message);
            }

            await _notificationRepository.UpdateNotification(notification);
        }

        private static Notification BuildNotification(NotificationModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!Enum.TryParse<Domain.ValueObjects.NotificationType>(model.NotificationType, ignoreCase: true, out var type))
            {
                throw new ArgumentException($"Unknown notification type '{model.NotificationType}'.", nameof(model));
            }

            var email = string.IsNullOrWhiteSpace(model.Email) ? null : new Email(model.Email!);
            var phone = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : new PhoneNumber(model.PhoneNumber!);

            var recipient = new Recipient(model.RecipientId!, email, phone);
            var subject = new MessageSubject(model.MessageSubject!);
            var body = new MessageBody(model.MessageBody!);

            return Notification.Create(recipient, subject, body, type);
        }
    }
}
