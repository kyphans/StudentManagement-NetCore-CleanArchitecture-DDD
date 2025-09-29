using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Infrastructure.Data;

namespace StudentManagement.WebApi.Controllers;

public class HealthController : BaseApiController
{
    private readonly StudentManagementDbContext _context;

    public HealthController(StudentManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetHealthStatus()
    {
        try
        {
            // Test database connection
            await _context.Database.CanConnectAsync();

            var health = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Database = new
                {
                    Status = "Connected",
                    Provider = "SQLite",
                    Database = "StudentManagement"
                }
            };

            return Ok(health);
        }
        catch (Exception ex)
        {
            var health = new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Database = new
                {
                    Status = "Disconnected",
                    Error = ex.Message
                }
            };

            return StatusCode(503, health);
        }
    }
}