using FluentValidation;
using StudentManagement.Application.Commands.Courses;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Validators.Courses;

public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IEnrollmentPersistencePort _enrollmentRepository;

    public DeleteCourseCommandValidator(ICoursePersistencePort courseRepository, IEnrollmentPersistencePort enrollmentRepository)
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