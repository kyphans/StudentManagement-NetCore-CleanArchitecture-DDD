using AutoMapper;
using MediatR;
using StudentManagement.Application.Commands.Students;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Ports;
using StudentManagement.Application.Queries.Students;

namespace StudentManagement.Adapters.WebApi.ApplicationServices;

/// <summary>
/// Application service implementing Student Management Port (Primary Adapter).
/// Delegates to CQRS handlers via MediatR.
/// </summary>
public class StudentApplicationService : IStudentManagementPort
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public StudentApplicationService(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto request, CancellationToken cancellationToken = default)
    {
        var command = CreateStudentCommand.FromDto(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            var errors = result.Errors != null && result.Errors.Any()
                ? string.Join(", ", result.Errors)
                : result.Message ?? "Failed to create student";
            throw new InvalidOperationException(errors);
        }

        return result.Data;
    }

    public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto request, CancellationToken cancellationToken = default)
    {
        var command = UpdateStudentCommand.FromDto(id, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            var errors = result.Errors != null && result.Errors.Any()
                ? string.Join(", ", result.Errors)
                : result.Message ?? "Failed to update student";
            throw new InvalidOperationException(errors);
        }

        return result.Data;
    }

    public async Task DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteStudentCommand(id);
        await _mediator.Send(command, cancellationToken);
    }

    public async Task<StudentDto?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Data;
    }

    public async Task<PagedResultDto<StudentSummaryDto>> GetStudentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetStudentsQuery.FromDto(new StudentFilterDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            IsActive = isActive
        });

        var result = await _mediator.Send(query, cancellationToken);
        return result.Data!;
    }

    public async Task<StudentWithEnrollmentsDto?> GetStudentWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Fetch student with basic info
        var student = await GetStudentByIdAsync(id, cancellationToken);
        if (student == null)
            return null;

        // Map to StudentWithEnrollmentsDto
        // Note: In a real implementation, you might want a dedicated query for this
        return _mapper.Map<StudentWithEnrollmentsDto>(student);
    }
}
