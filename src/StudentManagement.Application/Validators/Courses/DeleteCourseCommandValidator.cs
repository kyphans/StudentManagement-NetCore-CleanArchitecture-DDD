using FluentValidation;
using StudentManagement.Application.Commands.Courses;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Validators.Courses;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public DeleteCourseCommandValidator(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Course ID is required")
            .MustAsync(CourseExists).WithMessage("Course not found")
            .MustAsync(NotHaveActiveEnrollments).WithMessage("Cannot delete course with active enrollments");
    }

    private async Task<bool> CourseExists(Guid id, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        return course != null;
    }

    private async Task<bool> NotHaveActiveEnrollments(Guid id, CancellationToken cancellationToken)
    {
        var activeEnrollments = await _enrollmentRepository.GetByCourseIdAsync(id, cancellationToken);
        return !activeEnrollments.Any(e => e.IsActive);
    }
}