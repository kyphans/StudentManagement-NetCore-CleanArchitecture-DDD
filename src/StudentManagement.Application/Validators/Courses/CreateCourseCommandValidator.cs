using FluentValidation;
using StudentManagement.Application.Commands.Courses;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Validators.Courses;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseCommandValidator(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Course code is required")
            .Length(3, 10).WithMessage("Course code must be between 3 and 10 characters")
            .Matches("^[A-Z]{2,4}[0-9]{3,4}$").WithMessage("Course code must be in format like 'CS101' or 'MATH1001'")
            .MustAsync(BeUniqueCourseCode).WithMessage("Course code already exists");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Course name is required")
            .Length(1, 100).WithMessage("Course name must be between 1 and 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Course description is required")
            .Length(10, 500).WithMessage("Course description must be between 10 and 500 characters");

        RuleFor(x => x.CreditHours)
            .GreaterThan(0).WithMessage("Credit hours must be greater than 0")
            .LessThanOrEqualTo(6).WithMessage("Credit hours cannot exceed 6");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required")
            .Length(2, 50).WithMessage("Department must be between 2 and 50 characters");

        RuleFor(x => x.MaxEnrollment)
            .GreaterThan(0).WithMessage("Max enrollment must be greater than 0")
            .LessThanOrEqualTo(300).WithMessage("Max enrollment cannot exceed 300");
    }

    private async Task<bool> BeUniqueCourseCode(string code, CancellationToken cancellationToken)
    {
        try
        {
            var courseCode = new CourseCode(code);
            var existingCourse = await _courseRepository.GetByCourseCodeAsync(courseCode, cancellationToken);
            return existingCourse == null;
        }
        catch
        {
            return false;
        }
    }
}