using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Courses;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, ApiResponseDto<bool>>
{
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IUnitOfWorkPort _unitOfWork;

    public DeleteCourseCommandHandler(ICoursePersistencePort courseRepository, IUnitOfWorkPort unitOfWork)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<bool>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (course == null)
            {
                return ApiResponseDto<bool>.ErrorResult("Course not found");
            }

            _courseRepository.Remove(course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponseDto<bool>.SuccessResult(true, "Course deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.ErrorResult($"Failed to delete course: {ex.Message}");
        }
    }
}