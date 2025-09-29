using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Courses;

public record CreateCourseCommand : IRequest<ApiResponseDto<CourseDto>>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public string Department { get; init; } = string.Empty;
    public int MaxEnrollment { get; init; } = 30;

    public static CreateCourseCommand FromDto(CreateCourseDto dto)
    {
        return new CreateCourseCommand
        {
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            CreditHours = dto.CreditHours,
            Department = dto.Department,
            MaxEnrollment = dto.MaxEnrollment
        };
    }
}