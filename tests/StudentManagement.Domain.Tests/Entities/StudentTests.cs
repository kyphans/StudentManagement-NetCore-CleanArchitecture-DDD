using FluentAssertions;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Tests.Entities;

public class StudentTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new Email("john.doe@example.com");
        var dateOfBirth = new DateTime(2000, 1, 1);

        // Act
        var student = Student.Create(firstName, lastName, email, dateOfBirth);

        // Assert
        student.Should().NotBeNull();
        student.FirstName.Should().Be("John");
        student.LastName.Should().Be("Doe");
        student.Email.Value.Should().Be("john.doe@example.com");
        student.DateOfBirth.Should().Be(dateOfBirth.Date);
        student.IsActive.Should().BeTrue();
        student.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_ShouldSetEnrollmentDateToNow()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new Email("john.doe@example.com");
        var dateOfBirth = new DateTime(2000, 1, 1);
        var beforeCreate = DateTime.UtcNow;

        // Act
        var student = Student.Create(firstName, lastName, email, dateOfBirth);

        // Assert
        student.EnrollmentDate.Should().BeOnOrAfter(beforeCreate);
        student.EnrollmentDate.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyFirstName_ShouldThrowArgumentException(string invalidFirstName)
    {
        // Arrange
        var lastName = "Doe";
        var email = new Email("john.doe@example.com");
        var dateOfBirth = new DateTime(2000, 1, 1);

        // Act
        Action action = () => Student.Create(invalidFirstName, lastName, email, dateOfBirth);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be empty*");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("ThisIsAVeryLongNameThatExceedsFiftyCharactersInLength")]
    public void Create_WithInvalidFirstNameLength_ShouldThrowArgumentException(string invalidFirstName)
    {
        // Arrange
        var lastName = "Doe";
        var email = new Email("john.doe@example.com");
        var dateOfBirth = new DateTime(2000, 1, 1);

        // Act
        Action action = () => Student.Create(invalidFirstName, lastName, email, dateOfBirth);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Name must be between 2 and 50 characters*");
    }

    [Fact]
    public void Create_WithDateOfBirthTooOld_ShouldThrowArgumentException()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new Email("john.doe@example.com");
        var dateOfBirth = DateTime.UtcNow.AddYears(-121);

        // Act
        Action action = () => Student.Create(firstName, lastName, email, dateOfBirth);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid date of birth - student must be between 13 and 120 years old*");
    }

    [Fact]
    public void Create_WithDateOfBirthTooYoung_ShouldThrowArgumentException()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new Email("john.doe@example.com");
        var dateOfBirth = DateTime.UtcNow.AddYears(-12);

        // Act
        Action action = () => Student.Create(firstName, lastName, email, dateOfBirth);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid date of birth - student must be between 13 and 120 years old*");
    }

    [Fact]
    public void FullName_ShouldReturnCombinedName()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));

        // Act
        var fullName = student.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void Age_ShouldCalculateCorrectly()
    {
        // Arrange
        var dateOfBirth = DateTime.UtcNow.AddYears(-20).AddDays(-1);
        var student = Student.Create("John", "Doe", new Email("john@example.com"), dateOfBirth);

        // Act
        var age = student.Age;

        // Assert
        age.Should().Be(20);
    }

    [Fact]
    public void UpdatePersonalInfo_WithValidData_ShouldUpdate()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));
        var newEmail = new Email("john.updated@example.com");

        // Act
        student.UpdatePersonalInfo("Jane", "Smith", newEmail);

        // Assert
        student.FirstName.Should().Be("Jane");
        student.LastName.Should().Be("Smith");
        student.Email.Value.Should().Be("john.updated@example.com");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));

        // Act
        student.Deactivate();

        // Assert
        student.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Reactivate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));
        student.Deactivate();

        // Act
        student.Reactivate();

        // Assert
        student.IsActive.Should().BeTrue();
    }

    [Fact]
    public void AddEnrollment_WithValidEnrollment_ShouldSucceed()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));
        var courseId = Guid.NewGuid();
        var enrollment = Enrollment.Create(student.Id, courseId, 3);

        // Act
        student.AddEnrollment(enrollment);

        // Assert
        student.Enrollments.Should().HaveCount(1);
        student.Enrollments.First().Should().Be(enrollment);
    }

    [Fact]
    public void AddEnrollment_WithDifferentStudentId_ShouldThrowArgumentException()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));
        var otherStudentId = StudentId.New();
        var courseId = Guid.NewGuid();
        var enrollment = Enrollment.Create(otherStudentId, courseId, 3);

        // Act
        Action action = () => student.AddEnrollment(enrollment);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Enrollment must belong to this student*");
    }

    [Fact]
    public void AddEnrollment_WithDuplicateActiveCourse_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));
        var courseId = Guid.NewGuid();
        var enrollment1 = Enrollment.Create(student.Id, courseId, 3);
        var enrollment2 = Enrollment.Create(student.Id, courseId, 3);
        student.AddEnrollment(enrollment1);

        // Act
        Action action = () => student.AddEnrollment(enrollment2);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Student is already enrolled in this course*");
    }

    [Fact]
    public void CalculateGPA_WithNoCompletedEnrollments_ShouldReturnZero()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));

        // Act
        var gpa = student.CalculateGPA();

        // Assert
        gpa.Value.Should().Be(0.0m);
    }

    [Fact]
    public void CalculateGPA_WithCompletedEnrollments_ShouldCalculateCorrectly()
    {
        // Arrange
        var student = Student.Create("John", "Doe", new Email("john@example.com"), new DateTime(2000, 1, 1));

        var courseId1 = Guid.NewGuid();
        var enrollment1 = Enrollment.Create(student.Id, courseId1, 3);
        var grade1 = Grade.Create("A", 4.0m, "Instructor1", 90.0m);
        enrollment1.AssignGrade(grade1);
        enrollment1.Complete();
        student.AddEnrollment(enrollment1);

        var courseId2 = Guid.NewGuid();
        var enrollment2 = Enrollment.Create(student.Id, courseId2, 4);
        var grade2 = Grade.Create("B", 3.0m, "Instructor2", 80.0m);
        enrollment2.AssignGrade(grade2);
        enrollment2.Complete();
        student.AddEnrollment(enrollment2);

        // Act
        var gpa = student.CalculateGPA();

        // Assert
        // GPA = (3*4.0 + 4*3.0) / (3+4) = (12 + 12) / 7 = 24/7 = 3.43
        gpa.Value.Should().BeApproximately(3.43m, 0.01m);
    }
}
