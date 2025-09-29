namespace StudentManagement.Domain.ValueObjects;

public record GradeId(Guid Value)
{
    public static GradeId New() => new(Guid.NewGuid());
    
    public static GradeId From(Guid value) => new(value);
    
    public static GradeId From(string value) => 
        Guid.TryParse(value, out var guid) ? new(guid) : throw new ArgumentException("Invalid GradeId format", nameof(value));

    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(GradeId gradeId) => gradeId.Value;
    public static implicit operator GradeId(Guid value) => new(value);
}