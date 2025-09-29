using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.StudentId)
            .IsRequired();
            
        builder.Property(e => e.CourseId)
            .IsRequired();
            
        builder.Property(e => e.EnrollmentDate)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(e => e.CompletionDate)
            .IsRequired(false);
            
        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(e => e.CreditHours)
            .IsRequired();
            
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        // Ignore computed properties
        builder.Ignore(e => e.IsActive);
        builder.Ignore(e => e.IsCompleted);
        builder.Ignore(e => e.IsWithdrawn);

        // Configure relationships
        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Grade)
            .WithOne()
            .HasForeignKey<Enrollment>("GradeId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Ensure unique enrollment per student per course
        builder.HasIndex(e => new { e.StudentId, e.CourseId })
            .IsUnique();
    }
}