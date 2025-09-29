using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Repositories;

public interface ICourseRepository : IRepository<Course, Guid>
{
    Task<Course?> GetByCourseCodeAsync(CourseCode courseCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetActiveCoursesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetCoursesByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetAvailableCoursesAsync(CancellationToken cancellationToken = default);
    Task<Course?> GetWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetPrerequisitesAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> IsCourseCodeUniqueAsync(CourseCode courseCode, Guid? excludeCourseId = null, CancellationToken cancellationToken = default);
}