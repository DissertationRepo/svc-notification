namespace NotificationService.Domain.ValueObjects;

public sealed record MessageBody
{
    public string Value { get; }

    public MessageBody(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Message body is required.", nameof(value));
        }

        Value = value;
    }

    public override string ToString() => Value;
}
