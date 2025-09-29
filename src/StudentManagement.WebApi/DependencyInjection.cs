using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using System.IO.Compression;

namespace StudentManagement.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        // Controllers with enhanced configuration
        services.AddControllers(options =>
        {
            // Add global filters if needed
            options.SuppressAsyncSuffixInActionNames = false;
        });

        // API Explorer for Swagger
        services.AddEndpointsApiExplorer();

        // Response compression
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = new[]
            {
                "application/json",
                "application/xml",
                "text/plain",
                "text/json",
                "text/xml"
            };
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        // Memory caching
        services.AddMemoryCache();

        // Health checks
        services.AddHealthChecks();

        // Swagger Configuration
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Student Management API",
                Version = "v1",
                Description = @"A comprehensive Student Management System built with Clean Architecture and Domain-Driven Design principles.

## Features
- Student enrollment and management
- Course creation and administration
- Grade tracking and GPA calculation
- CQRS pattern with MediatR
- FluentValidation for business rules
- Comprehensive error handling

## Authentication
This API uses JWT Bearer tokens for authentication. Include the token in the Authorization header:
`Authorization: Bearer <your-token>`",
                Contact = new OpenApiContact
                {
                    Name = "Student Management System",
                    Email = "support@studentmanagement.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Include XML comments for better documentation
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Add response examples
            options.EnableAnnotations();

            // Add JWT Bearer authentication to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        // CORS (if needed for frontend integration)
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }
}