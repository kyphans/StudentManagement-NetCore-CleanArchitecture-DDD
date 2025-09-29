using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;

namespace StudentManagement.Application.Queries.Courses;

public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, ApiResponseDto<PagedResultDto<CourseSummaryDto>>>
{
    private readonly ICourseRepository _courseRepository;

    public GetCoursesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<ApiResponseDto<PagedResultDto<CourseSummaryDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all courses first, then filter and paginate in memory
            // In production, this should be done at database level for performance
            var allCourses = await _courseRepository.GetAllAsync(cancellationToken);
            
            // Apply filters
            var filteredCourses = allCourses.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filteredCourses = filteredCourses.Where(c => c.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                           c.Code.Value.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (!string.IsNullOrWhiteSpace(request.Department))
            {
                filteredCourses = filteredCourses.Where(c => c.Department.Equals(request.Department, StringComparison.OrdinalIgnoreCase));
            }
            
            if (request.IsActive.HasValue)
            {
                filteredCourses = filteredCourses.Where(c => c.IsActive == request.IsActive.Value);
            }
            
            if (request.AvailableOnly == true)
            {
                filteredCourses = filteredCourses.Where(c => c.CurrentEnrollmentCount < c.MaxEnrollment && c.IsActive);
            }
            
            if (request.MinCreditHours.HasValue)
            {
                filteredCourses = filteredCourses.Where(c => c.CreditHours >= request.MinCreditHours.Value);
            }
            
            if (request.MaxCreditHours.HasValue)
            {
                filteredCourses = filteredCourses.Where(c => c.CreditHours <= request.MaxCreditHours.Value);
            }
            
            var totalCount = filteredCourses.Count();
            var courses = filteredCourses
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var courseDtos = courses.Select(course => new CourseSummaryDto
            {
                Id = course.Id,
                Code = course.Code.Value,
                Name = course.Name,
                CreditHours = course.CreditHours,
                Department = course.Department,
                IsActive = course.IsActive,
                CurrentEnrollmentCount = course.CurrentEnrollmentCount,
                MaxEnrollment = course.MaxEnrollment,
                CanEnroll = course.CurrentEnrollmentCount < course.MaxEnrollment && course.IsActive
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var result = new PagedResultDto<CourseSummaryDto>
            {
                Items = courseDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.PageNumber < totalPages,
                HasPreviousPage = request.PageNumber > 1
            };

            return ApiResponseDto<PagedResultDto<CourseSummaryDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<PagedResultDto<CourseSummaryDto>>.ErrorResult($"Failed to get courses: {ex.Message}");
        }
    }
}