using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Ports.IPersistence;

/// <summary>
/// Student persistence port interface.
/// Secondary Port: Defines what operations the domain requires for student data persistence.
/// </summary>
public interface IStudentPersistencePort : IPersistencePort<Student, StudentId>
{
    Task<Student?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetActiveStudentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetStudentsByEnrollmentDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Student?> GetWithEnrollmentsAsync(StudentId id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, StudentId? excludeStudentId = null, CancellationToken cancellationToken = default);
}
