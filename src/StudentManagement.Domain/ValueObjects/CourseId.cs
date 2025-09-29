namespace StudentManagement.Domain.ValueObjects;

public record CourseId(Guid Value)
{
    public static CourseId New() => new(Guid.NewGuid());
    
    public static CourseId From(Guid value) => new(value);
    
    public static CourseId From(string value) => 
        Guid.TryParse(value, out var guid) ? new(guid) : throw new ArgumentException("Invalid CourseId format", nameof(value));

    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(CourseId courseId) => courseId.Value;
    public static implicit operator CourseId(Guid value) => new(value);
}