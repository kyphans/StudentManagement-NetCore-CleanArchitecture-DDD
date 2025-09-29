using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Queries.Students;

public record GetStudentsQuery : IRequest<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public DateTime? EnrollmentDateFrom { get; init; }
    public DateTime? EnrollmentDateTo { get; init; }
    public decimal? MinGPA { get; init; }
    public decimal? MaxGPA { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public static GetStudentsQuery FromDto(StudentFilterDto filter)
    {
        return new GetStudentsQuery
        {
            SearchTerm = filter.SearchTerm,
            IsActive = filter.IsActive,
            EnrollmentDateFrom = filter.EnrollmentDateFrom,
            EnrollmentDateTo = filter.EnrollmentDateTo,
            MinGPA = filter.MinGPA,
            MaxGPA = filter.MaxGPA,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
}