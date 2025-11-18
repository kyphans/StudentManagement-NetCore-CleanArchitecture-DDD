using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Enrollments;

public class AssignGradeCommandHandler : IRequestHandler<AssignGradeCommand, ApiResponseDto<EnrollmentDto>>
{
    private readonly IEnrollmentPersistencePort _enrollmentRepository;
    private readonly IUnitOfWorkPort _unitOfWork;
    private readonly IMapper _mapper;

    public AssignGradeCommandHandler(IEnrollmentPersistencePort enrollmentRepository, IUnitOfWorkPort unitOfWork, IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

            var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);

            return ApiResponseDto<EnrollmentDto>.SuccessResult(enrollmentDto, "Grade assigned successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<EnrollmentDto>.ErrorResult($"Failed to assign grade: {ex.Message}");
        }
    }
}