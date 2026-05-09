using AutoMapper;
using NotificationService.Api.Models;
using NotificationService.Domain.Entities.Aggregates;

namespace NotificationService.Api.Mappings
{
    public class DomainNotificationMapping : Profile
    {
        public DomainNotificationMapping()
        {
            CreateMap<Notification, NotificationResponse>()
                .ConvertUsing(src => ConvertToResponse(src));
        }

        private NotificationResponse ConvertToResponse(Notification src)
        {
            return new NotificationResponse
            {
                Id = src.Id.Value.ToString(),
                RecipientId = src.Recipient.RecipientId,
                Email = src.Recipient.Email?.Value,
                PhoneNumber = src.Recipient.PhoneNumber?.Value,
                MessageSubject = src.MessageSubject.Value,
                MessageBody = src.MessageBody.Value,
                NotificationType = src.NotificationType.ToString(),
                Status = src.Status.ToString(),
                FailureReason = src.FailureReason,
                CreatedAt = src.CreatedAt,
                SentAt = src.SentAt
            };
        }
    }
}
