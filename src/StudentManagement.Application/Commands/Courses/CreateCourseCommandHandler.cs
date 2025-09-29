using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Courses;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ApiResponseDto<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<CourseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var courseCode = new CourseCode(request.Code);

            var course = Course.Create(
                courseCode,
                request.Name,
                request.Description,
                request.CreditHours,
                request.Department,
                request.MaxEnrollment
            );

            await _courseRepository.AddAsync(course, cancellationToken);
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

            return ApiResponseDto<CourseDto>.SuccessResult(courseDto, "Course created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CourseDto>.ErrorResult($"Failed to create course: {ex.Message}");
        }
    }
}