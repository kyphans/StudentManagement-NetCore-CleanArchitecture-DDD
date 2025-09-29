using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Queries.Courses;

public record GetCoursesQuery : IRequest<ApiResponseDto<PagedResultDto<CourseSummaryDto>>>
{
    public string? SearchTerm { get; init; }
    public string? Department { get; init; }
    public bool? IsActive { get; init; }
    public bool? AvailableOnly { get; init; }
    public int? MinCreditHours { get; init; }
    public int? MaxCreditHours { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public static GetCoursesQuery FromDto(CourseFilterDto filter)
    {
        return new GetCoursesQuery
        {
            SearchTerm = filter.SearchTerm,
            Department = filter.Department,
            IsActive = filter.IsActive,
            AvailableOnly = filter.AvailableOnly,
            MinCreditHours = filter.MinCreditHours,
            MaxCreditHours = filter.MaxCreditHours,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
}