using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Commands.Students;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Queries.Students;

namespace StudentManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all students with optional filtering and pagination
    /// Override: chỉ Admin, Teacher, Staff mới xem list
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>> GetStudents(
        [FromQuery] StudentFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = GetStudentsQuery.FromDto(filter);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Get a specific student by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> GetStudent(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new student
    /// Override: chỉ Admin, Staff mới tạo student
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> CreateStudent(
        [FromBody] CreateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = CreateStudentCommand.FromDto(dto);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return CreatedAtAction(
            nameof(GetStudent), 
            new { id = result.Data!.Id }, 
            result);
    }

    /// <summary>
    /// Update an existing student
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> UpdateStudent(
        Guid id,
        [FromBody] UpdateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = UpdateStudentCommand.FromDto(id, dto);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Delete a student
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteStudent(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteStudentCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }
}