using FluentAssertions;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Tests.ValueObjects;

public class GPATests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(2.5)]
    [InlineData(3.5)]
    [InlineData(4.0)]
    public void Create_WithValidValue_ShouldSucceed(decimal value)
    {
        // Act
        var gpa = new GPA(value);

        // Assert
        gpa.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(2.567, 2.57)]
    [InlineData(3.123, 3.12)]
    [InlineData(1.999, 2.0)]
    public void Create_ShouldRoundToTwoDecimals(decimal input, decimal expected)
    {
        // Act
        var gpa = new GPA(input);

        // Assert
        gpa.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1.0)]
    [InlineData(4.1)]
    [InlineData(5.0)]
    public void Create_WithInvalidValue_ShouldThrowArgumentException(decimal invalidValue)
    {
        // Act
        Action action = () => new GPA(invalidValue);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"*GPA must be between {GPA.MinValue} and {GPA.MaxValue}*");
    }

    [Theory]
    [InlineData(3.5, true)]
    [InlineData(3.7, true)]
    [InlineData(4.0, true)]
    [InlineData(3.49, false)]
    [InlineData(2.0, false)]
    public void IsHonorRoll_ShouldReturnCorrectValue(decimal value, bool expectedIsHonorRoll)
    {
        // Arrange
        var gpa = new GPA(value);

        // Act & Assert
        gpa.IsHonorRoll.Should().Be(expectedIsHonorRoll);
    }

    [Theory]
    [InlineData(2.0, true)]
    [InlineData(2.5, true)]
    [InlineData(4.0, true)]
    [InlineData(1.99, false)]
    [InlineData(0.0, false)]
    public void IsPassing_ShouldReturnCorrectValue(decimal value, bool expectedIsPassing)
    {
        // Arrange
        var gpa = new GPA(value);

        // Act & Assert
        gpa.IsPassing.Should().Be(expectedIsPassing);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedValue()
    {
        // Arrange
        var gpa = new GPA(3.5m);

        // Act
        var result = gpa.ToString();

        // Assert
        result.Should().Be("3.50");
    }

    [Fact]
    public void ImplicitConversion_ToDecimal_ShouldWork()
    {
        // Arrange
        var gpa = new GPA(3.5m);

        // Act
        decimal result = gpa;

        // Assert
        result.Should().Be(3.5m);
    }

    [Fact]
    public void ImplicitConversion_FromDecimal_ShouldWork()
    {
        // Arrange
        decimal value = 3.5m;

        // Act
        GPA gpa = value;

        // Assert
        gpa.Value.Should().Be(3.5m);
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var gpa1 = new GPA(3.5m);
        var gpa2 = new GPA(3.50m);

        // Act & Assert
        gpa1.Should().Be(gpa2);
        (gpa1 == gpa2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var gpa1 = new GPA(3.5m);
        var gpa2 = new GPA(3.6m);

        // Act & Assert
        gpa1.Should().NotBe(gpa2);
        (gpa1 != gpa2).Should().BeTrue();
    }
}
