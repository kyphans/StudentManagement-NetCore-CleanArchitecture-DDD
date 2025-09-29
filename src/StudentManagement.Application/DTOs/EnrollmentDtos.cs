namespace StudentManagement.Application.DTOs;

// Response DTOs
public record EnrollmentDto
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public Guid CourseId { get; init; }
    public DateTime EnrollmentDate { get; init; }
    public DateTime? CompletionDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public GradeDto? Grade { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record EnrollmentSummaryDto
{
    public Guid Id { get; init; }
    public Guid StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;
    public Guid CourseId { get; init; }
    public string CourseCode { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public DateTime EnrollmentDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string? FinalGrade { get; init; }
    public decimal? GradePoints { get; init; }
}

public record EnrollmentWithDetailsDto : EnrollmentDto
{
    public StudentSummaryDto Student { get; init; } = null!;
    public CourseSummaryDto Course { get; init; } = null!;
}

// Request DTOs
public record CreateEnrollmentDto
{
    public Guid StudentId { get; init; }
    public Guid CourseId { get; init; }
    public int CreditHours { get; init; }
}

public record AssignGradeDto
{
    public string LetterGrade { get; init; } = string.Empty;
    public decimal GradePoints { get; init; }
    public decimal? NumericScore { get; init; }
    public string? Comments { get; init; }
    public string GradedBy { get; init; } = string.Empty;
}

// Filter DTOs
public record EnrollmentFilterDto
{
    public Guid? StudentId { get; init; }
    public Guid? CourseId { get; init; }
    public string? Status { get; init; }
    public DateTime? EnrollmentDateFrom { get; init; }
    public DateTime? EnrollmentDateTo { get; init; }
    public DateTime? CompletionDateFrom { get; init; }
    public DateTime? CompletionDateTo { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}