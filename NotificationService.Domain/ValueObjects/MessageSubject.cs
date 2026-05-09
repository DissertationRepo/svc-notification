namespace NotificationService.Domain.ValueObjects;

public sealed record MessageSubject
{
    public string Value { get; }

    public MessageSubject(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Message subject is required.", nameof(value));
        }

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
