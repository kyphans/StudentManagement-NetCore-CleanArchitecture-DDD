using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
/// Command để đăng nhập
/// </summary>
public record LoginCommand : IRequest<ApiResponseDto<AuthenticationResponseDto>>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; } // IP address của client

    public static LoginCommand FromDto(LoginRequestDto dto, string? ipAddress = null)
    {
        return new LoginCommand
        {
            Username = dto.Username,
            Password = dto.Password,
            IpAddress = ipAddress
        };
    }
}