using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Queries.Students;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, ApiResponseDto<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentByIdQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<ApiResponseDto<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var studentId = StudentId.From(request.Id);
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);

            if (student == null)
            {
                return ApiResponseDto<StudentDto>.ErrorResult("Student not found");
            }

            var studentDto = new StudentDto
            {
                Id = student.Id.Value,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email.Value,
                DateOfBirth = student.DateOfBirth,
                EnrollmentDate = student.EnrollmentDate,
                IsActive = student.IsActive,
                FullName = student.FullName,
                Age = student.Age,
                GPA = student.CalculateGPA().Value,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt
            };

            return ApiResponseDto<StudentDto>.SuccessResult(studentDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<StudentDto>.ErrorResult($"Failed to get student: {ex.Message}");
        }
    }
}