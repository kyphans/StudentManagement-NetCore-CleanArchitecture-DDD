namespace StudentManagement.Domain.ValueObjects;

public record StudentId(Guid Value)
{
    public static StudentId New() => new(Guid.NewGuid());
    
    public static StudentId From(Guid value) => new(value);
    
    public static StudentId From(string value) => 
        Guid.TryParse(value, out var guid) ? new(guid) : throw new ArgumentException("Invalid StudentId format", nameof(value));

    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(StudentId studentId) => studentId.Value;
    public static implicit operator StudentId(Guid value) => new(value);
}