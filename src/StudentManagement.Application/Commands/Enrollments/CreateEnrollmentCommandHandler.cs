using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Enrollments;

public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, ApiResponseDto<EnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEnrollmentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<EnrollmentDto>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var studentId = StudentId.From(request.StudentId);

            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
            {
                return ApiResponseDto<EnrollmentDto>.ErrorResult("Student not found");
            }

            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
            {
                return ApiResponseDto<EnrollmentDto>.ErrorResult("Course not found");
            }

            var enrollment = Enrollment.Create(
                studentId,
                request.CourseId,
                request.CreditHours
            );

            await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var enrollmentDto = new EnrollmentDto
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
                UpdatedAt = enrollment.UpdatedAt
            };

            return ApiResponseDto<EnrollmentDto>.SuccessResult(enrollmentDto, "Enrollment created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<EnrollmentDto>.ErrorResult($"Failed to create enrollment: {ex.Message}");
        }
    }
}