using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace StudentManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR - Register all handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation - Register all validators from this assembly
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper - Register all mapping profiles from this assembly
        services.AddAutoMapper(assembly);

        return services;
    }
}