namespace NotificationService.Domain.ValueObjects;

public sealed record Recipient
{
    public string RecipientId { get; }
    public Email Email { get; }

    public Recipient(string recipientId, Email email)
    {
        if (string.IsNullOrWhiteSpace(recipientId))
        {
            throw new ArgumentException("RecipientId is required.", nameof(recipientId));
        }

        RecipientId = recipientId.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
}
