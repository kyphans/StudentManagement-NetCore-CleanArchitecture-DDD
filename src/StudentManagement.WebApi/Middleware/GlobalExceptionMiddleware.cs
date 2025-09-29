using System.Net;
using System.Text.Json;
using FluentValidation;
using StudentManagement.Application.DTOs;

namespace StudentManagement.WebApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var apiResponse = exception switch
        {
            ValidationException validationEx => new ApiErrorResponse
            {
                Type = "ValidationError",
                Title = "Validation Failed",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = validationEx.Message,
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Errors = validationEx.Errors?.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
            },
            ArgumentException argEx => new ApiErrorResponse
            {
                Type = "ArgumentError",
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = argEx.Message,
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Errors = new[] { argEx.Message }
            },
            KeyNotFoundException => new ApiErrorResponse
            {
                Type = "NotFound",
                Title = "Resource Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = "The requested resource was not found",
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Errors = new[] { "Resource not found" }
            },
            UnauthorizedAccessException => new ApiErrorResponse
            {
                Type = "Unauthorized",
                Title = "Access Denied",
                Status = (int)HttpStatusCode.Unauthorized,
                Detail = "You are not authorized to access this resource",
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Errors = new[] { "Unauthorized access" }
            },
            _ => new ApiErrorResponse
            {
                Type = "InternalServerError",
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred while processing your request",
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Errors = new[] { "Internal server error" }
            }
        };

        response.StatusCode = apiResponse.Status;

        var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

public class ApiErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public string[] Errors { get; set; } = Array.Empty<string>();
    public Dictionary<string, object>? Extensions { get; set; }
}