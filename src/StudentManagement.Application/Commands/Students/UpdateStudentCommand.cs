using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Students;

public record UpdateStudentCommand : IRequest<ApiResponseDto<StudentDto>>
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public bool IsActive { get; init; }

    public static UpdateStudentCommand FromDto(Guid id, UpdateStudentDto dto)
    {
        return new UpdateStudentCommand
        {
            Id = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            IsActive = dto.IsActive
        };
    }
}