namespace StudentManagement.Domain.ValueObjects;

public record CourseCode
{
    public string Value { get; }

    public CourseCode(string value)
    {
        Value = ValidateAndFormat(value);
    }

    private static string ValidateAndFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Course code cannot be empty", nameof(value));

        var formatted = value.Trim().ToUpperInvariant();
        
        if (formatted.Length < 3 || formatted.Length > 10)
            throw new ArgumentException("Course code must be between 3 and 10 characters", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(formatted, @"^[A-Z0-9]+$"))
            throw new ArgumentException("Course code can only contain letters and numbers", nameof(value));

        return formatted;
    }

    public override string ToString() => Value;
    
    public static implicit operator string(CourseCode courseCode) => courseCode.Value;
    public static implicit operator CourseCode(string value) => new(value);
}