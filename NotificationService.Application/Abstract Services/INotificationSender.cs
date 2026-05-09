using NotificationService.Domain.Entities.Aggregates;

namespace NotificationService.Application.Abstract_Services
{
    public interface INotificationSender
    {
        Domain.ValueObjects.NotificationType Channel { get; }
        Task SendAsync(Notification notification, CancellationToken cancellationToken = default);
    }
}
