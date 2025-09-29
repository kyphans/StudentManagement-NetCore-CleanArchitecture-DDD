using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.HasIndex(s => s.Email)
            .IsUnique();
            
        builder.Property(s => s.DateOfBirth)
            .IsRequired()
            .HasColumnType("date");
            
        builder.Property(s => s.EnrollmentDate)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(s => s.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        // Ignore computed properties
        builder.Ignore(s => s.FullName);
        builder.Ignore(s => s.Age);

        // Configure relationships
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}