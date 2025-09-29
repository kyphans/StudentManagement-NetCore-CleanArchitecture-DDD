using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Queries.Students;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, ApiResponseDto<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IMapper _mapper;

    public GetStudentByIdQueryHandler(IStudentRepository studentRepository, IMapper mapper)
    {
        _studentRepository = studentRepository;
        _mapper = mapper;
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

            var studentDto = _mapper.Map<StudentDto>(student);

            return ApiResponseDto<StudentDto>.SuccessResult(studentDto);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<StudentDto>.ErrorResult($"Failed to get student: {ex.Message}");
        }
    }
}