using FluentAssertions;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Tests.Entities;

public class EnrollmentTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var studentId = StudentId.New();
        var courseId = Guid.NewGuid();
        var creditHours = 3;

        // Act
        var enrollment = Enrollment.Create(studentId, courseId, creditHours);

        // Assert
        enrollment.Should().NotBeNull();
        enrollment.StudentId.Should().Be(studentId);
        enrollment.CourseId.Should().Be(courseId);
        enrollment.CreditHours.Should().Be(creditHours);
        enrollment.Status.Should().Be(EnrollmentStatus.Active);
        enrollment.IsActive.Should().BeTrue();
        enrollment.CompletionDate.Should().BeNull();
        enrollment.Grade.Should().BeNull();
        enrollment.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_ShouldSetEnrollmentDateToNow()
    {
        // Arrange
        var studentId = StudentId.New();
        var courseId = Guid.NewGuid();
        var beforeCreate = DateTime.UtcNow;

        // Act
        var enrollment = Enrollment.Create(studentId, courseId, 3);

        // Assert
        enrollment.EnrollmentDate.Should().BeOnOrAfter(beforeCreate);
        enrollment.EnrollmentDate.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Create_WithInvalidCreditHours_ShouldThrowArgumentException(int invalidCreditHours)
    {
        // Arrange
        var studentId = StudentId.New();
        var courseId = Guid.NewGuid();

        // Act
        Action action = () => Enrollment.Create(studentId, courseId, invalidCreditHours);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Credit hours must be between 1 and 10*");
    }

    [Fact]
    public void AssignGrade_WithValidGrade_ShouldSucceed()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        var grade = Grade.Create("A", 4.0m, "Instructor");

        // Act
        enrollment.AssignGrade(grade);

        // Assert
        enrollment.Grade.Should().Be(grade);
    }

    [Fact]
    public void AssignGrade_WithNullGrade_ShouldThrowArgumentNullException()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);

        // Act
        Action action = () => enrollment.AssignGrade(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AssignGrade_WhenEnrollmentNotActive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        enrollment.Withdraw();
        var grade = Grade.Create("A", 4.0m, "Instructor");

        // Act
        Action action = () => enrollment.AssignGrade(grade);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot assign grade to inactive enrollment*");
    }

    [Fact]
    public void Complete_WithGrade_ShouldSucceed()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        var grade = Grade.Create("A", 4.0m, "Instructor");
        enrollment.AssignGrade(grade);

        // Act
        enrollment.Complete();

        // Assert
        enrollment.Status.Should().Be(EnrollmentStatus.Completed);
        enrollment.IsCompleted.Should().BeTrue();
        enrollment.CompletionDate.Should().NotBeNull();
        enrollment.CompletionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Complete_WithoutGrade_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);

        // Act
        Action action = () => enrollment.Complete();

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot complete enrollment without a grade*");
    }

    [Fact]
    public void Complete_WhenNotActive_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        enrollment.Withdraw();

        // Act
        Action action = () => enrollment.Complete();

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Can only complete active enrollments*");
    }

    [Fact]
    public void Withdraw_WhenActive_ShouldSucceed()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);

        // Act
        enrollment.Withdraw();

        // Assert
        enrollment.Status.Should().Be(EnrollmentStatus.Withdrawn);
        enrollment.IsWithdrawn.Should().BeTrue();
        enrollment.CompletionDate.Should().NotBeNull();
        enrollment.CompletionDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Withdraw_WhenCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        var grade = Grade.Create("A", 4.0m, "Instructor");
        enrollment.AssignGrade(grade);
        enrollment.Complete();

        // Act
        Action action = () => enrollment.Withdraw();

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot withdraw from completed enrollment*");
    }

    [Fact]
    public void Reactivate_WhenWithdrawn_ShouldSucceed()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        enrollment.Withdraw();

        // Act
        enrollment.Reactivate();

        // Assert
        enrollment.Status.Should().Be(EnrollmentStatus.Active);
        enrollment.IsActive.Should().BeTrue();
        enrollment.CompletionDate.Should().BeNull();
    }

    [Fact]
    public void Reactivate_WhenCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        var grade = Grade.Create("A", 4.0m, "Instructor");
        enrollment.AssignGrade(grade);
        enrollment.Complete();

        // Act
        Action action = () => enrollment.Reactivate();

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot reactivate completed enrollment*");
    }

    [Fact]
    public void IsActive_ShouldReturnCorrectValue()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);

        // Act & Assert
        enrollment.IsActive.Should().BeTrue();

        enrollment.Withdraw();
        enrollment.IsActive.Should().BeFalse();

        enrollment.Reactivate();
        enrollment.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsCompleted_ShouldReturnCorrectValue()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);
        var grade = Grade.Create("A", 4.0m, "Instructor");

        // Act & Assert
        enrollment.IsCompleted.Should().BeFalse();

        enrollment.AssignGrade(grade);
        enrollment.Complete();
        enrollment.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void IsWithdrawn_ShouldReturnCorrectValue()
    {
        // Arrange
        var enrollment = Enrollment.Create(StudentId.New(), Guid.NewGuid(), 3);

        // Act & Assert
        enrollment.IsWithdrawn.Should().BeFalse();

        enrollment.Withdraw();
        enrollment.IsWithdrawn.Should().BeTrue();

        enrollment.Reactivate();
        enrollment.IsWithdrawn.Should().BeFalse();
    }
}
