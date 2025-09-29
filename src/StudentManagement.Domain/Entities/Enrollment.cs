using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Entities;

public class Enrollment : BaseEntity<Guid>
{
    public StudentId StudentId { get; private set; } = null!;
    public Guid CourseId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public int CreditHours { get; private set; }
    public Grade? Grade { get; private set; }

    // Navigation properties
    public Student Student { get; private set; } = null!;
    public Course Course { get; private set; } = null!;

    public bool IsActive => Status == EnrollmentStatus.Active;
    public bool IsCompleted => Status == EnrollmentStatus.Completed;
    public bool IsWithdrawn => Status == EnrollmentStatus.Withdrawn;

    protected Enrollment() { } // For EF Core

    private Enrollment(Guid id, StudentId studentId, Guid courseId, int creditHours) : base(id)
    {
        StudentId = studentId;
        CourseId = courseId;
        CreditHours = ValidateCreditHours(creditHours);
        EnrollmentDate = DateTime.UtcNow;
        Status = EnrollmentStatus.Active;
    }

    public static Enrollment Create(StudentId studentId, Guid courseId, int creditHours)
    {
        return new Enrollment(Guid.NewGuid(), studentId, courseId, creditHours);
    }

    public static Enrollment Create(Guid id, StudentId studentId, Guid courseId, int creditHours, DateTime enrollmentDate)
    {
        var enrollment = new Enrollment(id, studentId, courseId, creditHours);
        enrollment.EnrollmentDate = enrollmentDate;
        return enrollment;
    }

    public void AssignGrade(Grade grade)
    {
        if (Status != EnrollmentStatus.Active)
            throw new InvalidOperationException("Cannot assign grade to inactive enrollment");

        Grade = grade ?? throw new ArgumentNullException(nameof(grade));
        UpdateTimestamp();
    }

    public void Complete()
    {
        if (Status != EnrollmentStatus.Active)
            throw new InvalidOperationException("Can only complete active enrollments");

        if (Grade == null)
            throw new InvalidOperationException("Cannot complete enrollment without a grade");

        Status = EnrollmentStatus.Completed;
        CompletionDate = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Withdraw()
    {
        if (Status == EnrollmentStatus.Completed)
            throw new InvalidOperationException("Cannot withdraw from completed enrollment");

        Status = EnrollmentStatus.Withdrawn;
        CompletionDate = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Reactivate()
    {
        if (Status == EnrollmentStatus.Completed)
            throw new InvalidOperationException("Cannot reactivate completed enrollment");

        Status = EnrollmentStatus.Active;
        CompletionDate = null;
        UpdateTimestamp();
    }

    private static int ValidateCreditHours(int creditHours)
    {
        if (creditHours < 1 || creditHours > 10)
            throw new ArgumentException("Credit hours must be between 1 and 10");

        return creditHours;
    }
}

public enum EnrollmentStatus
{
    Active = 1,
    Completed = 2,
    Withdrawn = 3
}