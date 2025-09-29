using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;
using StudentManagement.Infrastructure.Data;

namespace StudentManagement.Infrastructure.Repositories;

public class StudentRepository : Repository<Student, StudentId>, IStudentRepository
{
    public StudentRepository(StudentManagementDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(s => s.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Student>> GetStudentsByEnrollmentDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.EnrollmentDate >= startDate && s.EnrollmentDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Student>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await DbSet
            .Where(s => s.FirstName.ToLower().Contains(lowerSearchTerm) || 
                       s.LastName.ToLower().Contains(lowerSearchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<Student?> GetWithEnrollmentsAsync(StudentId id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Grade)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, StudentId? excludeStudentId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(s => s.Email == email);
        
        if (excludeStudentId != null)
        {
            query = query.Where(s => s.Id != excludeStudentId);
        }
        
        return !await query.AnyAsync(cancellationToken);
    }
}