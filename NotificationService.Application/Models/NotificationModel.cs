namespace NotificationService.Application.Models
{
    public record NotificationModel
    {
        public string? RecipientId { get; init; }
        public string? Email { get; init; }
        public string? MessageSubject { get; init; }
        public string? MessageBody { get; init; }
    }
}
