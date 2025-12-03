namespace StudentManagement.Application.Interfaces;

/// <summary>
/// Interface cho password hashing service
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify password với hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}