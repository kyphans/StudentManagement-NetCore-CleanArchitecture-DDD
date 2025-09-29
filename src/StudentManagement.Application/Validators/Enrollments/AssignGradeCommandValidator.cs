using FluentValidation;
using StudentManagement.Application.Commands.Enrollments;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;

namespace StudentManagement.Application.Validators.Enrollments;

public class AssignGradeCommandValidator : AbstractValidator<AssignGradeCommand>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly string[] _validGrades = { "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D+", "D", "D-", "F" };

    public AssignGradeCommandValidator(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;

        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required")
            .MustAsync(EnrollmentExists).WithMessage("Enrollment not found")
            .MustAsync(EnrollmentIsActive).WithMessage("Cannot assign grade to inactive enrollment");

        RuleFor(x => x.LetterGrade)
            .NotEmpty().WithMessage("Letter grade is required")
            .Must(BeValidLetterGrade).WithMessage("Invalid letter grade. Valid grades are: A+, A, A-, B+, B, B-, C+, C, C-, D+, D, D-, F");

        RuleFor(x => x.GradePoints)
            .InclusiveBetween(0.0m, 4.0m).WithMessage("Grade points must be between 0.0 and 4.0");

        RuleFor(x => x.NumericScore)
            .InclusiveBetween(0.0m, 100.0m).WithMessage("Numeric score must be between 0.0 and 100.0")
            .When(x => x.NumericScore.HasValue);

        RuleFor(x => x.GradedBy)
            .NotEmpty().WithMessage("Graded by is required")
            .Length(1, 100).WithMessage("Graded by must be between 1 and 100 characters");

        RuleFor(x => x.Comments)
            .Length(0, 500).WithMessage("Comments cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Comments));
    }

    private async Task<bool> EnrollmentExists(Guid enrollmentId, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        return enrollment != null;
    }

    private async Task<bool> EnrollmentIsActive(Guid enrollmentId, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        return enrollment?.Status == EnrollmentStatus.Active;
    }

    private bool BeValidLetterGrade(string letterGrade)
    {
        return _validGrades.Contains(letterGrade);
    }
}