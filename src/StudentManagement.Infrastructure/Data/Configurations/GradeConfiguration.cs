using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Data.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.LetterGrade)
            .IsRequired()
            .HasMaxLength(3);
            
        builder.Property(g => g.GradePoints)
            .IsRequired()
            .HasColumnType("decimal(3,2)");
            
        builder.Property(g => g.NumericScore)
            .IsRequired(false)
            .HasColumnType("decimal(5,2)");
            
        builder.Property(g => g.Comments)
            .HasMaxLength(1000);
            
        builder.Property(g => g.GradedDate)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(g => g.GradedBy)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(g => g.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(g => g.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        // Ignore computed properties
        builder.Ignore(g => g.IsPassing);
        builder.Ignore(g => g.IsHonorGrade);
    }
}