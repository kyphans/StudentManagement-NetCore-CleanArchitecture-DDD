using System.Text.RegularExpressions;

namespace StudentManagement.Domain.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        Value = ValidateAndFormat(value);
    }

    private static string ValidateAndFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        var formatted = value.Trim().ToLowerInvariant();
        
        if (!EmailRegex.IsMatch(formatted))
            throw new ArgumentException("Invalid email format", nameof(value));

        return formatted;
    }

    public override string ToString() => Value;
    
    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);
}