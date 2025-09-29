using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Queries.Enrollments;

public record GetEnrollmentByIdQuery : IRequest<ApiResponseDto<EnrollmentWithDetailsDto>>
{
    public Guid Id { get; init; }

    public GetEnrollmentByIdQuery(Guid id)
    {
        Id = id;
    }
}