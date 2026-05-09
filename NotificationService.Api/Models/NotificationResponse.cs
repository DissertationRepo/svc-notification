namespace NotificationService.Api.Models
{
    public record NotificationResponse
    {
        public string? Id { get; init; }
        public string? RecipientId { get; init; }
        public string? Email { get; init; }
        public string? MessageSubject { get; init; }
        public string? MessageBody { get; init; }
        public string? Status { get; init; }
        public string? FailureReason { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? SentAt { get; init; }
    }
}
