namespace StudentManagement.Application.DTOs;

// Response DTOs
public record StudentDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public DateTime EnrollmentDate { get; init; }
    public bool IsActive { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int Age { get; init; }
    public decimal GPA { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record StudentSummaryDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public decimal GPA { get; init; }
    public int TotalEnrollments { get; init; }
}

public record StudentWithEnrollmentsDto : StudentDto
{
    public ICollection<EnrollmentSummaryDto> Enrollments { get; init; } = new List<EnrollmentSummaryDto>();
}

// Request DTOs
public record CreateStudentDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
}

public record UpdateStudentDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public bool IsActive { get; init; }
}

// Filter DTOs
public record StudentFilterDto
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public DateTime? EnrollmentDateFrom { get; init; }
    public DateTime? EnrollmentDateTo { get; init; }
    public decimal? MinGPA { get; init; }
    public decimal? MaxGPA { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}