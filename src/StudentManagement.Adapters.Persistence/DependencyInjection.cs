using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Domain.Ports.IPersistence;
using StudentManagement.Adapters.Persistence.Data;
using StudentManagement.Adapters.Persistence.Repositories;

namespace StudentManagement.Adapters.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<StudentManagementDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Persistence Adapters (Secondary Adapters)
        services.AddScoped<IStudentPersistencePort, EfCoreStudentAdapter>();
        services.AddScoped<ICoursePersistencePort, EfCoreCourseAdapter>();
        services.AddScoped<IEnrollmentPersistencePort, EfCoreEnrollmentAdapter>();

        // Unit of Work
        services.AddScoped<IUnitOfWorkPort, EfCoreUnitOfWorkAdapter>();

        return services;
    }
}