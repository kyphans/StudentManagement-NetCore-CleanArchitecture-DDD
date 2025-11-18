using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Ports;

namespace StudentManagement.Adapters.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentManagementPort _enrollmentPort;

    public EnrollmentsController(IEnrollmentManagementPort enrollmentPort)
    {
        _enrollmentPort = enrollmentPort;
    }

    /// <summary>
    /// Get all enrollments with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>>> GetEnrollments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? courseId = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _enrollmentPort.GetEnrollmentsAsync(pageNumber, pageSize, studentId, courseId, status, cancellationToken);
        return Ok(ApiResponseDto<PagedResultDto<EnrollmentSummaryDto>>.SuccessResult(result));
    }

    /// <summary>
    /// Get a specific enrollment by ID with details
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<EnrollmentWithDetailsDto>>> GetEnrollment(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _enrollmentPort.GetEnrollmentWithDetailsAsync(id, cancellationToken);

        if (result == null)
            return NotFound(ApiResponseDto<EnrollmentWithDetailsDto>.ErrorResult("Enrollment not found"));

        return Ok(ApiResponseDto<EnrollmentWithDetailsDto>.SuccessResult(result));
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

        var result = await _enrollmentPort.CreateEnrollmentAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetEnrollment),
            new { id = result.Id },
            ApiResponseDto<EnrollmentDto>.SuccessResult(result));
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

        var result = await _enrollmentPort.AssignGradeAsync(id, dto, cancellationToken);

        return Ok(ApiResponseDto<EnrollmentDto>.SuccessResult(result));
    }
}