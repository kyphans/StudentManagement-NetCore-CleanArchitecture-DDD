using FluentValidation;
using StudentManagement.Application.Commands.Students;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Validators.Students;

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public DeleteStudentCommandValidator(IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository)
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