namespace StudentManagement.Domain.ValueObjects;

public record EnrollmentId(Guid Value)
{
    public static EnrollmentId New() => new(Guid.NewGuid());
    
    public static EnrollmentId From(Guid value) => new(value);
    
    public static EnrollmentId From(string value) => 
        Guid.TryParse(value, out var guid) ? new(guid) : throw new ArgumentException("Invalid EnrollmentId format", nameof(value));

    public override string ToString() => Value.ToString();
    
    public static implicit operator Guid(EnrollmentId enrollmentId) => enrollmentId.Value;
    public static implicit operator EnrollmentId(Guid value) => new(value);
}