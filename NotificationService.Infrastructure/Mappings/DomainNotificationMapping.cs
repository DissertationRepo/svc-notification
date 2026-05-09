using AutoMapper;
using NotificationService.Domain.Entities.Aggregates;
using NotificationService.Infrastructure.Entities;

namespace NotificationService.Infrastructure.Mappings
{
    public class DomainNotificationMapping : Profile
    {
        public DomainNotificationMapping()
        {
            CreateMap<Notification, NotificationEntity>()
                .ConvertUsing(src => CreateInfrastructureNotification(src));
        }

        private NotificationEntity CreateInfrastructureNotification(Notification src)
        {
            return new NotificationEntity
            {
                Id = src.Id.Value,
                RecipientId = src.Recipient.RecipientId,
                Email = src.Recipient.Email?.Value,
                Phone = src.Recipient.PhoneNumber?.Value,
                MessageSubject = src.MessageSubject.Value,
                MessageBody = src.MessageBody.Value,
                NotificationType = (int)src.NotificationType,
                Status = (int)src.Status,
                FailureReason = src.FailureReason,
                CreatedAt = src.CreatedAt,
                SentAt = src.SentAt
            };
        }
    }
}
