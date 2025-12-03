using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Common.Enums;

namespace StudentManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration cho User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Username (Value Object)
        builder.Property(u => u.Username)
            .HasConversion(
                username => username.Value,
                value => Domain.ValueObjects.Username.Create(value))
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        // Email (Value Object)
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Domain.ValueObjects.Email.Create(value))
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // PasswordHash (Value Object)
        builder.Property(u => u.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => Domain.ValueObjects.PasswordHash.FromHash(value))
            .IsRequired()
            .HasMaxLength(500);

        // FirstName
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        // LastName
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // Role (Enum)
        builder.Property(u => u.Role)
            .HasConversion<string>() // Store as string
            .IsRequired()
            .HasMaxLength(20);

        // IsActive
        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Timestamps
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        builder.Property(u => u.LastLoginAt)
            .IsRequired(false);

        // Ignore computed properties
        builder.Ignore(u => u.FullName);

        // Relationships
        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}