using NotificationService.Application.Models;
using NotificationService.Domain.Entities.Aggregates;

namespace NotificationService.Application.Abstract_Services
{
    public interface INotificationService
    {
        Task<Notification> CreateAndSendAsync(NotificationModel notification, CancellationToken cancellationToken = default);
        Task<Notification?> GetNotificationByIdAsync(Guid id);
        Task HandleMessageSentAsync(MessageSentEvent message, CancellationToken cancellationToken = default);
    }
}
