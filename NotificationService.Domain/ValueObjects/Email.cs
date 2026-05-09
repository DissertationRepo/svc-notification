using System.Text.RegularExpressions;

namespace NotificationService.Domain.ValueObjects;

public sealed record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value), "Email is required.");
        }

        var normalizedValue = value.Trim();

        if (!EmailRegex.IsMatch(normalizedValue))
        {
            throw new ArgumentException("Email format is invalid.", nameof(value));
        }

        Value = normalizedValue;
    }

    public override string ToString() => Value;
}
