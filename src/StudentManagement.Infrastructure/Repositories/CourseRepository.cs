using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;
using StudentManagement.Infrastructure.Data;

namespace StudentManagement.Infrastructure.Repositories;

public class CourseRepository : Repository<Course, Guid>, ICourseRepository
{
    public CourseRepository(StudentManagementDbContext context) : base(context)
    {
    }

    public async Task<Course?> GetByCourseCodeAsync(CourseCode courseCode, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Code == courseCode, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetActiveCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(c => c.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetCoursesByDepartmentAsync(string department, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(c => c.Department == department).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetAvailableCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.IsActive)
            .Include(c => c.Enrollments)
            .Where(c => c.Enrollments.Count(e => e.IsActive) < c.MaxEnrollment)
            .ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Grade)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetPrerequisitesAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await DbSet.FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);
        if (course == null)
            return Enumerable.Empty<Course>();

        var prerequisiteIds = course.Prerequisites.ToList();
        if (!prerequisiteIds.Any())
            return Enumerable.Empty<Course>();

        return await DbSet.Where(c => prerequisiteIds.Contains(c.Id)).ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCourseCodeUniqueAsync(CourseCode courseCode, Guid? excludeCourseId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(c => c.Code == courseCode);
        
        if (excludeCourseId != null)
        {
            query = query.Where(c => c.Id != excludeCourseId);
        }
        
        return !await query.AnyAsync(cancellationToken);
    }
}