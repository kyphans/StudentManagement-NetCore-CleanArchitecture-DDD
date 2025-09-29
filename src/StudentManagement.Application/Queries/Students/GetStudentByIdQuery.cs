using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Queries.Students;

public record GetStudentByIdQuery : IRequest<ApiResponseDto<StudentDto>>
{
    public Guid Id { get; init; }

    public GetStudentByIdQuery(Guid id)
    {
        Id = id;
    }
}