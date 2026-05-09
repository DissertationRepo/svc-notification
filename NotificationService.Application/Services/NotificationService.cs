using NotificationService.Application.Abstract_Services;
using NotificationService.Application.Models;
using NotificationService.Domain.Entities.Aggregates;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationSender _emailSender;

        public NotificationService(
            INotificationRepository notificationRepository,
            IEnumerable<INotificationSender> senders)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));

            if (senders is null)
            {
                throw new ArgumentNullException(nameof(senders));
            }

            _emailSender = senders.FirstOrDefault(s => s.Channel == Domain.ValueObjects.NotificationType.Email)
                ?? throw new InvalidOperationException("No email notification sender is registered.");
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

            if (string.IsNullOrWhiteSpace(message.Email))
            {
                return;
            }

            var subject = $"New message in conversation {message.ConversationId}";
            var body = $"From {message.SenderId} at {message.Timestamp:u}:{Environment.NewLine}{message.Content}";

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
                    MessageSubject = subject,
                    MessageBody = body
                };

                await CreateAndSendAsync(model, cancellationToken);
            }
        }

        private async Task DispatchAsync(Notification notification, CancellationToken cancellationToken)
        {
            try
            {
                await _emailSender.SendAsync(notification, cancellationToken);
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

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentException("Email is required.", nameof(model));
            }

            var recipient = new Recipient(model.RecipientId!, new Email(model.Email!));
            var subject = new MessageSubject(model.MessageSubject!);
            var body = new MessageBody(model.MessageBody!);

            return Notification.Create(recipient, subject, body, Domain.ValueObjects.NotificationType.Email);
        }
    }
}
