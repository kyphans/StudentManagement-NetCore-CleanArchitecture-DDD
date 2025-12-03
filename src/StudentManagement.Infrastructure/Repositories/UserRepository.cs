using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;
using StudentManagement.Infrastructure.Data;

namespace StudentManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation cho User entity
/// </summary>
public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(StudentManagementDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(
        Username username,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(
        Username username,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(u => u.Username == username);

        if (excludeUserId != null)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(
        Email email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(u => u.Email == email);

        if (excludeUserId != null)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<User?> GetWithRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken),
                cancellationToken);
    }
}