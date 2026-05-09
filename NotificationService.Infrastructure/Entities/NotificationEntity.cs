namespace NotificationService.Infrastructure.Entities;

public sealed class NotificationEntity
{
    public Guid Id { get; set; }
    public string RecipientId { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string MessageSubject { get; set; } = default!;
    public string MessageBody { get; set; } = default!;
    public int NotificationType { get; set; }
    public int Status { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}
