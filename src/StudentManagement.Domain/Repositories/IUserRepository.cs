using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Repositories;

/// <summary>
/// Repository interface cho User entity
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// Lấy user theo username
    /// </summary>
    Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy user theo email
    /// </summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra username đã tồn tại chưa
    /// </summary>
    Task<bool> IsUsernameUniqueAsync(Username username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    Task<bool> IsEmailUniqueAsync(Email email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy user với refresh tokens
    /// </summary>
    Task<User?> GetWithRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy user theo refresh token
    /// </summary>
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}