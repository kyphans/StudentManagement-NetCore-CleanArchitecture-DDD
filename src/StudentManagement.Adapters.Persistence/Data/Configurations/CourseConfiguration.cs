using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Adapters.Persistence.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(10);
            
        builder.HasIndex(c => c.Code)
            .IsUnique();
            
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(c => c.Description)
            .HasMaxLength(1000);
            
        builder.Property(c => c.CreditHours)
            .IsRequired();
            
        builder.Property(c => c.Department)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(c => c.MaxEnrollment)
            .IsRequired()
            .HasDefaultValue(30);
            
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        // Ignore computed properties
        builder.Ignore(c => c.CurrentEnrollmentCount);

        // Configure relationships
        builder.HasMany(c => c.Enrollments)
            .WithOne(e => e.Course)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure prerequisites as a simple collection
        // Map to the private backing field instead of the public property
        builder.Property<List<Guid>>("_prerequisites")
            .HasConversion(
                v => string.Join(',', v.Select(id => id.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList())
            .HasColumnName("Prerequisites")
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Guid>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
    }
}