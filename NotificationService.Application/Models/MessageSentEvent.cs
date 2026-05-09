namespace NotificationService.Application.Models
{
    public record MessageSentEvent
    {
        public string MessageId { get; init; } = default!;
        public string ConversationId { get; init; } = default!;
        public string SenderId { get; init; } = default!;
        public string Content { get; init; } = default!;
        public DateTime Timestamp { get; init; }
        public ICollection<string> RecipientIds { get; init; } = new List<string>();
        public string? Email { get; init; }
    }
}
