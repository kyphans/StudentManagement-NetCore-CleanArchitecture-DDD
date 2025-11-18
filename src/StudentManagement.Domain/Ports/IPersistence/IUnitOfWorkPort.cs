namespace StudentManagement.Domain.Ports.IPersistence;

/// <summary>
/// Unit of Work port interface for managing transactions and coordinating persistence operations.
/// Secondary Port: Defines what transaction management operations the domain requires.
/// </summary>
public interface IUnitOfWorkPort : IDisposable
{
    IStudentPersistencePort Students { get; }
    ICoursePersistencePort Courses { get; }
    IEnrollmentPersistencePort Enrollments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
