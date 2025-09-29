using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Courses;

public record DeleteCourseCommand : IRequest<ApiResponseDto<bool>>
{
    public Guid Id { get; init; }

    public DeleteCourseCommand(Guid id)
    {
        Id = id;
    }
}