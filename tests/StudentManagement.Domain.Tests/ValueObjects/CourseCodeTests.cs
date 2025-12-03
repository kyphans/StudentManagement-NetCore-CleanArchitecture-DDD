using FluentAssertions;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Tests.ValueObjects;

public class CourseCodeTests
{
    [Theory]
    [InlineData("CS101")]
    [InlineData("MATH200")]
    [InlineData("ENG")]
    [InlineData("ABC1234567")]
    public void Create_WithValidCode_ShouldSucceed(string validCode)
    {
        // Act
        var courseCode = new CourseCode(validCode);

        // Assert
        courseCode.Value.Should().Be(validCode.ToUpperInvariant());
    }

    [Theory]
    [InlineData("cs101", "CS101")]
    [InlineData("  math200  ", "MATH200")]
    [InlineData("Eng", "ENG")]
    public void Create_ShouldNormalizeCode(string input, string expected)
    {
        // Act
        var courseCode = new CourseCode(input);

        // Assert
        courseCode.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyCode_ShouldThrowArgumentException(string invalidCode)
    {
        // Act
        Action action = () => new CourseCode(invalidCode);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Course code cannot be empty*");
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("12345678901")]
    public void Create_WithInvalidLength_ShouldThrowArgumentException(string invalidCode)
    {
        // Act
        Action action = () => new CourseCode(invalidCode);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Course code must be between 3 and 10 characters*");
    }

    [Theory]
    [InlineData("CS-101")]
    [InlineData("CS 101")]
    [InlineData("CS@101")]
    [InlineData("CS_101")]
    public void Create_WithInvalidCharacters_ShouldThrowArgumentException(string invalidCode)
    {
        // Act
        Action action = () => new CourseCode(invalidCode);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Course code can only contain letters and numbers*");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var courseCode = new CourseCode("CS101");

        // Act
        var result = courseCode.ToString();

        // Assert
        result.Should().Be("CS101");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var courseCode = new CourseCode("CS101");

        // Act
        string result = courseCode;

        // Assert
        result.Should().Be("CS101");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        string code = "CS101";

        // Act
        CourseCode courseCode = code;

        // Assert
        courseCode.Value.Should().Be("CS101");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var code1 = new CourseCode("CS101");
        var code2 = new CourseCode("cs101");

        // Act & Assert
        code1.Should().Be(code2);
        (code1 == code2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var code1 = new CourseCode("CS101");
        var code2 = new CourseCode("CS102");

        // Act & Assert
        code1.Should().NotBe(code2);
        (code1 != code2).Should().BeTrue();
    }
}
