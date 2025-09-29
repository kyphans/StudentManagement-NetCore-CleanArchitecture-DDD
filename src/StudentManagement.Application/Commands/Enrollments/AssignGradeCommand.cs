using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Enrollments;

public record AssignGradeCommand : IRequest<ApiResponseDto<EnrollmentDto>>
{
    public Guid EnrollmentId { get; init; }
    public string LetterGrade { get; init; } = string.Empty;
    public decimal GradePoints { get; init; }
    public decimal? NumericScore { get; init; }
    public string? Comments { get; init; }
    public string GradedBy { get; init; } = string.Empty;

    public static AssignGradeCommand FromDto(Guid enrollmentId, AssignGradeDto dto)
    {
        return new AssignGradeCommand
        {
            EnrollmentId = enrollmentId,
            LetterGrade = dto.LetterGrade,
            GradePoints = dto.GradePoints,
            NumericScore = dto.NumericScore,
            Comments = dto.Comments,
            GradedBy = dto.GradedBy
        };
    }
}