using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Students;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<StudentDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var email = new Email(request.Email);

            var student = Student.Create(
                request.FirstName,
                request.LastName,
                email,
                request.DateOfBirth
            );

            await _studentRepository.AddAsync(student, cancellationToken);
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

            return ApiResponseDto<StudentDto>.SuccessResult(studentDto, "Student created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<StudentDto>.ErrorResult($"Failed to create student: {ex.Message}");
        }
    }
}