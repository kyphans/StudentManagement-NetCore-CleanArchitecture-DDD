using FluentValidation;
using StudentManagement.Application.Commands.Students;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Validators.Students;

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    private readonly IStudentPersistencePort _studentRepository;

    public UpdateStudentCommandValidator(IStudentPersistencePort studentRepository)
    {
        _studentRepository = studentRepository;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Student ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(1, 50).WithMessage("First name must be between 1 and 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmailForUpdate).WithMessage("Email already exists for another student");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeValidAge).WithMessage("Student must be at least 16 years old and not older than 100 years");
    }

    private async Task<bool> BeUniqueEmailForUpdate(UpdateStudentCommand command, string email, CancellationToken cancellationToken)
    {
        try
        {
            var emailValue = new Email(email);
            var existingStudent = await _studentRepository.GetByEmailAsync(emailValue, cancellationToken);
            return existingStudent == null || existingStudent.Id.Value == command.Id;
        }
        catch
        {
            return false;
        }
    }

    private bool BeValidAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;

        return age >= 16 && age <= 100;
    }
}