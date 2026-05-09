using NotificationService.Domain.Entities.Aggregates;

namespace NotificationService.Application.Abstract_Services
{
    public interface INotificationRepository
    {
        Task AddNotification(Notification notification);
        Task<Notification?> GetNotificationById(Guid id);
        Task UpdateNotification(Notification notification);
    }
}
