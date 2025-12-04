namespace StudentManagement.Domain.Entities;

/// <summary>
/// Entity để quản lý refresh tokens
/// Refresh token dùng để lấy access token mới khi token cũ hết hạn
/// KHÔNG kế thừa BaseEntity vì RefreshToken là immutable (không cần UpdatedAt)
/// </summary>
public class RefreshToken
{
    // Primary key
    public Guid Id { get; private set; }

    // Foreign key
    public Guid UserId { get; private set; }

    // Token data
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedByIp { get; private set; } = string.Empty;

    // Revocation data
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; private set; }

    // Computed properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Constructor private để bắt buộc dùng factory method
    private RefreshToken() { }

    /// <summary>
    /// Factory method để tạo refresh token mới
    /// </summary>
    public static RefreshToken Create(
        Guid userId,
        string token,
        int expiryDays,
        string createdByIp)
    {
        // Validations
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID không hợp lệ");

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token không được để trống");

        if (expiryDays <= 0)
            throw new ArgumentException("Expiry days phải > 0");

        if (string.IsNullOrWhiteSpace(createdByIp))
            throw new ArgumentException("Created by IP không được để trống");

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = createdByIp
        };

        // NOTE: UpdatedAt is ignored in EF Core config, but we need to prevent BaseEntity from setting it
        // This is a workaround since RefreshToken is immutable
        return refreshToken;
    }

    /// <summary>
    /// Revoke (thu hồi) refresh token
    /// </summary>
    public void Revoke(string revokedByIp, string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new InvalidOperationException("Token đã bị revoke rồi");

        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}