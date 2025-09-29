using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Courses;

public record UpdateCourseCommand : IRequest<ApiResponseDto<CourseDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string Department { get; init; } = string.Empty;
    public int MaxEnrollment { get; init; }

    public static UpdateCourseCommand FromDto(Guid id, UpdateCourseDto dto)
    {
        return new UpdateCourseCommand
        {
            Id = id,
            Name = dto.Name,
            Description = dto.Description,
            CreditHours = dto.CreditHours,
            Department = dto.Department,
            MaxEnrollment = dto.MaxEnrollment
        };
    }
}