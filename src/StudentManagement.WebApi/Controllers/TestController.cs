using Microsoft.AspNetCore.Mvc;
using StudentManagement.Domain.Repositories;

namespace StudentManagement.WebApi.Controllers;

public class TestController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public TestController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("di-test")]
    public IActionResult TestDependencyInjection()
    {
        try
        {
            // Test that all repositories are properly injected
            var result = new
            {
                Status = "Success",
                Message = "Dependency Injection is working correctly",
                Timestamp = DateTime.UtcNow,
                RegisteredServices = new
                {
                    UnitOfWork = _unitOfWork != null ? "✅ Registered" : "❌ Not Registered",
                    StudentRepository = _unitOfWork.Students != null ? "✅ Registered" : "❌ Not Registered",
                    CourseRepository = _unitOfWork.Courses != null ? "✅ Registered" : "❌ Not Registered",
                    EnrollmentRepository = _unitOfWork.Enrollments != null ? "✅ Registered" : "❌ Not Registered"
                }
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Error",
                Message = "Dependency Injection failed",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}