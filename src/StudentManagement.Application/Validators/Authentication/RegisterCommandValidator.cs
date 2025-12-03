using FluentValidation;
using StudentManagement.Application.Commands.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Application.Validators.Authentication
{
    /// <summary>
    /// Validator cho RegisterCommand
    /// </summary>
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            // Username validation
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username không được để trống")
                .Length(3, 50).WithMessage("Username phải từ 3-50 ký tự")
                .Matches(@"^[a-zA-Z0-9_.]+$").WithMessage("Username chỉ được chứa chữ, số, underscore và dấu chấm");

            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(255).WithMessage("Email không được quá 255 ký tự");

            // Password validation
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password không được để trống")
                .MinimumLength(8).WithMessage("Password phải ít nhất 8 ký tự")
                .Matches(@"[A-Z]").WithMessage("Password phải có ít nhất 1 chữ hoa")
                .Matches(@"[a-z]").WithMessage("Password phải có ít nhất 1 chữ thường")
                .Matches(@"[0-9]").WithMessage("Password phải có ít nhất 1 số")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password phải có ít nhất 1 ký tự đặc biệt");

            // Confirm password validation
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password không được để trống")
                .Equal(x => x.Password).WithMessage("Confirm password không khớp với password");

            // First name validation
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name không được để trống")
                .MaximumLength(50).WithMessage("First name không được quá 50 ký tự");

            // Last name validation
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name không được để trống")
                .MaximumLength(50).WithMessage("Last name không được quá 50 ký tự");

            // Role validation
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role không được để trống")
                .Must(BeValidRole).WithMessage("Role không hợp lệ");
        }

        private bool BeValidRole(string role)
        {
            var validRoles = new[] { "Admin", "Teacher", "Student", "Staff" };
            return validRoles.Contains(role);
        }
    }
}
