using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Ports;

/// <summary>
/// Enrollment management primary port interface.
/// Primary Port (Driving/Inbound): Defines what operations external actors can perform on enrollment management.
/// This interface represents the application's API for enrollment-related operations.
/// </summary>
public interface IEnrollmentManagementPort
{
    /// <summary>
    /// Creates a new enrollment (student enrolls in a course).
    /// </summary>
    /// <param name="request">Enrollment creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created enrollment details</returns>
    Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a grade to an existing enrollment.
    /// </summary>
    /// <param name="enrollmentId">Enrollment ID</param>
    /// <param name="request">Grade assignment data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated enrollment with grade</returns>
    Task<EnrollmentDto> AssignGradeAsync(Guid enrollmentId, AssignGradeDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an enrollment by its ID.
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enrollment details or null if not found</returns>
    Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of enrollments with optional filtering.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="studentId">Optional student ID filter</param>
    /// <param name="courseId">Optional course ID filter</param>
    /// <param name="status">Optional status filter (Active, Completed, Withdrawn)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of enrollments</returns>
    Task<PagedResultDto<EnrollmentSummaryDto>> GetEnrollmentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        Guid? studentId = null,
        Guid? courseId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an enrollment with full student and course details.
    /// </summary>
    /// <param name="id">Enrollment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enrollment with details or null if not found</returns>
    Task<EnrollmentWithDetailsDto?> GetEnrollmentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all enrollments for a specific student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of enrollments for the student</returns>
    Task<IEnumerable<EnrollmentSummaryDto>> GetEnrollmentsByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all enrollments for a specific course.
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of enrollments for the course</returns>
    Task<IEnumerable<EnrollmentSummaryDto>> GetEnrollmentsByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
}
