using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Application.Common.Behaviors;

namespace StudentManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR - Register all handlers from this assembly with behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // FluentValidation - Register all validators from this assembly
        services.AddValidatorsFromAssembly(assembly);

        // AutoMapper - Register all mapping profiles from this assembly
        services.AddAutoMapper(assembly);

        return services;
    }
}