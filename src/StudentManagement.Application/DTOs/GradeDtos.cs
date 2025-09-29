namespace StudentManagement.Application.DTOs;

// Response DTOs
public record GradeDto
{
    public Guid Id { get; init; }
    public string LetterGrade { get; init; } = string.Empty;
    public decimal GradePoints { get; init; }
    public decimal? NumericScore { get; init; }
    public string? Comments { get; init; }
    public DateTime GradedDate { get; init; }
    public string GradedBy { get; init; } = string.Empty;
    public bool IsPassing { get; init; }
    public bool IsHonorGrade { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record GradeSummaryDto
{
    public Guid Id { get; init; }
    public string LetterGrade { get; init; } = string.Empty;
    public decimal GradePoints { get; init; }
    public decimal? NumericScore { get; init; }
    public DateTime GradedDate { get; init; }
    public string GradedBy { get; init; } = string.Empty;
    public bool IsPassing { get; init; }
    public bool IsHonorGrade { get; init; }
}

// Request DTOs
public record CreateGradeFromLetterDto
{
    public string LetterGrade { get; init; } = string.Empty;
    public decimal GradePoints { get; init; }
    public decimal? NumericScore { get; init; }
    public string? Comments { get; init; }
    public string GradedBy { get; init; } = string.Empty;
}

public record CreateGradeFromNumericDto
{
    public decimal NumericScore { get; init; }
    public string? Comments { get; init; }
    public string GradedBy { get; init; } = string.Empty;
}

public record UpdateGradeDto
{
    public string LetterGrade { get; init; } = string.Empty;
    public decimal GradePoints { get; init; }
    public decimal? NumericScore { get; init; }
    public string? Comments { get; init; }
}