using FluentAssertions;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Tests.Entities;

public class CourseTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var code = new CourseCode("CS101");
        var name = "Introduction to Computer Science";
        var description = "Basic programming concepts";
        var creditHours = 3;
        var department = "Computer Science";
        var maxEnrollment = 30;

        // Act
        var course = Course.Create(code, name, description, creditHours, department, maxEnrollment);

        // Assert
        course.Should().NotBeNull();
        course.Code.Value.Should().Be("CS101");
        course.Name.Should().Be(name);
        course.Description.Should().Be(description);
        course.CreditHours.Should().Be(creditHours);
        course.Department.Should().Be(department);
        course.MaxEnrollment.Should().Be(maxEnrollment);
        course.IsActive.Should().BeTrue();
        course.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithDefaultMaxEnrollment_ShouldUseDefaultValue()
    {
        // Arrange
        var code = new CourseCode("CS101");

        // Act
        var course = Course.Create(code, "Computer Science", "Description", 3, "CS");

        // Assert
        course.MaxEnrollment.Should().Be(30);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var code = new CourseCode("CS101");

        // Act
        Action action = () => Course.Create(code, invalidName, "Description", 3, "CS");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Course name cannot be empty*");
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("ThisIsAVeryLongCourseNameThatExceedsOneHundredCharactersAndShouldThrowAnExceptionWhenUsedInCourseCreation")]
    public void Create_WithInvalidNameLength_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var code = new CourseCode("CS101");

        // Act
        Action action = () => Course.Create(code, invalidName, "Description", 3, "CS");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Course name must be between 3 and 100 characters*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Create_WithInvalidCreditHours_ShouldThrowArgumentException(int invalidCreditHours)
    {
        // Arrange
        var code = new CourseCode("CS101");

        // Act
        Action action = () => Course.Create(code, "Course Name", "Description", invalidCreditHours, "CS");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Credit hours must be between 1 and 10*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyDepartment_ShouldThrowArgumentException(string invalidDepartment)
    {
        // Arrange
        var code = new CourseCode("CS101");

        // Act
        Action action = () => Course.Create(code, "Course Name", "Description", 3, invalidDepartment);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Department cannot be empty*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(501)]
    [InlineData(-1)]
    public void Create_WithInvalidMaxEnrollment_ShouldThrowArgumentException(int invalidMaxEnrollment)
    {
        // Arrange
        var code = new CourseCode("CS101");

        // Act
        Action action = () => Course.Create(code, "Course Name", "Description", 3, "CS", invalidMaxEnrollment);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Max enrollment must be between 1 and 500*");
    }

    [Fact]
    public void UpdateCourseInfo_WithValidData_ShouldUpdate()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Old Name", "Old Desc", 3, "Old Dept");

        // Act
        course.UpdateCourseInfo("New Name", "New Description", 4, "New Department");

        // Assert
        course.Name.Should().Be("New Name");
        course.Description.Should().Be("New Description");
        course.CreditHours.Should().Be(4);
        course.Department.Should().Be("New Department");
    }

    [Fact]
    public void UpdateMaxEnrollment_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS", 30);

        // Act
        course.UpdateMaxEnrollment(50);

        // Assert
        course.MaxEnrollment.Should().Be(50);
    }

    [Fact]
    public void AddPrerequisite_WithValidCourseId_ShouldSucceed()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS201"), "Advanced CS", "Desc", 3, "CS");
        var prerequisiteId = Guid.NewGuid();

        // Act
        course.AddPrerequisite(prerequisiteId);

        // Assert
        course.Prerequisites.Should().Contain(prerequisiteId);
    }

    [Fact]
    public void AddPrerequisite_WithSameCourseId_ShouldThrowArgumentException()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS");

        // Act
        Action action = () => course.AddPrerequisite(course.Id);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Course cannot be a prerequisite for itself*");
    }

    [Fact]
    public void AddPrerequisite_WithDuplicatePrerequisite_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS201"), "Course", "Desc", 3, "CS");
        var prerequisiteId = Guid.NewGuid();
        course.AddPrerequisite(prerequisiteId);

        // Act
        Action action = () => course.AddPrerequisite(prerequisiteId);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Prerequisite already exists*");
    }

    [Fact]
    public void RemovePrerequisite_WithExistingPrerequisite_ShouldRemove()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS201"), "Course", "Desc", 3, "CS");
        var prerequisiteId = Guid.NewGuid();
        course.AddPrerequisite(prerequisiteId);

        // Act
        course.RemovePrerequisite(prerequisiteId);

        // Assert
        course.Prerequisites.Should().NotContain(prerequisiteId);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS");

        // Act
        course.Deactivate();

        // Assert
        course.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Reactivate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS");
        course.Deactivate();

        // Act
        course.Reactivate();

        // Assert
        course.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CanEnroll_WhenActiveAndNotFull_ShouldReturnTrue()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS", 30);

        // Act & Assert
        course.CanEnroll().Should().BeTrue();
    }

    [Fact]
    public void CanEnroll_WhenInactive_ShouldReturnFalse()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS", 30);
        course.Deactivate();

        // Act & Assert
        course.CanEnroll().Should().BeFalse();
    }

    [Fact]
    public void CanEnroll_WhenFull_ShouldReturnFalse()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS", 1);
        var studentId = StudentId.New();
        var enrollment = Enrollment.Create(studentId, course.Id, 3);
        course.AddEnrollment(enrollment);

        // Act & Assert
        course.CanEnroll().Should().BeFalse();
    }

    [Fact]
    public void AddEnrollment_WithValidEnrollment_ShouldSucceed()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS", 30);
        var studentId = StudentId.New();
        var enrollment = Enrollment.Create(studentId, course.Id, 3);

        // Act
        course.AddEnrollment(enrollment);

        // Assert
        course.Enrollments.Should().HaveCount(1);
        course.CurrentEnrollmentCount.Should().Be(1);
    }

    [Fact]
    public void AddEnrollment_WithDifferentCourseId_ShouldThrowArgumentException()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS");
        var studentId = StudentId.New();
        var otherCourseId = Guid.NewGuid();
        var enrollment = Enrollment.Create(studentId, otherCourseId, 3);

        // Act
        Action action = () => course.AddEnrollment(enrollment);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Enrollment must belong to this course*");
    }

    [Fact]
    public void AddEnrollment_WhenCourseFull_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS", 1);
        var student1Id = StudentId.New();
        var enrollment1 = Enrollment.Create(student1Id, course.Id, 3);
        course.AddEnrollment(enrollment1);

        var student2Id = StudentId.New();
        var enrollment2 = Enrollment.Create(student2Id, course.Id, 3);

        // Act
        Action action = () => course.AddEnrollment(enrollment2);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Course is full or inactive*");
    }

    [Fact]
    public void CurrentEnrollmentCount_ShouldOnlyCountActiveEnrollments()
    {
        // Arrange
        var course = Course.Create(new CourseCode("CS101"), "Course", "Desc", 3, "CS", 30);

        var student1Id = StudentId.New();
        var enrollment1 = Enrollment.Create(student1Id, course.Id, 3);
        course.AddEnrollment(enrollment1);

        var student2Id = StudentId.New();
        var enrollment2 = Enrollment.Create(student2Id, course.Id, 3);
        enrollment2.Withdraw();
        course.AddEnrollment(enrollment2);

        // Act & Assert
        course.CurrentEnrollmentCount.Should().Be(1);
    }
}
