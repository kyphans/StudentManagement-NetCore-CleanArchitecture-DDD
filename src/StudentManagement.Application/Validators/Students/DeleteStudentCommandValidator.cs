using FluentValidation;
using StudentManagement.Application.Commands.Students;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Validators.Students;

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    private readonly IStudentPersistencePort _studentRepository;
    private readonly IEnrollmentPersistencePort _enrollmentRepository;

    public DeleteStudentCommandValidator(IStudentPersistencePort studentRepository, IEnrollmentPersistencePort enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Student ID is required")
            .MustAsync(StudentExists).WithMessage("Student not found")
            .MustAsync(NotHaveActiveEnrollments).WithMessage("Cannot delete student with active enrollments");
    }

    private async Task<bool> StudentExists(Guid id, CancellationToken cancellationToken)
    {
        var studentId = StudentId.From(id);
        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
        return student != null;
    }

    private async Task<bool> NotHaveActiveEnrollments(Guid id, CancellationToken cancellationToken)
    {
        var studentId = StudentId.From(id);
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId, cancellationToken);
        var activeEnrollments = enrollments.Where(e => e.IsActive);
        return !activeEnrollments.Any();
    }
}