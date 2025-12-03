namespace StudentManagement.Application.DTOs;

// ============ REQUEST DTOs ============

/// <summary>
/// DTO cho request đăng ký user mới
/// </summary>
public record RegisterRequestDto
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = "Student"; // Default role
}

/// <summary>
/// DTO cho request đăng nhập
/// </summary>
public record LoginRequestDto
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// DTO cho request refresh token
/// </summary>
public record RefreshTokenRequestDto
{
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// DTO cho request đổi password
/// </summary>
public record ChangePasswordRequestDto
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}

// ============ RESPONSE DTOs ============

/// <summary>
/// DTO cho response sau khi đăng nhập thành công
/// </summary>
public record AuthenticationResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}

/// <summary>
/// DTO cho User
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}