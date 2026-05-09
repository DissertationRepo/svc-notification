using AutoMapper;
using NotificationService.Api.Models;
using NotificationService.Application.Models;

namespace NotificationService.Api.Mappings
{
    public class SendNotificationMapping : Profile
    {
        public SendNotificationMapping()
        {
            CreateMap<SendNotification, NotificationModel>()
                .ConstructUsing(src => new NotificationModel
                {
                    RecipientId = src.RecipientId,
                    Email = src.Email,
                    MessageSubject = src.MessageSubject,
                    MessageBody = src.MessageBody
                });
        }
    }
}
