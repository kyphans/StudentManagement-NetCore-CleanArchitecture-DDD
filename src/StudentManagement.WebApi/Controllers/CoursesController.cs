using MediatR;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Commands.Courses;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Queries.Courses;

namespace StudentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all courses with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<CourseSummaryDto>>>> GetCourses(
        [FromQuery] CourseFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = GetCoursesQuery.FromDto(filter);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Get a specific course by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<CourseDto>>> GetCourse(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<CourseDto>>> CreateCourse(
        [FromBody] CreateCourseDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = CreateCourseCommand.FromDto(dto);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return CreatedAtAction(
            nameof(GetCourse), 
            new { id = result.Data!.Id }, 
            result);
    }

    /// <summary>
    /// Update an existing course
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<CourseDto>>> UpdateCourse(
        Guid id,
        [FromBody] UpdateCourseDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = UpdateCourseCommand.FromDto(id, dto);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Delete a course
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteCourse(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCourseCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }
}