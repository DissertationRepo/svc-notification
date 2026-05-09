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
            RuleFor(x => x.NotificationType)
                .NotEmpty().WithMessage("NotificationType is required.")
                .Must(type => string.Equals(type, "Email", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(type, "Sms", StringComparison.OrdinalIgnoreCase))
                .WithMessage("NotificationType must be 'Email' or 'Sms'.");

            RuleFor(x => x).Must(x =>
                !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Either Email or PhoneNumber must be provided.");

            When(x => string.Equals(x.NotificationType, "Email", StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required for Email notifications.");
            });

            When(x => string.Equals(x.NotificationType, "Sms", StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required for SMS notifications.");
            });
        }
    }
}
