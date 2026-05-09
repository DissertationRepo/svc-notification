using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Entities.Aggregates;

public sealed class Notification
{
    public NotificationId Id { get; }
    public Recipient Recipient { get; }
    public MessageSubject MessageSubject { get; }
    public MessageBody MessageBody { get; }
    public NotificationType NotificationType { get; }
    public NotificationStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? SentAt { get; private set; }

    private Notification(
        NotificationId id,
        Recipient recipient,
        MessageSubject messageSubject,
        MessageBody messageBody,
        NotificationType notificationType,
        NotificationStatus status,
        string? failureReason,
        DateTime createdAt,
        DateTime? sentAt)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        MessageSubject = messageSubject ?? throw new ArgumentNullException(nameof(messageSubject));
        MessageBody = messageBody ?? throw new ArgumentNullException(nameof(messageBody));
        NotificationType = notificationType;
        Status = status;
        FailureReason = failureReason;
        CreatedAt = createdAt;
        SentAt = sentAt;

        EnsureChannelMatchesRecipient();
    }

    public static Notification Create(
        Recipient recipient,
        MessageSubject messageSubject,
        MessageBody messageBody,
        NotificationType notificationType)
    {
        return new Notification(
            NotificationId.New(),
            recipient,
            messageSubject,
            messageBody,
            notificationType,
            NotificationStatus.Pending,
            failureReason: null,
            createdAt: DateTime.UtcNow,
            sentAt: null);
    }

    public static Notification Load(
        NotificationId id,
        Recipient recipient,
        MessageSubject messageSubject,
        MessageBody messageBody,
        NotificationType notificationType,
        NotificationStatus status,
        string? failureReason,
        DateTime createdAt,
        DateTime? sentAt)
    {
        return new Notification(
            id,
            recipient,
            messageSubject,
            messageBody,
            notificationType,
            status,
            failureReason,
            createdAt,
            sentAt);
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        FailureReason = null;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = NotificationStatus.Failed;
        FailureReason = string.IsNullOrWhiteSpace(reason) ? "Unknown error." : reason.Trim();
    }

    private void EnsureChannelMatchesRecipient()
    {
        switch (NotificationType)
        {
            case NotificationType.Email when Recipient.Email is null:
                throw new InvalidOperationException("An email recipient address is required for email notifications.");
            case NotificationType.Sms when Recipient.PhoneNumber is null:
                throw new InvalidOperationException("A recipient phone number is required for SMS notifications.");
        }
    }
}

public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}
