using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces;

/// <summary>
/// Interface cho JWT token service
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate access token (JWT)
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate access token
    /// </summary>
    bool ValidateToken(string token);

    /// <summary>
    /// Get user ID từ token
    /// </summary>
    Guid? GetUserIdFromToken(string token);
}