using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Queries.Courses;

public record GetCourseByIdQuery : IRequest<ApiResponseDto<CourseDto>>
{
    public Guid Id { get; init; }

    public GetCourseByIdQuery(Guid id)
    {
        Id = id;
    }
}