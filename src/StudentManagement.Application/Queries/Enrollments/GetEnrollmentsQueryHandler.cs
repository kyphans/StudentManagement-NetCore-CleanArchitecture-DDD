using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;

namespace StudentManagement.Application.Queries.Enrollments;

public class GetEnrollmentsQueryHandler : IRequestHandler<GetEnrollmentsQuery, ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetEnrollmentsQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>> Handle(GetEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all enrollments first, then filter and paginate in memory
            // In production, this should be done at database level for performance
            var allEnrollments = await _enrollmentRepository.GetAllAsync(cancellationToken);
            
            // Apply filters
            var filteredEnrollments = allEnrollments.AsQueryable();
            
            if (request.StudentId.HasValue)
            {
                filteredEnrollments = filteredEnrollments.Where(e => e.StudentId.Value == request.StudentId.Value);
            }
            
            if (request.CourseId.HasValue)
            {
                filteredEnrollments = filteredEnrollments.Where(e => e.CourseId == request.CourseId.Value);
            }
            
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                filteredEnrollments = filteredEnrollments.Where(e => e.Status.ToString().Equals(request.Status, StringComparison.OrdinalIgnoreCase));
            }
            
            if (request.EnrollmentDateFrom.HasValue)
            {
                filteredEnrollments = filteredEnrollments.Where(e => e.EnrollmentDate >= request.EnrollmentDateFrom.Value);
            }
            
            if (request.EnrollmentDateTo.HasValue)
            {
                filteredEnrollments = filteredEnrollments.Where(e => e.EnrollmentDate <= request.EnrollmentDateTo.Value);
            }
            
            if (request.CompletionDateFrom.HasValue)
            {
                filteredEnrollments = filteredEnrollments.Where(e => e.CompletionDate >= request.CompletionDateFrom.Value);
            }
            
            if (request.CompletionDateTo.HasValue)
            {
                filteredEnrollments = filteredEnrollments.Where(e => e.CompletionDate <= request.CompletionDateTo.Value);
            }
            
            var totalCount = filteredEnrollments.Count();
            var enrollments = filteredEnrollments
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var enrollmentDtos = enrollments.Select(enrollment => new EnrollmentSummaryDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId.Value,
                StudentName = "Student", // Would need to join with student data
                CourseId = enrollment.CourseId,
                CourseCode = "Course", // Would need to join with course data
                CourseName = "Course Name", // Would need to join with course data
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status.ToString(),
                CreditHours = enrollment.CreditHours,
                FinalGrade = enrollment.Grade?.LetterGrade,
                GradePoints = enrollment.Grade?.GradePoints
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var result = new PagedResultDto<EnrollmentSummaryDto>
            {
                Items = enrollmentDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.PageNumber < totalPages,
                HasPreviousPage = request.PageNumber > 1
            };

            return ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>.ErrorResult($"Failed to get enrollments: {ex.Message}");
        }
    }
}