namespace StudentManagement.Domain.ValueObjects;

/// <summary>
/// Value object cho password đã hash
/// Không bao giờ lưu plain password!
/// </summary>
public class PasswordHash : IEquatable<PasswordHash>
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Tạo từ password đã hash (từ database)
    /// </summary>
    public static PasswordHash FromHash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            throw new ArgumentException("Password hash không được để trống");
        }

        return new PasswordHash(hashedPassword);
    }

    public bool Equals(PasswordHash? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PasswordHash);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(PasswordHash? left, PasswordHash? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(PasswordHash? left, PasswordHash? right)
    {
        return !(left == right);
    }
}