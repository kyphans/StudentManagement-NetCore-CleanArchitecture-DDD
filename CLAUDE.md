# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Student Management System** built using **Hexagonal Architecture** (Ports & Adapters) with **Domain-Driven Design (DDD)** principles in .NET 8.0.

**Architecture**: Hexagonal Architecture (Domain → Application → Adapters)
**Database**: SQLite with Entity Framework Core
**Key Patterns**: CQRS (MediatR), Repository Pattern, Domain Events, AutoMapper, FluentValidation

> **Note**: Project is migrating from Clean Architecture to Hexagonal Architecture naming (Phase 03 complete)

## Essential Commands

### Build and Run
```bash
# Build solution
dotnet build

# Run API (starts on http://localhost:5282)
dotnet run --project src/StudentManagement.Adapters.WebApi

# Clean build
dotnet clean && dotnet build
```

### Database Migrations
```bash
# Create migration (use -p for project containing DbContext, -s for startup project)
dotnet ef migrations add <MigrationName> -p src/StudentManagement.Adapters.Persistence -s src/StudentManagement.Adapters.WebApi

# Apply migrations to database
dotnet ef database update -p src/StudentManagement.Adapters.Persistence -s src/StudentManagement.Adapters.WebApi

# Remove last migration
dotnet ef migrations remove -p src/StudentManagement.Adapters.Persistence -s src/StudentManagement.Adapters.WebApi
```

### Testing and Development
```bash
# Access Swagger UI at http://localhost:5282/swagger after running
# Health check endpoint: http://localhost:5282/health

# View connection string and settings in:
# src/StudentManagement.Adapters.WebApi/appsettings.json
```

## Architecture Rules

### Layer Dependencies (STRICT)
```
Domain (no dependencies)
  ↑
Application (depends on Domain only)
  ↑
Adapters.Persistence (depends on Domain + Application)
  ↑
Adapters.WebApi (depends on Application + Adapters.Persistence)
```

**Critical**: Domain layer has ZERO external dependencies (no NuGet packages)

### CQRS Pattern with MediatR
- **Commands**: Write operations (Create, Update, Delete) - return data or success
- **Queries**: Read operations (Get, List) - return DTOs
- **Handlers**: One handler per command/query in Application layer
- **Validators**: FluentValidation validators for each command (registered in ValidationBehavior pipeline)

### Repository Pattern
- **Interfaces**: Defined in `Domain/Repositories/` (IStudentRepository, ICourseRepository, etc.)
- **Implementations**: In `Adapters.Persistence/Repositories/`
- **Unit of Work**: IUnitOfWork for transaction management
- **Base Repository**: IRepository<T> provides common CRUD operations

### Domain-Driven Design
- **Entities**: Rich domain models in `Domain/Entities/` (Student, Course, Enrollment, Grade)
- **Value Objects**: Immutable types in `Domain/ValueObjects/` (Email, GPA, CourseCode, StudentId, etc.)
- **Domain Events**: In `Domain/Events/` (StudentEnrolledEvent, GradeAssignedEvent, CourseCompletedEvent)
- **Aggregate Root**: BaseEntity provides ID and common behavior

## Project Structure

```
src/
├── StudentManagement.Domain/           # Core business logic (NO external dependencies)
│   ├── Entities/                      # Student, Course, Enrollment, Grade
│   ├── ValueObjects/                  # Email, GPA, CourseCode, StudentId
│   ├── Events/                        # Domain events (IDomainEvent)
│   └── Repositories/                  # Repository interfaces
│
├── StudentManagement.Application/      # Use cases (depends on Domain only)
│   ├── Commands/                      # Students/, Courses/, Enrollments/
│   │   └── [Entity]/                  # CreateXCommand.cs, UpdateXCommand.cs, etc.
│   ├── Queries/                       # GetXByIdQuery.cs, GetXsQuery.cs
│   ├── DTOs/                          # StudentDtos.cs, CourseDtos.cs, etc.
│   ├── Validators/                    # FluentValidation validators
│   ├── Mappings/                      # AutoMapper profiles
│   ├── Common/Behaviors/              # ValidationBehavior (MediatR pipeline)
│   └── DependencyInjection.cs         # Service registration
│
├── StudentManagement.Adapters.Persistence/   # Data access adapter
│   ├── Data/
│   │   ├── StudentManagementDbContext.cs
│   │   └── Configurations/            # EF Core entity configurations
│   ├── Repositories/                  # Repository implementations
│   ├── Migrations/                    # EF Core migrations
│   └── DependencyInjection.cs
│
└── StudentManagement.Adapters.WebApi/       # API adapter (presentation layer)
    ├── Controllers/                   # StudentsController, CoursesController, etc.
    ├── Middleware/                    # GlobalExceptionMiddleware
    ├── Program.cs                     # Startup configuration
    └── appsettings.json               # Configuration
```

## Key Dependencies

### Domain Layer
- **None** - Keep it pure!

### Application Layer
- MediatR 13.0.0 (CQRS pattern)
- AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
- FluentValidation 12.0.0
- FluentValidation.DependencyInjectionExtensions 12.0.0

### Adapters.Persistence Layer
- Microsoft.EntityFrameworkCore.Sqlite 8.0.4
- Microsoft.EntityFrameworkCore.Design 8.0.4

### Adapters.WebApi Layer
- Swashbuckle.AspNetCore 6.4.0 (Swagger)
- Swashbuckle.AspNetCore.Annotations 9.0.4
- Serilog.AspNetCore 9.0.0 (logging)
- MediatR 13.0.0
- AutoMapper + AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1

## Configuration

### Database
- **File**: `studentmanagement.db` (auto-created in WebApi output directory)
- **Connection String**: In `appsettings.json` → `ConnectionStrings:DefaultConnection`
- **Provider**: SQLite with EF Core

### Dependency Injection
Each layer registers its services via extension methods:
- `services.AddApplication()` - in Application/DependencyInjection.cs
- `services.AddPersistence(config)` - in Adapters.Persistence/DependencyInjection.cs
- `services.AddWebApi()` - in Adapters.WebApi/DependencyInjection.cs

### API Features
- Global exception handling (GlobalExceptionMiddleware)
- Response compression (Gzip)
- CORS (AllowAll policy in development)
- Health checks at `/health`
- Swagger UI at `/swagger`

## Development Workflow

### Adding New Features
1. **Domain**: Create entity/value object in Domain layer (if needed)
2. **Application**:
   - Create command/query class
   - Create handler class
   - Create validator (FluentValidation)
   - Add DTO and AutoMapper profile
3. **Adapters.Persistence**: Add repository methods if needed
4. **Adapters.WebApi**: Create controller endpoint

### Database Changes
1. Modify entity in Domain layer
2. Update EF configuration in Adapters.Persistence/Data/Configurations/
3. Create migration: `dotnet ef migrations add <Name> -p src/StudentManagement.Adapters.Persistence -s src/StudentManagement.Adapters.WebApi`
4. Apply migration: `dotnet ef database update -p src/StudentManagement.Adapters.Persistence -s src/StudentManagement.Adapters.WebApi`

### Debugging
- Database file location: `src/StudentManagement.Adapters.WebApi/bin/Debug/net8.0/studentmanagement.db`
- Logs: Configured via Serilog in Program.cs
- Swagger: Navigate to http://localhost:5282/swagger for API testing