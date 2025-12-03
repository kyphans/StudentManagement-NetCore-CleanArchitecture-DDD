using FluentValidation;
using StudentManagement.Application.Commands.Authentication;

namespace StudentManagement.Application.Validators.Authentication;

/// <summary>
/// Validator cho LoginCommand
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username không được để trống");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password không được để trống");
    }
}