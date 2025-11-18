using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Courses;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ApiResponseDto<CourseDto>>
{
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IUnitOfWorkPort _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCourseCommandHandler(ICoursePersistencePort courseRepository, IUnitOfWorkPort unitOfWork, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

            var courseDto = _mapper.Map<CourseDto>(course);

            return ApiResponseDto<CourseDto>.SuccessResult(courseDto, "Course created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CourseDto>.ErrorResult($"Failed to create course: {ex.Message}");
        }
    }
}