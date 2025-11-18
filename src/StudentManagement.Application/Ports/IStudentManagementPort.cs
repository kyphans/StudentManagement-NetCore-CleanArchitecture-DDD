using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Ports;

/// <summary>
/// Student management primary port interface.
/// Primary Port (Driving/Inbound): Defines what operations external actors can perform on student management.
/// This interface represents the application's API for student-related operations.
/// </summary>
public interface IStudentManagementPort
{
    /// <summary>
    /// Creates a new student in the system.
    /// </summary>
    /// <param name="request">Student creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created student details</returns>
    Task<StudentDto> CreateStudentAsync(CreateStudentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing student's information.
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="request">Updated student data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated student details</returns>
    Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a student from the system.
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a student by their ID.
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Student details or null if not found</returns>
    Task<StudentDto?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of students with optional filtering.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search term for filtering</param>
    /// <param name="isActive">Optional filter for active/inactive students</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of students</returns>
    Task<PagedResultDto<StudentSummaryDto>> GetStudentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a student with their enrollment details.
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Student with enrollments or null if not found</returns>
    Task<StudentWithEnrollmentsDto?> GetStudentWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default);
}
