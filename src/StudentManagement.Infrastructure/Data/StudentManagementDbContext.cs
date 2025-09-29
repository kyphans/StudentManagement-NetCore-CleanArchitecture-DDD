using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;
using StudentManagement.Infrastructure.Data.Configurations;

namespace StudentManagement.Infrastructure.Data;

public class StudentManagementDbContext : IdentityDbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<Grade> Grades { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new GradeConfiguration());

        // Configure value object conversions
        modelBuilder.Entity<Student>()
            .Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new StudentId(value));

        modelBuilder.Entity<Student>()
            .Property(s => s.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value));

        modelBuilder.Entity<Course>()
            .Property(c => c.Code)
            .HasConversion(
                code => code.Value,
                value => new CourseCode(value));

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.StudentId)
            .HasConversion(
                id => id.Value,
                value => new StudentId(value));
    }
}