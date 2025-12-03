using System.Text.RegularExpressions;

namespace StudentManagement.Domain.ValueObjects;

/// <summary>
/// Value object cho Username
/// Username phải từ 3-50 ký tự, chỉ chứa chữ, số, underscore, dấu chấm
/// </summary>
public class Username : IEquatable<Username>
{
    public string Value { get; }

    // Constructor private để bắt buộc dùng factory method
    private Username(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Factory method để tạo Username với validation
    /// </summary>
    public static Username Create(string value)
    {
        // Validation 1: Null hoặc empty
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Username không được để trống");
        }

        // Validation 2: Độ dài
        if (value.Length < 3 || value.Length > 50)
        {
            throw new ArgumentException("Username phải từ 3-50 ký tự");
        }

        // Validation 3: Format (chỉ chữ, số, underscore, dấu chấm)
        var regex = new Regex(@"^[a-zA-Z0-9_.]+$");
        if (!regex.IsMatch(value))
        {
            throw new ArgumentException("Username chỉ được chứa chữ, số, underscore và dấu chấm");
        }

        return new Username(value.ToLower()); // Normalize về lowercase
    }

    // Implement IEquatable để so sánh
    public bool Equals(Username? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Username);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    // Operator overloading để so sánh
    public static bool operator ==(Username? left, Username? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Username? left, Username? right)
    {
        return !(left == right);
    }
}