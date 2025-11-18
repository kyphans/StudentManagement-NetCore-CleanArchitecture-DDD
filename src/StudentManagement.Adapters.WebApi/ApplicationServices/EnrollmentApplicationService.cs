using AutoMapper;
using MediatR;
using StudentManagement.Application.Commands.Enrollments;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Ports;
using StudentManagement.Application.Queries.Enrollments;

namespace StudentManagement.Adapters.WebApi.ApplicationServices;

/// <summary>
/// Application service implementing Enrollment Management Port (Primary Adapter).
/// Delegates to CQRS handlers via MediatR.
/// </summary>
public class EnrollmentApplicationService : IEnrollmentManagementPort
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public EnrollmentApplicationService(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto request, CancellationToken cancellationToken = default)
    {
        var command = CreateEnrollmentCommand.FromDto(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            var errors = result.Errors != null && result.Errors.Any()
                ? string.Join(", ", result.Errors)
                : result.Message ?? "Failed to create enrollment";
            throw new InvalidOperationException(errors);
        }

        return result.Data;
    }

    public async Task<EnrollmentDto> AssignGradeAsync(Guid enrollmentId, AssignGradeDto request, CancellationToken cancellationToken = default)
    {
        var command = AssignGradeCommand.FromDto(enrollmentId, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            var errors = result.Errors != null && result.Errors.Any()
                ? string.Join(", ", result.Errors)
                : result.Message ?? "Failed to assign grade";
            throw new InvalidOperationException(errors);
        }

        return result.Data;
    }

    public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetEnrollmentByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Data;
    }

    public async Task<PagedResultDto<EnrollmentSummaryDto>> GetEnrollmentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        Guid? studentId = null,
        Guid? courseId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetEnrollmentsQuery.FromDto(new EnrollmentFilterDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            StudentId = studentId,
            CourseId = courseId,
            Status = status
        });

        var result = await _mediator.Send(query, cancellationToken);
        return result.Data!;
    }

    public async Task<EnrollmentWithDetailsDto?> GetEnrollmentWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await GetEnrollmentByIdAsync(id, cancellationToken);
        if (enrollment == null)
            return null;

        return _mapper.Map<EnrollmentWithDetailsDto>(enrollment);
    }

    public async Task<IEnumerable<EnrollmentSummaryDto>> GetEnrollmentsByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var result = await GetEnrollmentsAsync(
            pageNumber: 1,
            pageSize: 1000,
            studentId: studentId,
            cancellationToken: cancellationToken);

        return result.Items;
    }

    public async Task<IEnumerable<EnrollmentSummaryDto>> GetEnrollmentsByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var result = await GetEnrollmentsAsync(
            pageNumber: 1,
            pageSize: 1000,
            courseId: courseId,
            cancellationToken: cancellationToken);

        return result.Items;
    }
}
