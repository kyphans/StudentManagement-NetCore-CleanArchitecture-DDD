using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Ports;

namespace StudentManagement.Adapters.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentManagementPort _studentPort;

    public StudentsController(IStudentManagementPort studentPort)
    {
        _studentPort = studentPort;
    }

    /// <summary>
    /// Get all students with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>> GetStudents(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _studentPort.GetStudentsAsync(pageNumber, pageSize, searchTerm, isActive, cancellationToken);
        return Ok(ApiResponseDto<PagedResultDto<StudentSummaryDto>>.SuccessResult(result));
    }

    /// <summary>
    /// Get a specific student by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> GetStudent(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _studentPort.GetStudentByIdAsync(id, cancellationToken);

        if (result == null)
            return NotFound(ApiResponseDto<StudentDto>.ErrorResult("Student not found"));

        return Ok(ApiResponseDto<StudentDto>.SuccessResult(result));
    }

    /// <summary>
    /// Create a new student
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> CreateStudent(
        [FromBody] CreateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _studentPort.CreateStudentAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetStudent),
            new { id = result.Id },
            ApiResponseDto<StudentDto>.SuccessResult(result));
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

        var result = await _studentPort.UpdateStudentAsync(id, dto, cancellationToken);

        return Ok(ApiResponseDto<StudentDto>.SuccessResult(result));
    }

    /// <summary>
    /// Delete a student
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteStudent(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _studentPort.DeleteStudentAsync(id, cancellationToken);

        return Ok(ApiResponseDto<bool>.SuccessResult(true, "Student deleted successfully"));
    }
}