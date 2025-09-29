using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Queries.Courses;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, ApiResponseDto<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<ApiResponseDto<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var courseId = request.Id;
            var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);

            if (course == null)
            {
                return ApiResponseDto<CourseDto>.ErrorResult("Course not found");
            }

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Code = course.Code.Value,
                Name = course.Name,
                Description = course.Description,
                CreditHours = course.CreditHours,
                Department = course.Department,
                IsActive = course.IsActive,
                MaxEnrollment = course.MaxEnrollment,
                CurrentEnrollmentCount = course.CurrentEnrollmentCount,
                Prerequisites = course.Prerequisites.ToList(),
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            };

            return ApiResponseDto<CourseDto>.SuccessResult(courseDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CourseDto>.ErrorResult($"Failed to get course: {ex.Message}");
        }
    }
}