using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Commands.Authentication;
using StudentManagement.Application.DTOs;

namespace StudentManagement.WebApi.Controllers;

/// <summary>
/// Controller cho authentication và authorization
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Đăng ký user mới
    /// </summary>
    /// <param name="dto">Thông tin đăng ký</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Thông tin user đã tạo</returns>
    [HttpPost("register")]
    [AllowAnonymous] // Cho phép anonymous (chưa đăng nhập)
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Register(
        [FromBody] RegisterRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = RegisterCommand.FromDto(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    /// <param name="dto">Username và password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token và refresh token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponseDto<AuthenticationResponseDto>>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Lấy IP address của client
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = LoginCommand.FromDto(dto, ipAddress);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin user hiện tại (yêu cầu authenticated)
    /// </summary>
    /// <returns>Thông tin user</returns>
    [HttpGet("me")]
    [Authorize] // Yêu cầu authenticated
    public ActionResult<object> GetCurrentUser()
    {
        // Lấy claims từ token
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var fullName = User.FindFirst("fullName")?.Value;

        return Ok(new
        {
            userId,
            username,
            email,
            role,
            fullName
        });
    }

    /// <summary>
    /// Test endpoint cho Admin only
    /// </summary>
    [HttpGet("admin-test")]
    [Authorize(Policy = "AdminOnly")]
    public ActionResult<object> AdminTest()
    {
        return Ok(new { message = "Bạn là Admin!" });
    }

    /// <summary>
    /// Test endpoint cho Teacher only
    /// </summary>
    [HttpGet("teacher-test")]
    [Authorize(Policy = "TeacherOnly")]
    public ActionResult<object> TeacherTest()
    {
        return Ok(new { message = "Bạn là Teacher!" });
    }
}