using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Students;

public record DeleteStudentCommand : IRequest<ApiResponseDto<bool>>
{
    public Guid Id { get; init; }

    public DeleteStudentCommand(Guid id)
    {
        Id = id;
    }
}