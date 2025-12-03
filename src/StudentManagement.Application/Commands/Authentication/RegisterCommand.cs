using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
///     Command để đăng ký user mới
/// </summary>
public record RegisterCommand : IRequest<ApiResponseDto<UserDto>>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = "Student";

    public static RegisterCommand FromDto(RegisterRequestDto dto)
    {
        return new RegisterCommand
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                ConfirmPassword = dto.ConfirmPassword,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = dto.Role
            }
            ;
    }
}