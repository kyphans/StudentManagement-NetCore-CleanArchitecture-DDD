using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Students;

public record CreateStudentCommand : IRequest<ApiResponseDto<StudentDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }

    public static CreateStudentCommand FromDto(CreateStudentDto dto)
    {
        return new CreateStudentCommand
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth
        };
    }
}