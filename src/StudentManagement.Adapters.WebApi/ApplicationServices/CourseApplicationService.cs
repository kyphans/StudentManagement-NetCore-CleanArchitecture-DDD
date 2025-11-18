using AutoMapper;
using MediatR;
using StudentManagement.Application.Commands.Courses;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Ports;
using StudentManagement.Application.Queries.Courses;

namespace StudentManagement.Adapters.WebApi.ApplicationServices;

/// <summary>
/// Application service implementing Course Management Port (Primary Adapter).
/// Delegates to CQRS handlers via MediatR.
/// </summary>
public class CourseApplicationService : ICourseManagementPort
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CourseApplicationService(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto request, CancellationToken cancellationToken = default)
    {
        var command = CreateCourseCommand.FromDto(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            var errors = result.Errors != null && result.Errors.Any()
                ? string.Join(", ", result.Errors)
                : result.Message ?? "Failed to create course";
            throw new InvalidOperationException(errors);
        }

        return result.Data;
    }

    public async Task<CourseDto> UpdateCourseAsync(Guid id, UpdateCourseDto request, CancellationToken cancellationToken = default)
    {
        var command = UpdateCourseCommand.FromDto(id, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            var errors = result.Errors != null && result.Errors.Any()
                ? string.Join(", ", result.Errors)
                : result.Message ?? "Failed to update course";
            throw new InvalidOperationException(errors);
        }

        return result.Data;
    }

    public async Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteCourseCommand(id);
        await _mediator.Send(command, cancellationToken);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Data;
    }

    public async Task<PagedResultDto<CourseSummaryDto>> GetCoursesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? department = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = GetCoursesQuery.FromDto(new CourseFilterDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            Department = department,
            IsActive = isActive
        });

        var result = await _mediator.Send(query, cancellationToken);
        return result.Data!;
    }

    public async Task<CourseWithEnrollmentsDto?> GetCourseWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await GetCourseByIdAsync(id, cancellationToken);
        if (course == null)
            return null;

        return _mapper.Map<CourseWithEnrollmentsDto>(course);
    }

    public async Task<CourseWithPrerequisitesDto?> GetCourseWithPrerequisitesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await GetCourseByIdAsync(id, cancellationToken);
        if (course == null)
            return null;

        return _mapper.Map<CourseWithPrerequisitesDto>(course);
    }
}
