using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Courses;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ApiResponseDto<CourseDto>>
{
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IUnitOfWorkPort _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCourseCommandHandler(ICoursePersistencePort courseRepository, IUnitOfWorkPort unitOfWork, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

            var courseDto = _mapper.Map<CourseDto>(course);

            return ApiResponseDto<CourseDto>.SuccessResult(courseDto, "Course updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CourseDto>.ErrorResult($"Failed to update course: {ex.Message}");
        }
    }
}