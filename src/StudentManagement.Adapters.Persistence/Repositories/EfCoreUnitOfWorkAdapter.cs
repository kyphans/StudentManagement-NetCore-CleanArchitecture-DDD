using Microsoft.EntityFrameworkCore.Storage;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Adapters.Persistence.Data;

namespace StudentManagement.Adapters.Persistence.Repositories;

public class EfCoreUnitOfWorkAdapter : IUnitOfWorkPort
{
    private readonly StudentManagementDbContext _context;
    private IDbContextTransaction? _transaction;

    public EfCoreUnitOfWorkAdapter(StudentManagementDbContext context)
    {
        _context = context;
        Students = new EfCoreStudentAdapter(_context);
        Courses = new EfCoreCourseAdapter(_context);
        Enrollments = new EfCoreEnrollmentAdapter(_context);
    }

    public IStudentPersistencePort Students { get; }
    public ICoursePersistencePort Courses { get; }
    public IEnrollmentPersistencePort Enrollments { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}