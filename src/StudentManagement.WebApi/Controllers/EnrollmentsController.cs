using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Commands.Enrollments;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Queries.Enrollments;

namespace StudentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all enrollments with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>>> GetEnrollments(
        [FromQuery] EnrollmentFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = GetEnrollmentsQuery.FromDto(filter);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Get a specific enrollment by ID with details
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<EnrollmentWithDetailsDto>>> GetEnrollment(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetEnrollmentByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new enrollment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<EnrollmentDto>>> CreateEnrollment(
        [FromBody] CreateEnrollmentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = CreateEnrollmentCommand.FromDto(dto);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return CreatedAtAction(
            nameof(GetEnrollment), 
            new { id = result.Data!.Id }, 
            result);
    }

    /// <summary>
    /// Assign a grade to an enrollment
    /// </summary>
    [HttpPost("{id:guid}/grade")]
    public async Task<ActionResult<ApiResponseDto<EnrollmentDto>>> AssignGrade(
        Guid id,
        [FromBody] AssignGradeDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = AssignGradeCommand.FromDto(id, dto);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }
}