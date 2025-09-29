# Architecture & Technology Stack - Comprehensive Guide

**Last Updated**: 2025-09-29
**Status**: ✅ COMPLETE - Consolidated Architecture & Tech Stack Documentation
**Coverage**: Clean Architecture rules, layer responsibilities, patterns, tech stack, and dependencies

## Architecture Overview

The Student Management System follows **Clean Architecture** with **Domain-Driven Design (DDD)** principles using a 4-layer architecture with strict dependency rules:

```
StudentManagement.Domain (Core) 
    ↑
StudentManagement.Application (Use Cases)
    ↑  
StudentManagement.Infrastructure (Data & External)
    ↑
StudentManagement.WebApi (Presentation)
```

## Technology Stack

### Target Framework & Language Features
- **.NET 8.0** with C# 12 features
- **Nullable reference types** enabled across all projects
- **Implicit usings** enabled for cleaner code

### Database & Authentication
- **SQLite** with Entity Framework Core 9.0
- **ASP.NET Core Identity** for user management
- **JWT Bearer** tokens for API authentication
- **File Location**: `studentmanagement.db` in WebApi output directory

## Dependency Flow Rules & Package Distribution

### Layer Dependencies (Strict Enforcement)
- **Domain** → No external dependencies (pure .NET)
- **Application** → References Domain only
- **Infrastructure** → References Domain + Application  
- **WebApi** → References Application + Infrastructure

### NuGet Package Distribution

**Domain Layer**: No packages (pure .NET)
- Entities, Value Objects, Domain Events, Repository Interfaces

**Application Layer**:
```xml
<PackageReference Include="MediatR" Version="13.0.0" />
<PackageReference Include="FluentValidation" Version="12.0.0" />
<PackageReference Include="AutoMapper" Version="15.0.1" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.9" />
```

**Infrastructure Layer**:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.9" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.14.0" />
```

**WebApi Layer**:
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.4" />
<PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
<PackageReference Include="AutoMapper" Version="15.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="MediatR" Version="13.0.0" />
```

## Layer Responsibilities

### Domain Layer (StudentManagement.Domain)
- **Pure business logic** with no external dependencies
- **Entities**: Core business objects (Student, Course, Enrollment, Grade)
- **Value Objects**: Immutable objects representing concepts (Email, StudentId)
- **Domain Events**: Business events that occur within aggregates
- **Repository Interfaces**: Abstractions for data access
- **Domain Services**: Business logic that doesn't belong to a single entity

### Application Layer (StudentManagement.Application)
- **CQRS Commands/Queries**: Use cases implemented via MediatR
- **DTOs**: Data transfer objects for requests and responses
- **Validation**: FluentValidation rules for business logic
- **AutoMapper Profiles**: Object-to-object mapping configurations
- **Application Services**: Orchestration of domain operations

### Infrastructure Layer (StudentManagement.Infrastructure)
- **EF Core/SQLite**: Database access and persistence
- **ASP.NET Identity**: User management and authentication
- **JWT Services**: Token generation and validation
- **Repository Implementations**: Concrete data access implementations
- **External Services**: Integration with third-party systems

### WebApi Layer (StudentManagement.WebApi)
- **Controllers**: REST API endpoints
- **Middleware**: Cross-cutting concerns (logging, exception handling)
- **DI Configuration**: Dependency injection setup
- **Authentication Setup**: JWT Bearer configuration
- **API Documentation**: Swagger/OpenAPI configuration

## CQRS Pattern Rules

### Command Pattern
- **Location**: `Application/Commands/`
- **Purpose**: Modify state, perform business operations
- **Return Type**: Void or simple result objects
- **Validation**: FluentValidation validators
- **Flow**: Controller → MediatR → Command Handler → Domain → Repository

### Query Pattern
- **Location**: `Application/Queries/`
- **Purpose**: Read-only operations, data retrieval
- **Return Type**: DTOs optimized for specific use cases
- **Performance**: Can bypass domain layer for performance
- **Flow**: Controller → MediatR → Query Handler → Repository → DTO

### MediatR Integration
- All requests/responses flow through MediatR handlers
- Validation happens in FluentValidation validators, not controllers
- Cross-cutting concerns handled via MediatR behaviors
- Decouples controllers from business logic

## Entity Framework Conventions

### Database Context
- **Location**: Infrastructure layer (`Data/StudentManagementDbContext`)
- **Configuration**: Fluent API for entity configurations
- **Migrations**: Generated in Infrastructure project

### Repository Pattern
- **Interfaces**: Defined in Domain layer
- **Implementations**: Located in Infrastructure layer
- **Generic Repository**: Base repository for common operations
- **Specific Repositories**: For complex queries and business-specific operations

### Migration Commands
```bash
# Add migration
dotnet ef migrations add <MigrationName> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Apply to database
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Remove last migration
dotnet ef migrations remove -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

## Authentication & Authorization Architecture

### JWT Bearer Implementation
- **Configuration**: `appsettings.json` under `JwtSettings`
- **User Management**: ASP.NET Core Identity
- **Role Definitions**: Admin, Teacher, Student, Staff
- **Token Storage**: Database via Identity tables
- **Claims**: Role-based and custom claims for fine-grained permissions

### Security Flow
1. User authenticates with credentials
2. JWT token generated with roles and claims
3. Token included in Authorization header for API requests
4. Middleware validates token and populates User principal
5. Authorization policies check roles/claims for access

## Development Tools & Environment

### Primary IDE & Tools
- **JetBrains Rider** - Full-featured .NET IDE
- **Entity Framework Core CLI** - Migration management
- **Serilog** - Structured logging with JSON format
- **Swagger UI** - Interactive API documentation at `/swagger`

### Essential CLI Commands
```bash
# Solution management
dotnet build                    # Build entire solution
dotnet restore                  # Restore NuGet packages
dotnet run --project WebApi     # Run application
dotnet watch --project WebApi   # Run with file watching

# Testing (when test projects exist)
dotnet test                     # Run all tests
```

## Architectural Boundaries & Rules

### Dependency Inversion
- High-level modules (Domain) don't depend on low-level modules (Infrastructure)
- Both depend on abstractions (interfaces)
- Infrastructure implements Domain interfaces

### SOLID Principles Application
- **S**: Single responsibility per class/layer
- **O**: Open/closed via dependency injection
- **L**: Liskov substitution through proper inheritance
- **I**: Interface segregation in repository design
- **D**: Dependency inversion through abstraction layers

### Domain-Driven Design
- **Aggregates**: Consistency boundaries around related entities
- **Aggregate Roots**: Entry points for aggregate access
- **Value Objects**: Immutable objects representing concepts
- **Domain Events**: Communication between aggregates

## Clean Architecture Benefits
- **Testability**: Business logic isolated from framework concerns
- **Independence**: Framework-agnostic business rules
- **Flexibility**: Easy to change external concerns (database, UI)
- **Maintainability**: Clear separation of concerns