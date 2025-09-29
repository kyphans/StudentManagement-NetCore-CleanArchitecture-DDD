using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Queries.Enrollments;

public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, ApiResponseDto<EnrollmentWithDetailsDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    public GetEnrollmentByIdQueryHandler(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<ApiResponseDto<EnrollmentWithDetailsDto>> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollmentId = request.Id;
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);

            if (enrollment == null)
            {
                return ApiResponseDto<EnrollmentWithDetailsDto>.ErrorResult("Enrollment not found");
            }

            var student = await _studentRepository.GetByIdAsync(enrollment.StudentId, cancellationToken);
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);

            if (student == null || course == null)
            {
                return ApiResponseDto<EnrollmentWithDetailsDto>.ErrorResult("Student or Course not found");
            }

            var enrollmentDto = new EnrollmentWithDetailsDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId.Value,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate,
                CompletionDate = enrollment.CompletionDate,
                Status = enrollment.Status.ToString(),
                CreditHours = enrollment.CreditHours,
                Grade = enrollment.Grade != null ? new GradeDto
                {
                    Id = enrollment.Grade.Id,
                    LetterGrade = enrollment.Grade.LetterGrade,
                    GradePoints = enrollment.Grade.GradePoints,
                    NumericScore = enrollment.Grade.NumericScore,
                    Comments = enrollment.Grade.Comments,
                    GradedDate = enrollment.Grade.GradedDate,
                    GradedBy = enrollment.Grade.GradedBy,
                    IsPassing = enrollment.Grade.IsPassing,
                    IsHonorGrade = enrollment.Grade.IsHonorGrade,
                    CreatedAt = enrollment.Grade.CreatedAt,
                    UpdatedAt = enrollment.Grade.UpdatedAt
                } : null,
                CreatedAt = enrollment.CreatedAt,
                UpdatedAt = enrollment.UpdatedAt,
                Student = new StudentSummaryDto
                {
                    Id = student.Id.Value,
                    FullName = $"{student.FirstName} {student.LastName}",
                    Email = student.Email.Value,
                    IsActive = student.IsActive,
                    GPA = student.CalculateGPA().Value,
                    TotalEnrollments = student.Enrollments.Count
                },
                Course = new CourseSummaryDto
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
                }
            };

            return ApiResponseDto<EnrollmentWithDetailsDto>.SuccessResult(enrollmentDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<EnrollmentWithDetailsDto>.ErrorResult($"Failed to get enrollment: {ex.Message}");
        }
    }
}