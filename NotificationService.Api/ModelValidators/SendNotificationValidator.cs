using FluentValidation;
using NotificationService.Api.Models;

namespace NotificationService.Api.ModelValidators
{
    public class SendNotificationValidator : AbstractValidator<SendNotification>
    {
        public SendNotificationValidator()
        {
            RuleFor(x => x.RecipientId).NotEmpty().WithMessage("RecipientId is required.");
            RuleFor(x => x.MessageSubject).NotEmpty().WithMessage("MessageSubject is required.");
            RuleFor(x => x.MessageBody).NotEmpty().WithMessage("MessageBody is required.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");
        }
    }
}
