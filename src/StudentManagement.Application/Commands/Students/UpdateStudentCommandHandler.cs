using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Students;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var studentId = StudentId.From(request.Id);
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            
            if (student == null)
            {
                return ApiResponseDto<StudentDto>.ErrorResult("Student not found");
            }

            var email = new Email(request.Email);
            
            student.UpdatePersonalInfo(
                request.FirstName,
                request.LastName,
                email
            );

            if (!request.IsActive && student.IsActive)
            {
                student.Deactivate();
            }
            else if (request.IsActive && !student.IsActive)
            {
                student.Reactivate();
            }

            _studentRepository.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var gpa = student.CalculateGPA();
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
                GPA = gpa.Value,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt
            };

            return ApiResponseDto<StudentDto>.SuccessResult(studentDto, "Student updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<StudentDto>.ErrorResult($"Failed to update student: {ex.Message}");
        }
    }
}