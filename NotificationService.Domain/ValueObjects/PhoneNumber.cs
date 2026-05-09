namespace NotificationService.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Phone number is required.", nameof(value));
        }

        Value = value.Trim();
    }

    public override string ToString() => Value;
}
