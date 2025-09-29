using FluentValidation;
using StudentManagement.Application.Commands.Courses;

namespace StudentManagement.Application.Validators.Courses;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Course ID is required");

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
}