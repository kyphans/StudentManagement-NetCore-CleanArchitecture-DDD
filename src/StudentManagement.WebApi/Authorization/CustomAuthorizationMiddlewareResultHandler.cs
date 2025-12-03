using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using StudentManagement.Application.DTOs;
using System.Text.Json;

namespace StudentManagement.WebApi.Authorization;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // If authorization was successful, continue with default behavior
        if (authorizeResult.Succeeded)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        // Handle authentication failure (401)
        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new ApiResponseDto<object>
            {
                Success = false,
                Message = "Bạn chưa đăng nhập hoặc token đã hết hạn. Vui lòng đăng nhập lại.",
                Data = null,
                Errors = new List<string> { "Authentication required" }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
            return;
        }

        // Handle authorization failure (403)
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new ApiResponseDto<object>
            {
                Success = false,
                Message = "Bạn không có quyền truy cập tài nguyên này.",
                Data = null,
                Errors = new List<string> { "Insufficient permissions for this resource" }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
            return;
        }

        // Fallback to default handler
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}