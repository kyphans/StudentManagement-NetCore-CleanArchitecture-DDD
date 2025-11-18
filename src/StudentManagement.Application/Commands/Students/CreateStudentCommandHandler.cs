using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Students;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentPersistencePort _studentRepository;
    private readonly IUnitOfWorkPort _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStudentCommandHandler(IStudentPersistencePort studentRepository, IUnitOfWorkPort unitOfWork, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

            var studentDto = _mapper.Map<StudentDto>(student);

            return ApiResponseDto<StudentDto>.SuccessResult(studentDto, "Student created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<StudentDto>.ErrorResult($"Failed to create student: {ex.Message}");
        }
    }
}