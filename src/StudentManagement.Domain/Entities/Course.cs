using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Entities;

public class Course : BaseEntity<Guid>
{
    public CourseCode Code { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int CreditHours { get; private set; }
    public string Department { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private readonly List<Guid> _prerequisites = new();
    public IReadOnlyCollection<Guid> Prerequisites => _prerequisites.AsReadOnly();

    public int CurrentEnrollmentCount => _enrollments.Count(e => e.IsActive);
    public int MaxEnrollment { get; private set; } = 30;

    protected Course() { } // For EF Core

    private Course(Guid id, CourseCode code, string name, string description, 
                  int creditHours, string department, int maxEnrollment) : base(id)
    {
        Code = code;
        Name = ValidateName(name);
        Description = description.Trim();
        CreditHours = ValidateCreditHours(creditHours);
        Department = ValidateDepartment(department);
        MaxEnrollment = ValidateMaxEnrollment(maxEnrollment);
    }

    public static Course Create(CourseCode code, string name, string description, 
                              int creditHours, string department, int maxEnrollment = 30)
    {
        return new Course(Guid.NewGuid(), code, name, description, creditHours, department, maxEnrollment);
    }

    public static Course Create(Guid id, CourseCode code, string name, string description, 
                              int creditHours, string department, int maxEnrollment = 30)
    {
        return new Course(id, code, name, description, creditHours, department, maxEnrollment);
    }

    public void UpdateCourseInfo(string name, string description, int creditHours, string department)
    {
        Name = ValidateName(name);
        Description = description.Trim();
        CreditHours = ValidateCreditHours(creditHours);
        Department = ValidateDepartment(department);
        UpdateTimestamp();
    }

    public void UpdateMaxEnrollment(int maxEnrollment)
    {
        MaxEnrollment = ValidateMaxEnrollment(maxEnrollment);
        UpdateTimestamp();
    }

    public void AddPrerequisite(Guid prerequisiteCourseId)
    {
        if (prerequisiteCourseId == Id)
            throw new ArgumentException("Course cannot be a prerequisite for itself");

        if (_prerequisites.Contains(prerequisiteCourseId))
            throw new InvalidOperationException("Prerequisite already exists");

        _prerequisites.Add(prerequisiteCourseId);
        UpdateTimestamp();
    }

    public void RemovePrerequisite(Guid prerequisiteCourseId)
    {
        if (_prerequisites.Remove(prerequisiteCourseId))
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

    public bool CanEnroll() => IsActive && CurrentEnrollmentCount < MaxEnrollment;

    public void AddEnrollment(Enrollment enrollment)
    {
        if (enrollment.CourseId != Id)
            throw new ArgumentException("Enrollment must belong to this course");

        if (!CanEnroll())
            throw new InvalidOperationException("Course is full or inactive");

        _enrollments.Add(enrollment);
        UpdateTimestamp();
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name cannot be empty");

        var trimmed = name.Trim();
        if (trimmed.Length < 3 || trimmed.Length > 100)
            throw new ArgumentException("Course name must be between 3 and 100 characters");

        return trimmed;
    }

    private static int ValidateCreditHours(int creditHours)
    {
        if (creditHours < 1 || creditHours > 10)
            throw new ArgumentException("Credit hours must be between 1 and 10");

        return creditHours;
    }

    private static string ValidateDepartment(string department)
    {
        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("Department cannot be empty");

        var trimmed = department.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 50)
            throw new ArgumentException("Department must be between 2 and 50 characters");

        return trimmed;
    }

    private static int ValidateMaxEnrollment(int maxEnrollment)
    {
        if (maxEnrollment < 1 || maxEnrollment > 500)
            throw new ArgumentException("Max enrollment must be between 1 and 500");

        return maxEnrollment;
    }
}