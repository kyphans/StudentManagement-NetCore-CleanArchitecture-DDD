using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;
using StudentManagement.Adapters.Persistence.Data;

namespace StudentManagement.Adapters.Persistence.Repositories;

/// <summary>
/// EF Core implementation of Enrollment persistence port (Secondary Adapter).
/// </summary>
public class EfCoreEnrollmentAdapter : EfCoreRepositoryBase<Enrollment, Guid>, IEnrollmentPersistencePort
{
    public EfCoreEnrollmentAdapter(StudentManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(StudentId studentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Course)
            .Include(e => e.Grade)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Student)
            .Include(e => e.Grade)
            .Where(e => e.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetActiveEnrollmentsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.Status == EnrollmentStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetCompletedEnrollmentsAsync(StudentId studentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Course)
            .Include(e => e.Grade)
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed)
            .ToListAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetActiveEnrollmentAsync(StudentId studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Include(e => e.Grade)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && 
                               e.CourseId == courseId && 
                               e.Status == EnrollmentStatus.Active, 
                               cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetEnrollmentsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Where(e => e.EnrollmentDate >= startDate && e.EnrollmentDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetWithStudentAndCourseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Student)
            .Include(e => e.Course)
            .Include(e => e.Grade)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> IsStudentEnrolledAsync(StudentId studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.StudentId == studentId && 
                                   e.CourseId == courseId && 
                                   e.Status == EnrollmentStatus.Active, 
                                   cancellationToken);
    }
}