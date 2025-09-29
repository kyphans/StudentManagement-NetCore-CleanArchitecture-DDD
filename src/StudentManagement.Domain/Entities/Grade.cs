namespace StudentManagement.Domain.Entities;

public class Grade : BaseEntity<Guid>
{
    public string LetterGrade { get; private set; } = string.Empty;
    public decimal GradePoints { get; private set; }
    public decimal? NumericScore { get; private set; }
    public string? Comments { get; private set; }
    public DateTime GradedDate { get; private set; }
    public string GradedBy { get; private set; } = string.Empty;

    public bool IsPassing => GradePoints >= 2.0m;
    public bool IsHonorGrade => GradePoints >= 3.5m;

    protected Grade() { } // For EF Core

    private Grade(Guid id, string letterGrade, decimal gradePoints, decimal? numericScore, 
                 string? comments, string gradedBy) : base(id)
    {
        LetterGrade = ValidateLetterGrade(letterGrade);
        GradePoints = ValidateGradePoints(gradePoints);
        NumericScore = ValidateNumericScore(numericScore);
        Comments = comments?.Trim();
        GradedBy = ValidateGradedBy(gradedBy);
        GradedDate = DateTime.UtcNow;
    }

    public static Grade Create(string letterGrade, decimal gradePoints, string gradedBy, 
                             decimal? numericScore = null, string? comments = null)
    {
        return new Grade(Guid.NewGuid(), letterGrade, gradePoints, numericScore, comments, gradedBy);
    }

    public static Grade CreateFromNumericScore(decimal numericScore, string gradedBy, string? comments = null)
    {
        var (letterGrade, gradePoints) = ConvertNumericToLetterGrade(numericScore);
        return new Grade(Guid.NewGuid(), letterGrade, gradePoints, numericScore, comments, gradedBy);
    }

    public void UpdateGrade(string letterGrade, decimal gradePoints, decimal? numericScore = null, string? comments = null)
    {
        LetterGrade = ValidateLetterGrade(letterGrade);
        GradePoints = ValidateGradePoints(gradePoints);
        NumericScore = ValidateNumericScore(numericScore);
        Comments = comments?.Trim();
        UpdateTimestamp();
    }

    public void UpdateComments(string? comments)
    {
        Comments = comments?.Trim();
        UpdateTimestamp();
    }

    private static string ValidateLetterGrade(string letterGrade)
    {
        if (string.IsNullOrWhiteSpace(letterGrade))
            throw new ArgumentException("Letter grade cannot be empty");

        var valid = new[] { "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D+", "D", "D-", "F", "I", "W" };
        var normalized = letterGrade.Trim().ToUpperInvariant();

        if (!valid.Contains(normalized))
            throw new ArgumentException($"Invalid letter grade: {letterGrade}");

        return normalized;
    }

    private static decimal ValidateGradePoints(decimal gradePoints)
    {
        if (gradePoints < 0.0m || gradePoints > 4.0m)
            throw new ArgumentException("Grade points must be between 0.0 and 4.0");

        return Math.Round(gradePoints, 2);
    }

    private static decimal? ValidateNumericScore(decimal? numericScore)
    {
        if (numericScore.HasValue)
        {
            if (numericScore < 0 || numericScore > 100)
                throw new ArgumentException("Numeric score must be between 0 and 100");

            return Math.Round(numericScore.Value, 2);
        }

        return null;
    }

    private static string ValidateGradedBy(string gradedBy)
    {
        if (string.IsNullOrWhiteSpace(gradedBy))
            throw new ArgumentException("GradedBy cannot be empty");

        var trimmed = gradedBy.Trim();
        if (trimmed.Length > 100)
            throw new ArgumentException("GradedBy cannot exceed 100 characters");

        return trimmed;
    }

    private static (string letterGrade, decimal gradePoints) ConvertNumericToLetterGrade(decimal numericScore)
    {
        return numericScore switch
        {
            >= 97 => ("A+", 4.0m),
            >= 93 => ("A", 4.0m),
            >= 90 => ("A-", 3.7m),
            >= 87 => ("B+", 3.3m),
            >= 83 => ("B", 3.0m),
            >= 80 => ("B-", 2.7m),
            >= 77 => ("C+", 2.3m),
            >= 73 => ("C", 2.0m),
            >= 70 => ("C-", 1.7m),
            >= 67 => ("D+", 1.3m),
            >= 63 => ("D", 1.0m),
            >= 60 => ("D-", 0.7m),
            _ => ("F", 0.0m)
        };
    }
}