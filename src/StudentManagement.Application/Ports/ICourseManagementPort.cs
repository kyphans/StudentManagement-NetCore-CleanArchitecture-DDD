using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Ports;

/// <summary>
/// Course management primary port interface.
/// Primary Port (Driving/Inbound): Defines what operations external actors can perform on course management.
/// This interface represents the application's API for course-related operations.
/// </summary>
public interface ICourseManagementPort
{
    /// <summary>
    /// Creates a new course in the system.
    /// </summary>
    /// <param name="request">Course creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created course details</returns>
    Task<CourseDto> CreateCourseAsync(CreateCourseDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing course's information.
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="request">Updated course data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated course details</returns>
    Task<CourseDto> UpdateCourseAsync(Guid id, UpdateCourseDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a course from the system.
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a course by its ID.
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Course details or null if not found</returns>
    Task<CourseDto?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of courses with optional filtering.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search term for filtering</param>
    /// <param name="department">Optional department filter</param>
    /// <param name="isActive">Optional filter for active/inactive courses</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of courses</returns>
    Task<PagedResultDto<CourseSummaryDto>> GetCoursesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? department = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a course with its enrollment details.
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Course with enrollments or null if not found</returns>
    Task<CourseWithEnrollmentsDto?> GetCourseWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a course with its prerequisite details.
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Course with prerequisites or null if not found</returns>
    Task<CourseWithPrerequisitesDto?> GetCourseWithPrerequisitesAsync(Guid id, CancellationToken cancellationToken = default);
}
