using FluentAssertions;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Domain.Tests.Entities;

public class GradeTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var letterGrade = "A";
        var gradePoints = 4.0m;
        var gradedBy = "Instructor Smith";
        var numericScore = 95.5m;
        var comments = "Excellent work";

        // Act
        var grade = Grade.Create(letterGrade, gradePoints, gradedBy, numericScore, comments);

        // Assert
        grade.Should().NotBeNull();
        grade.LetterGrade.Should().Be("A");
        grade.GradePoints.Should().Be(4.0m);
        grade.GradedBy.Should().Be(gradedBy);
        grade.NumericScore.Should().Be(95.5m);
        grade.Comments.Should().Be(comments);
        grade.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithoutOptionalParameters_ShouldSucceed()
    {
        // Act
        var grade = Grade.Create("B", 3.0m, "Instructor");

        // Assert
        grade.NumericScore.Should().BeNull();
        grade.Comments.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldSetGradedDateToNow()
    {
        // Arrange
        var beforeCreate = DateTime.UtcNow;

        // Act
        var grade = Grade.Create("A", 4.0m, "Instructor");

        // Assert
        grade.GradedDate.Should().BeOnOrAfter(beforeCreate);
        grade.GradedDate.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Theory]
    [InlineData("A+")]
    [InlineData("A")]
    [InlineData("A-")]
    [InlineData("B+")]
    [InlineData("B")]
    [InlineData("B-")]
    [InlineData("C+")]
    [InlineData("C")]
    [InlineData("C-")]
    [InlineData("D+")]
    [InlineData("D")]
    [InlineData("D-")]
    [InlineData("F")]
    [InlineData("I")]
    [InlineData("W")]
    public void Create_WithValidLetterGrade_ShouldSucceed(string letterGrade)
    {
        // Act
        var grade = Grade.Create(letterGrade, 3.0m, "Instructor");

        // Assert
        grade.LetterGrade.Should().Be(letterGrade.ToUpperInvariant());
    }

    [Theory]
    [InlineData("a", "A")]
    [InlineData("  B+  ", "B+")]
    [InlineData("c-", "C-")]
    public void Create_ShouldNormalizeLetterGrade(string input, string expected)
    {
        // Act
        var grade = Grade.Create(input, 3.0m, "Instructor");

        // Assert
        grade.LetterGrade.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyLetterGrade_ShouldThrowArgumentException(string invalidLetterGrade)
    {
        // Act
        Action action = () => Grade.Create(invalidLetterGrade, 3.0m, "Instructor");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Letter grade cannot be empty*");
    }

    [Theory]
    [InlineData("X")]
    [InlineData("AB")]
    [InlineData("E")]
    public void Create_WithInvalidLetterGrade_ShouldThrowArgumentException(string invalidLetterGrade)
    {
        // Act
        Action action = () => Grade.Create(invalidLetterGrade, 3.0m, "Instructor");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"*Invalid letter grade: {invalidLetterGrade}*");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(4.1)]
    [InlineData(5.0)]
    public void Create_WithInvalidGradePoints_ShouldThrowArgumentException(decimal invalidGradePoints)
    {
        // Act
        Action action = () => Grade.Create("A", invalidGradePoints, "Instructor");

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Grade points must be between 0.0 and 4.0*");
    }

    [Theory]
    [InlineData(3.567, 3.57)]
    [InlineData(2.123, 2.12)]
    [InlineData(1.999, 2.0)]
    public void Create_ShouldRoundGradePointsToTwoDecimals(decimal input, decimal expected)
    {
        // Act
        var grade = Grade.Create("A", input, "Instructor");

        // Assert
        grade.GradePoints.Should().Be(expected);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(150)]
    public void Create_WithInvalidNumericScore_ShouldThrowArgumentException(decimal invalidScore)
    {
        // Act
        Action action = () => Grade.Create("A", 4.0m, "Instructor", invalidScore);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Numeric score must be between 0 and 100*");
    }

    [Theory]
    [InlineData(95.567, 95.57)]
    [InlineData(88.123, 88.12)]
    [InlineData(75.999, 76.0)]
    public void Create_ShouldRoundNumericScoreToTwoDecimals(decimal input, decimal expected)
    {
        // Act
        var grade = Grade.Create("A", 4.0m, "Instructor", input);

        // Assert
        grade.NumericScore.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyGradedBy_ShouldThrowArgumentException(string invalidGradedBy)
    {
        // Act
        Action action = () => Grade.Create("A", 4.0m, invalidGradedBy);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("*GradedBy cannot be empty*");
    }

    [Theory]
    [InlineData(97, "A+", 4.0)]
    [InlineData(95, "A", 4.0)]
    [InlineData(90, "A-", 3.7)]
    [InlineData(87, "B+", 3.3)]
    [InlineData(85, "B", 3.0)]
    [InlineData(80, "B-", 2.7)]
    [InlineData(77, "C+", 2.3)]
    [InlineData(75, "C", 2.0)]
    [InlineData(70, "C-", 1.7)]
    [InlineData(67, "D+", 1.3)]
    [InlineData(65, "D", 1.0)]
    [InlineData(60, "D-", 0.7)]
    [InlineData(50, "F", 0.0)]
    public void CreateFromNumericScore_ShouldMapCorrectly(decimal score, string expectedLetter, decimal expectedPoints)
    {
        // Act
        var grade = Grade.CreateFromNumericScore(score, "Instructor");

        // Assert
        grade.LetterGrade.Should().Be(expectedLetter);
        grade.GradePoints.Should().Be(expectedPoints);
        grade.NumericScore.Should().Be(score);
    }

    [Fact]
    public void UpdateGrade_WithValidData_ShouldUpdate()
    {
        // Arrange
        var grade = Grade.Create("B", 3.0m, "Instructor");

        // Act
        grade.UpdateGrade("A", 4.0m, 95.0m, "Updated comments");

        // Assert
        grade.LetterGrade.Should().Be("A");
        grade.GradePoints.Should().Be(4.0m);
        grade.NumericScore.Should().Be(95.0m);
        grade.Comments.Should().Be("Updated comments");
    }

    [Fact]
    public void UpdateComments_ShouldUpdateComments()
    {
        // Arrange
        var grade = Grade.Create("A", 4.0m, "Instructor");

        // Act
        grade.UpdateComments("New comments");

        // Assert
        grade.Comments.Should().Be("New comments");
    }

    [Theory]
    [InlineData(2.0, true)]
    [InlineData(3.0, true)]
    [InlineData(4.0, true)]
    [InlineData(1.9, false)]
    [InlineData(0.0, false)]
    public void IsPassing_ShouldReturnCorrectValue(decimal gradePoints, bool expectedIsPassing)
    {
        // Arrange
        var grade = Grade.Create("A", gradePoints, "Instructor");

        // Act & Assert
        grade.IsPassing.Should().Be(expectedIsPassing);
    }

    [Theory]
    [InlineData(3.5, true)]
    [InlineData(4.0, true)]
    [InlineData(3.7, true)]
    [InlineData(3.49, false)]
    [InlineData(2.0, false)]
    public void IsHonorGrade_ShouldReturnCorrectValue(decimal gradePoints, bool expectedIsHonor)
    {
        // Arrange
        var grade = Grade.Create("A", gradePoints, "Instructor");

        // Act & Assert
        grade.IsHonorGrade.Should().Be(expectedIsHonor);
    }
}
