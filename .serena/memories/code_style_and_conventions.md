# Code Style and Conventions

## C# Language Features
- **Target Framework**: .NET 8.0
- **C# Version**: 12 (latest features enabled)
- **Nullable Reference Types**: Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)

## Project Structure Conventions
- **Clean Architecture Layers**: Domain → Application → Infrastructure → WebApi
- **Namespace Pattern**: `StudentManagement.{LayerName}.{FeatureArea}`
- **File Organization**: Group by feature/aggregate, not by technical concern

## Naming Conventions
- **Projects**: PascalCase with company/product prefix (`StudentManagement.Domain`)
- **Classes**: PascalCase (`StudentEntity`, `UserService`)
- **Interfaces**: PascalCase with 'I' prefix (`IStudentRepository`, `IUserService`)
- **Methods**: PascalCase (`GetStudentById`, `CreateStudent`)
- **Properties**: PascalCase (`FirstName`, `StudentId`)
- **Fields**: camelCase with underscore prefix (`_studentRepository`)
- **Parameters**: camelCase (`studentId`, `userName`)
- **Local Variables**: camelCase (`student`, `courseList`)

## Architecture Patterns
- **CQRS**: Commands modify state, Queries read state
- **Repository Pattern**: Interfaces in Domain, implementations in Infrastructure
- **Domain-Driven Design**: Aggregates, Value Objects, Domain Events
- **Dependency Injection**: Constructor injection preferred
- **MediatR**: All application use cases go through MediatR handlers

## Clean Architecture Dependencies
- **Domain**: No external dependencies (pure .NET)
- **Application**: References Domain only
- **Infrastructure**: References Domain + Application
- **WebApi**: References Application + Infrastructure

## Entity Framework Conventions
- **DbContext**: Lives in Infrastructure layer
- **Entities**: Domain entities with EF Core configurations
- **Migrations**: Generated in Infrastructure but run against WebApi startup
- **Connection Strings**: Stored in `appsettings.json`

## Authentication Patterns
- **ASP.NET Core Identity**: For user management
- **JWT Tokens**: For API authentication
- **Role-based Authorization**: Admin, Teacher, Student, Staff roles
- **Policy-based Authorization**: For complex permission scenarios

## Error Handling
- **Global Exception Handling**: Middleware-based approach
- **Validation**: FluentValidation for business rules
- **Result Pattern**: Consider for operation outcomes
- **Logging**: Serilog with structured logging

## API Conventions
- **REST Endpoints**: Follow REST principles
- **DTOs**: Separate models for requests/responses
- **Swagger/OpenAPI**: Full API documentation
- **Versioning**: Consider for future API versions
- **HTTP Status Codes**: Proper semantic usage

## Code Quality
- **Immutability**: Prefer immutable objects where possible
- **SOLID Principles**: Follow SOLID design principles
- **Single Responsibility**: One responsibility per class/method
- **Unit Testing**: Test domain logic and application services
- **Integration Testing**: Test API endpoints and data access