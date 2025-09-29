namespace StudentManagement.Domain.ValueObjects;

public record GPA
{
    public const decimal MinValue = 0.0m;
    public const decimal MaxValue = 4.0m;

    public decimal Value { get; }

    public GPA(decimal value)
    {
        Value = ValidateValue(value);
    }

    private static decimal ValidateValue(decimal value)
    {
        if (value < MinValue || value > MaxValue)
            throw new ArgumentException($"GPA must be between {MinValue} and {MaxValue}", nameof(value));

        return Math.Round(value, 2);
    }

    public bool IsHonorRoll => Value >= 3.5m;
    public bool IsPassing => Value >= 2.0m;

    public override string ToString() => Value.ToString("F2");
    
    public static implicit operator decimal(GPA gpa) => gpa.Value;
    public static implicit operator GPA(decimal value) => new(value);
}