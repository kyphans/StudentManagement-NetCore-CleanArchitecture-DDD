using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Enrollments;

public record CreateEnrollmentCommand : IRequest<ApiResponseDto<EnrollmentDto>>
{
    public Guid StudentId { get; init; }
    public Guid CourseId { get; init; }
    public int CreditHours { get; init; }

    public static CreateEnrollmentCommand FromDto(CreateEnrollmentDto dto)
    {
        return new CreateEnrollmentCommand
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            CreditHours = dto.CreditHours
        };
    }
}