using FluentValidation;
using StudentManagement.Application.Commands.Enrollments;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Validators.Enrollments;

public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    private readonly IStudentPersistencePort _studentRepository;
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IEnrollmentPersistencePort _enrollmentRepository;

    public CreateEnrollmentCommandValidator(
        IStudentPersistencePort studentRepository,
        ICoursePersistencePort courseRepository,
        IEnrollmentPersistencePort enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required")
            .MustAsync(StudentExists).WithMessage("Student not found")
            .MustAsync(StudentIsActive).WithMessage("Student must be active to enroll");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required")
            .MustAsync(CourseExists).WithMessage("Course not found")
            .MustAsync(CourseIsActive).WithMessage("Course must be active for enrollment")
            .MustAsync(CourseHasCapacity).WithMessage("Course is at maximum capacity");

        RuleFor(x => x.CreditHours)
            .GreaterThan(0).WithMessage("Credit hours must be greater than 0")
            .LessThanOrEqualTo(6).WithMessage("Credit hours cannot exceed 6");

        RuleFor(x => x)
            .MustAsync(NotAlreadyEnrolled).WithMessage("Student is already enrolled in this course");
    }

    private async Task<bool> StudentExists(Guid studentId, CancellationToken cancellationToken)
    {
        var id = StudentId.From(studentId);
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        return student != null;
    }

    private async Task<bool> StudentIsActive(Guid studentId, CancellationToken cancellationToken)
    {
        var id = StudentId.From(studentId);
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken);
        return student?.IsActive == true;
    }

    private async Task<bool> CourseExists(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
        return course != null;
    }

    private async Task<bool> CourseIsActive(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
        return course?.IsActive == true;
    }

    private async Task<bool> CourseHasCapacity(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
        return course != null && course.CurrentEnrollmentCount < course.MaxEnrollment;
    }

    private async Task<bool> NotAlreadyEnrolled(CreateEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var studentId = StudentId.From(command.StudentId);
        
        var existingEnrollment = await _enrollmentRepository.GetActiveEnrollmentAsync(studentId, command.CourseId, cancellationToken);
        return existingEnrollment == null;
    }
}