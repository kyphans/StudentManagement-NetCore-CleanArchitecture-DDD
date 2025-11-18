using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Queries.Enrollments;

public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, ApiResponseDto<EnrollmentWithDetailsDto>>
{
    private readonly IEnrollmentPersistencePort _enrollmentRepository;
    private readonly IStudentPersistencePort _studentRepository;
    private readonly ICoursePersistencePort _courseRepository;
    private readonly IMapper _mapper;

    public GetEnrollmentByIdQueryHandler(
        IEnrollmentPersistencePort enrollmentRepository,
        IStudentPersistencePort studentRepository,
        ICoursePersistencePort courseRepository,
        IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
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

            var enrollmentDto = _mapper.Map<EnrollmentWithDetailsDto>(enrollment) with
            {
                Student = _mapper.Map<StudentSummaryDto>(student),
                Course = _mapper.Map<CourseSummaryDto>(course)
            };

            return ApiResponseDto<EnrollmentWithDetailsDto>.SuccessResult(enrollmentDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<EnrollmentWithDetailsDto>.ErrorResult($"Failed to get enrollment: {ex.Message}");
        }
    }
}