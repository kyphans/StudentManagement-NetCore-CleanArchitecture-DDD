using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;

namespace StudentManagement.Application.Queries.Students;

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, ApiResponseDto<PagedResultDto<StudentSummaryDto>>>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentsQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<ApiResponseDto<PagedResultDto<StudentSummaryDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all students first, then filter and paginate in memory
            // In production, this should be done at database level for performance
            var allStudents = await _studentRepository.GetAllAsync(cancellationToken);
            
            // Apply filters
            var filteredStudents = allStudents.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filteredStudents = filteredStudents.Where(s => s.FirstName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                             s.LastName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                             s.Email.Value.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (request.IsActive.HasValue)
            {
                filteredStudents = filteredStudents.Where(s => s.IsActive == request.IsActive.Value);
            }
            
            if (request.EnrollmentDateFrom.HasValue)
            {
                filteredStudents = filteredStudents.Where(s => s.EnrollmentDate >= request.EnrollmentDateFrom.Value);
            }
            
            if (request.EnrollmentDateTo.HasValue)
            {
                filteredStudents = filteredStudents.Where(s => s.EnrollmentDate <= request.EnrollmentDateTo.Value);
            }
            
            if (request.MinGPA.HasValue)
            {
                filteredStudents = filteredStudents.Where(s => s.CalculateGPA().Value >= request.MinGPA.Value);
            }
            
            if (request.MaxGPA.HasValue)
            {
                filteredStudents = filteredStudents.Where(s => s.CalculateGPA().Value <= request.MaxGPA.Value);
            }
            
            var totalCount = filteredStudents.Count();
            var students = filteredStudents
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var studentDtos = students.Select(student => new StudentSummaryDto
            {
                Id = student.Id.Value,
                FullName = $"{student.FirstName} {student.LastName}",
                Email = student.Email.Value,
                IsActive = student.IsActive,
                GPA = student.CalculateGPA().Value,
                TotalEnrollments = student.Enrollments.Count
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var result = new PagedResultDto<StudentSummaryDto>
            {
                Items = studentDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.PageNumber < totalPages,
                HasPreviousPage = request.PageNumber > 1
            };

            return ApiResponseDto<PagedResultDto<StudentSummaryDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<PagedResultDto<StudentSummaryDto>>.ErrorResult($"Failed to get students: {ex.Message}");
        }
    }
}