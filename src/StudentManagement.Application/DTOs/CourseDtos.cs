namespace StudentManagement.Application.DTOs;

// Response DTOs
public record CourseDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string Department { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int MaxEnrollment { get; init; }
    public int CurrentEnrollmentCount { get; init; }
    public ICollection<Guid> Prerequisites { get; init; } = new List<Guid>();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CourseSummaryDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string Department { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int CurrentEnrollmentCount { get; init; }
    public int MaxEnrollment { get; init; }
    public bool CanEnroll { get; init; }
}

public record CourseWithEnrollmentsDto : CourseDto
{
    public ICollection<EnrollmentSummaryDto> Enrollments { get; init; } = new List<EnrollmentSummaryDto>();
}

public record CourseWithPrerequisitesDto : CourseDto
{
    public ICollection<CourseSummaryDto> PrerequisiteCourses { get; init; } = new List<CourseSummaryDto>();
}

// Request DTOs
public record CreateCourseDto
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string Department { get; init; } = string.Empty;
    public int MaxEnrollment { get; init; } = 30;
}

public record UpdateCourseDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string Department { get; init; } = string.Empty;
    public int MaxEnrollment { get; init; }
}

public record AddPrerequisiteDto
{
    public Guid PrerequisiteCourseId { get; init; }
}

// Filter DTOs
public record CourseFilterDto
{
    public string? SearchTerm { get; init; }
    public string? Department { get; init; }
    public bool? IsActive { get; init; }
    public bool? AvailableOnly { get; init; }
    public int? MinCreditHours { get; init; }
    public int? MaxCreditHours { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}