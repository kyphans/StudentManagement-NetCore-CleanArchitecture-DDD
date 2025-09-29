using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Queries.Enrollments;

public record GetEnrollmentsQuery : IRequest<ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>>
{
    public Guid? StudentId { get; init; }
    public Guid? CourseId { get; init; }
    public string? Status { get; init; }
    public DateTime? EnrollmentDateFrom { get; init; }
    public DateTime? EnrollmentDateTo { get; init; }
    public DateTime? CompletionDateFrom { get; init; }
    public DateTime? CompletionDateTo { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public static GetEnrollmentsQuery FromDto(EnrollmentFilterDto filter)
    {
        return new GetEnrollmentsQuery
        {
            StudentId = filter.StudentId,
            CourseId = filter.CourseId,
            Status = filter.Status,
            EnrollmentDateFrom = filter.EnrollmentDateFrom,
            EnrollmentDateTo = filter.EnrollmentDateTo,
            CompletionDateFrom = filter.CompletionDateFrom,
            CompletionDateTo = filter.CompletionDateTo,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
}