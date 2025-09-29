using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Enrollments;

public class AssignGradeCommandHandler : IRequestHandler<AssignGradeCommand, ApiResponseDto<EnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignGradeCommandHandler(IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<EnrollmentDto>> Handle(AssignGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            
            if (enrollment == null)
            {
                return ApiResponseDto<EnrollmentDto>.ErrorResult("Enrollment not found");
            }

            var grade = Grade.Create(
                request.LetterGrade,
                request.GradePoints,
                request.GradedBy,
                request.NumericScore,
                request.Comments
            );

            enrollment.AssignGrade(grade);

            _enrollmentRepository.Update(enrollment);
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

            return ApiResponseDto<EnrollmentDto>.SuccessResult(enrollmentDto, "Grade assigned successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<EnrollmentDto>.ErrorResult($"Failed to assign grade: {ex.Message}");
        }
    }
}