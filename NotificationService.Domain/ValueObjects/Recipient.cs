namespace NotificationService.Domain.ValueObjects;

public sealed record Recipient
{
    public string RecipientId { get; }
    public Email? Email { get; }
    public PhoneNumber? PhoneNumber { get; }

    public Recipient(string recipientId, Email? email, PhoneNumber? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientId))
        {
            throw new ArgumentException("RecipientId is required.", nameof(recipientId));
        }

        if (email is null && phoneNumber is null)
        {
            throw new ArgumentException("Recipient must have at least an email or a phone number.");
        }

        RecipientId = recipientId.Trim();
        Email = email;
        PhoneNumber = phoneNumber;
    }
}
