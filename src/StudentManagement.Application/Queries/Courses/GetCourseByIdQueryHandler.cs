using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Queries.Courses;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, ApiResponseDto<CourseDto>>
{
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(ICoursePersistencePort courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
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

            var courseDto = _mapper.Map<CourseDto>(course);

            return ApiResponseDto<CourseDto>.SuccessResult(courseDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<CourseDto>.ErrorResult($"Failed to get course: {ex.Message}");
        }
    }
}