using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Entities;

public class Student : BaseEntity<StudentId>
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - 
                     (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

    protected Student() { } // For EF Core

    private Student(StudentId id, string firstName, string lastName, Email email, 
                   DateTime dateOfBirth, DateTime enrollmentDate) : base(id)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        Email = email;
        DateOfBirth = ValidateDateOfBirth(dateOfBirth);
        EnrollmentDate = enrollmentDate;
    }

    public static Student Create(string firstName, string lastName, Email email, DateTime dateOfBirth)
    {
        return new Student(StudentId.New(), firstName, lastName, email, dateOfBirth, DateTime.UtcNow);
    }

    public static Student Create(StudentId id, string firstName, string lastName, Email email, 
                               DateTime dateOfBirth, DateTime enrollmentDate)
    {
        return new Student(id, firstName, lastName, email, dateOfBirth, enrollmentDate);
    }

    public void UpdatePersonalInfo(string firstName, string lastName, Email email)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        Email = email;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Reactivate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    public void AddEnrollment(Enrollment enrollment)
    {
        if (enrollment.StudentId != Id)
            throw new ArgumentException("Enrollment must belong to this student");

        if (_enrollments.Any(e => e.CourseId == enrollment.CourseId && e.IsActive))
            throw new InvalidOperationException("Student is already enrolled in this course");

        _enrollments.Add(enrollment);
        UpdateTimestamp();
    }

    public GPA CalculateGPA()
    {
        var completedEnrollments = _enrollments
            .Where(e => e.Grade != null && e.IsCompleted)
            .ToList();

        if (!completedEnrollments.Any())
            return new GPA(0.0m);

        var totalPoints = completedEnrollments.Sum(e => e.Grade!.GradePoints * e.CreditHours);
        var totalCredits = completedEnrollments.Sum(e => e.CreditHours);

        return new GPA(totalCredits > 0 ? totalPoints / totalCredits : 0.0m);
    }

    private static string ValidateName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", paramName);

        var trimmed = name.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 50)
            throw new ArgumentException("Name must be between 2 and 50 characters", paramName);

        return trimmed;
    }

    private static DateTime ValidateDateOfBirth(DateTime dateOfBirth)
    {
        var minAge = DateTime.UtcNow.AddYears(-120);
        var maxAge = DateTime.UtcNow.AddYears(-13);

        if (dateOfBirth < minAge || dateOfBirth > maxAge)
            throw new ArgumentException("Invalid date of birth - student must be between 13 and 120 years old");

        return dateOfBirth.Date;
    }
}