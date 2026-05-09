namespace NotificationService.Api.Models
{
    public record SendNotification
    {
        public string? RecipientId { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? MessageSubject { get; init; }
        public string? MessageBody { get; init; }
        public string? NotificationType { get; init; }
    }
}
