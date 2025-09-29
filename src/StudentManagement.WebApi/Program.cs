using StudentManagement.Application;
using StudentManagement.Infrastructure;
using StudentManagement.WebApi;
using StudentManagement.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container by layer
services.AddApplication();
services.AddInfrastructure(config);
services.AddWebApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API V1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    });
}

app.UseHttpsRedirection();

// Response compression
app.UseResponseCompression();

// Global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable CORS (if configured)
app.UseCors("AllowAll");

// Health checks
app.MapHealthChecks("/health");

// Map controllers
app.MapControllers();

app.Run();
