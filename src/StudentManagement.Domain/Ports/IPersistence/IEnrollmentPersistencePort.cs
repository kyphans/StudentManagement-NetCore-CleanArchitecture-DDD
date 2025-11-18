using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Ports.IPersistence;

/// <summary>
/// Enrollment persistence port interface.
/// Secondary Port: Defines what operations the domain requires for enrollment data persistence.
/// </summary>
public interface IEnrollmentPersistencePort : IPersistencePort<Enrollment, Guid>
{
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(StudentId studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetActiveEnrollmentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetCompletedEnrollmentsAsync(StudentId studentId, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetActiveEnrollmentAsync(StudentId studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetEnrollmentsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetWithStudentAndCourseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsStudentEnrolledAsync(StudentId studentId, Guid courseId, CancellationToken cancellationToken = default);
}
