namespace NotificationService.Domain.ValueObjects;

public sealed record NotificationId
{
    public Guid Value { get; }

    public NotificationId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("NotificationId cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static NotificationId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(NotificationId id) => id.Value;
}
