using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Enrollments;

public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, ApiResponseDto<EnrollmentDto>>
{
    private readonly IEnrollmentPersistencePort _enrollmentRepository;
    private readonly IStudentPersistencePort _studentRepository;
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IUnitOfWorkPort _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEnrollmentCommandHandler(
        IEnrollmentPersistencePort enrollmentRepository,
        IStudentPersistencePort studentRepository,
        ICoursePersistencePort courseRepository,
        IUnitOfWorkPort unitOfWork,
        IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

            var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);

            return ApiResponseDto<EnrollmentDto>.SuccessResult(enrollmentDto, "Enrollment created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<EnrollmentDto>.ErrorResult($"Failed to create enrollment: {ex.Message}");
        }
    }
}