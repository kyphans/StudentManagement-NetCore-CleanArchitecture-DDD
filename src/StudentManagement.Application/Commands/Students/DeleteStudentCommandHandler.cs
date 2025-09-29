using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Students;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, ApiResponseDto<bool>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStudentCommandHandler(IStudentRepository studentRepository, IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<bool>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var studentId = StudentId.From(request.Id);
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            
            if (student == null)
            {
                return ApiResponseDto<bool>.ErrorResult("Student not found");
            }

            _studentRepository.Remove(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponseDto<bool>.SuccessResult(true, "Student deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.ErrorResult($"Failed to delete student: {ex.Message}");
        }
    }
}