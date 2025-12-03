using StudentManagement.Domain.Common.Enums;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Entities;

/// <summary>
/// User entity - Aggregate Root
/// Quản lý authentication và authorization
/// </summary>
public class User : BaseEntity<Guid>
{
    // Properties
    public Username Username { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Navigation property
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    // Computed property
    public string FullName => $"{FirstName} {LastName}";

    // Constructor private
    private User() {}

    /// <summary>
    /// Factory method để tạo User mới
    /// </summary>
    public static User Create(
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        UserRole role)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name không được để trống");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name không được để trống");

        if (firstName.Length > 50)
            throw new ArgumentException("First name không được quá 50 ký tự");

        if (lastName.Length > 50)
            throw new ArgumentException("Last name không được quá 50 ký tự");

        // Tạo user mới
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = Username.Create(username),
            Email = Email.Create(email),
            PasswordHash = PasswordHash.FromHash(passwordHash),
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return user;
    }

    /// <summary>
    /// Cập nhật thông tin user
    /// </summary>
    public void UpdateInfo(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name không được để trống");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name không được để trống");

        FirstName = firstName;
        LastName = lastName;
        Email = Email.Create(email);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đổi password
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash không được để trống");

        PasswordHash = PasswordHash.FromHash(newPasswordHash);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đổi role
    /// </summary>
    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activate user
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("User đã active rồi");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivate user
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User đã bị deactivate rồi");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật last login time
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Thêm refresh token
    /// </summary>
    public void AddRefreshToken(RefreshToken token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        _refreshTokens.Add(token);
    }

    /// <summary>
    /// Revoke tất cả refresh tokens
    /// </summary>
    public void RevokeAllRefreshTokens(string revokedByIp)
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke(revokedByIp);
        }
    }

    /// <summary>
    /// Xóa các refresh tokens đã expired
    /// </summary>
    public void RemoveExpiredRefreshTokens()
    {
        _refreshTokens.RemoveAll(t => t.IsExpired);
    }
}