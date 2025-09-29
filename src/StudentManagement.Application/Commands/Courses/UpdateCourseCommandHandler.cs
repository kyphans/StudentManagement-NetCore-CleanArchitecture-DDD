using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Courses;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ApiResponseDto<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<CourseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (course == null)
            {
                return ApiResponseDto<CourseDto>.ErrorResult("Course not found");
            }

            course.UpdateCourseInfo(
                request.Name,
                request.Description,
                request.CreditHours,
                request.Department
            );
            
            course.UpdateMaxEnrollment(request.MaxEnrollment);

            _courseRepository.Update(course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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

            return ApiResponseDto<CourseDto>.SuccessResult(courseDto, "Course updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CourseDto>.ErrorResult($"Failed to update course: {ex.Message}");
        }
    }
}