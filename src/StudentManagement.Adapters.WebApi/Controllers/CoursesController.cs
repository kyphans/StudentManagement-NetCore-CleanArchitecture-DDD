using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Ports;

namespace StudentManagement.Adapters.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseManagementPort _coursePort;

    public CoursesController(ICourseManagementPort coursePort)
    {
        _coursePort = coursePort;
    }

    /// <summary>
    /// Get all courses with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<CourseSummaryDto>>>> GetCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? department = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _coursePort.GetCoursesAsync(pageNumber, pageSize, searchTerm, department, isActive, cancellationToken);
        return Ok(ApiResponseDto<PagedResultDto<CourseSummaryDto>>.SuccessResult(result));
    }

    /// <summary>
    /// Get a specific course by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<CourseDto>>> GetCourse(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _coursePort.GetCourseByIdAsync(id, cancellationToken);

        if (result == null)
            return NotFound(ApiResponseDto<CourseDto>.ErrorResult("Course not found"));

        return Ok(ApiResponseDto<CourseDto>.SuccessResult(result));
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

        var result = await _coursePort.CreateCourseAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetCourse),
            new { id = result.Id },
            ApiResponseDto<CourseDto>.SuccessResult(result));
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

        var result = await _coursePort.UpdateCourseAsync(id, dto, cancellationToken);

        return Ok(ApiResponseDto<CourseDto>.SuccessResult(result));
    }

    /// <summary>
    /// Delete a course
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteCourse(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _coursePort.DeleteCourseAsync(id, cancellationToken);

        return Ok(ApiResponseDto<bool>.SuccessResult(true, "Course deleted successfully"));
    }
}