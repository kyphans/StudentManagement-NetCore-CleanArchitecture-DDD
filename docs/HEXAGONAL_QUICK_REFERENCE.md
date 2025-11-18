# Hexagonal Architecture - Quick Reference Guide

**Student Management System** | **Version**: 2.0 (Hexagonal) | **Last Updated**: 2025-11-18

## 🎯 What is Hexagonal Architecture?

Hexagonal Architecture (also called Ports & Adapters) is an architectural pattern that:
- **Isolates business logic** from external concerns
- **Makes technology decisions** easily replaceable
- **Enables testing** without external dependencies
- **Clarifies boundaries** between core and infrastructure

## 🏗️ Architecture Layers

```
┌──────────────────────────────┐
│  PRIMARY ADAPTERS            │  HTTP API, gRPC, CLI
│  (Driving/Inbound)           │  Controllers, Services
└──────────┬───────────────────┘
           ↓
┌──────────────────────────────┐
│  PRIMARY PORTS               │  Application Interfaces
│  (Inbound Interfaces)        │  IStudentManagementPort
└──────────┬───────────────────┘
           ↓
┌──────────────────────────────┐
│  APPLICATION CORE            │
│  ┌────────────────────────┐  │
│  │ DOMAIN (Hexagon Core)  │  │  Pure Business Logic
│  │ Entities, Value Objects│  │  No Dependencies
│  └────────────────────────┘  │
│  ┌────────────────────────┐  │
│  │ APPLICATION            │  │  Use Cases
│  │ Commands, Queries, DTOs│  │  Orchestration
│  └────────────────────────┘  │
└──────────┬───────────────────┘
           ↓
┌──────────────────────────────┐
│  SECONDARY PORTS             │  Persistence Interfaces
│  (Outbound Interfaces)       │  IStudentPersistencePort
└──────────┬───────────────────┘
           ↓
┌──────────────────────────────┐
│  SECONDARY ADAPTERS          │  Database, External APIs
│  (Driven/Outbound)           │  EfCoreStudentAdapter
└──────────────────────────────┘
```

## 📁 Project Structure

```
src/StudentManagement.Domain/              # 🎯 DOMAIN CORE
└── Ports/IPersistence/                    # 🔌 Secondary Ports (Outbound)
    ├── IStudentPersistencePort.cs
    ├── ICoursePersistencePort.cs
    └── IUnitOfWorkPort.cs

src/StudentManagement.Application/         # 🔄 APPLICATION CORE
├── Ports/                                 # 🔌 Primary Ports (Inbound)
│   ├── IStudentManagementPort.cs
│   ├── ICourseManagementPort.cs
│   └── IEnrollmentManagementPort.cs
├── Commands/                              # CQRS Write
├── Queries/                               # CQRS Read
└── DTOs/                                  # Data Transfer

src/StudentManagement.Adapters.Persistence/  # 🔧 Secondary Adapters
└── Repositories/
    ├── EfCoreStudentAdapter.cs           # implements IStudentPersistencePort
    ├── EfCoreCourseAdapter.cs            # implements ICoursePersistencePort
    └── EfCoreUnitOfWorkAdapter.cs        # implements IUnitOfWorkPort

src/StudentManagement.Adapters.WebApi/     # 🌐 Primary Adapters
├── Controllers/                           # HTTP Endpoints
│   ├── StudentsController.cs             # uses IStudentManagementPort
│   ├── CoursesController.cs              # uses ICourseManagementPort
│   └── EnrollmentsController.cs          # uses IEnrollmentManagementPort
└── ApplicationServices/                   # Primary Port Implementations
    ├── StudentApplicationService.cs       # implements IStudentManagementPort
    ├── CourseApplicationService.cs        # implements ICourseManagementPort
    └── EnrollmentApplicationService.cs    # implements IEnrollmentManagementPort
```

## 🔑 Key Concepts

### Primary Port (Inbound Interface)

**What**: Interface defining what the application PROVIDES to external world
**Where**: `Application/Ports/I*ManagementPort.cs`
**Example**: `IStudentManagementPort`

```csharp
public interface IStudentManagementPort
{
    Task<StudentDto> CreateStudentAsync(CreateStudentDto request);
    Task<StudentDto> GetStudentByIdAsync(Guid id);
}
```

### Primary Adapter (Driving Adapter)

**What**: Implementation connecting external actors TO the core
**Where**: `Adapters.WebApi/ApplicationServices/*ApplicationService.cs`
**Example**: `StudentApplicationService`

```csharp
public class StudentApplicationService : IStudentManagementPort
{
    private readonly IMediator _mediator;
    // Implements the port interface
}
```

### Secondary Port (Outbound Interface)

**What**: Interface defining what the application NEEDS from external systems
**Where**: `Domain/Ports/IPersistence/I*PersistencePort.cs`
**Example**: `IStudentPersistencePort`

```csharp
public interface IStudentPersistencePort : IPersistencePort<Student, StudentId>
{
    Task<Student?> GetByEmailAsync(Email email);
    Task<IEnumerable<Student>> GetActiveStudentsAsync();
}
```

### Secondary Adapter (Driven Adapter)

**What**: Implementation connecting the core TO external systems
**Where**: `Adapters.Persistence/Repositories/EfCore*Adapter.cs`
**Example**: `EfCoreStudentAdapter`

```csharp
public class EfCoreStudentAdapter : IStudentPersistencePort
{
    private readonly StudentManagementDbContext _context;
    // Implements the port interface
}
```

## 🔄 Request Flow Example

```
1. HTTP POST /api/students
        ↓
2. StudentsController.CreateStudent()
   (PRIMARY ADAPTER - Adapters.WebApi)
        ↓
3. IStudentManagementPort.CreateStudentAsync()
   (PRIMARY PORT - Application/Ports)
        ↓
4. StudentApplicationService.CreateStudentAsync()
   (PRIMARY ADAPTER IMPLEMENTATION)
        ↓
5. MediatR.Send(CreateStudentCommand)
   (APPLICATION CORE)
        ↓
6. CreateStudentCommandHandler.Handle()
   (APPLICATION USE CASE)
        ↓
7. Student.Create() + Validation
   (DOMAIN LOGIC)
        ↓
8. IStudentPersistencePort.AddAsync()
   (SECONDARY PORT - Domain/Ports)
        ↓
9. EfCoreStudentAdapter.AddAsync()
   (SECONDARY ADAPTER - Adapters.Persistence)
        ↓
10. DbContext.SaveChangesAsync()
    (DATABASE)
```

## 📝 Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Primary Port | I*ManagementPort | IStudentManagementPort |
| Primary Adapter Service | *ApplicationService | StudentApplicationService |
| Secondary Port | I*PersistencePort | IStudentPersistencePort |
| Secondary Adapter | EfCore*Adapter | EfCoreStudentAdapter |
| Port Base Interface | I*Port | IPersistencePort<T, TId> |

## 🛠️ Common Commands

### Build & Run
```bash
# Build solution
dotnet build

# Run API
dotnet run --project src/StudentManagement.Adapters.WebApi

# Access Swagger
# http://localhost:5282/swagger
```

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName \
  -p src/StudentManagement.Adapters.Persistence \
  -s src/StudentManagement.Adapters.WebApi

# Apply migration
dotnet ef database update \
  -p src/StudentManagement.Adapters.Persistence \
  -s src/StudentManagement.Adapters.WebApi

# Remove last migration
dotnet ef migrations remove \
  -p src/StudentManagement.Adapters.Persistence \
  -s src/StudentManagement.Adapters.WebApi
```

## 🎯 Benefits of This Architecture

| Benefit | Description |
|---------|-------------|
| **Framework Independence** | ASP.NET Core → gRPC/GraphQL easily |
| **Database Independence** | SQLite → PostgreSQL/MongoDB easily |
| **UI Independence** | Web API → CLI/Desktop easily |
| **Testability** | Mock adapters for unit tests |
| **Clear Boundaries** | Ports define exact contracts |
| **Technology Agnostic** | Business logic has zero tech dependencies |

## 🔄 Swapping Technologies

### Example: Change Database
```csharp
// Create new adapter
public class MongoStudentAdapter : IStudentPersistencePort
{
    // Implement using MongoDB instead of EF Core
}

// Update DI registration
services.AddScoped<IStudentPersistencePort, MongoStudentAdapter>();
// Core logic unchanged!
```

### Example: Add gRPC API
```csharp
// Create new primary adapter
public class StudentGrpcService : IStudentManagementPort
{
    // Implement gRPC endpoints
}

// Core logic unchanged!
```

## 📊 Architecture Comparison

| Aspect | Clean Architecture | Hexagonal Architecture |
|--------|-------------------|------------------------|
| **Terminology** | Layers (Domain, Application, Infrastructure, Presentation) | Hexagon + Ports & Adapters |
| **Focus** | Layer dependencies | Data flow direction (in/out) |
| **Interfaces** | Implicit (repositories) | Explicit (ports) |
| **Adapters** | Mixed with layers | Clearly separated |
| **Clarity** | Good | Excellent |

## 🧪 Testing Strategy

### Unit Tests
```csharp
// Mock secondary port
var mockPersistencePort = new Mock<IStudentPersistencePort>();

// Test command handler with mocked persistence
var handler = new CreateStudentCommandHandler(
    mockPersistencePort.Object,
    ...);
```

### Integration Tests
```csharp
// Use real adapters
var testDbContext = CreateTestDbContext();
var persistenceAdapter = new EfCoreStudentAdapter(testDbContext);

// Test with real database
```

## 📚 Key Files to Review

1. **Architecture Explanation**: `docs/ARCHITECTURE_EXPLANATION_VN.md`
2. **Migration Summary**: `docs/MIGRATION_COMPLETE_SUMMARY.md`
3. **System Architecture**: `docs/system-architecture.md`
4. **Code Standards**: `docs/code-standards.md`
5. **CLAUDE Guide**: `CLAUDE.md`

## 🎓 Learning Path

1. ✅ Understand Hexagonal Architecture concept
2. ✅ Learn Primary vs Secondary distinction
3. ✅ Study Port interfaces (inbound/outbound)
4. ✅ Understand Adapter implementations
5. ✅ Practice with code examples
6. ✅ Review data flow diagrams
7. ✅ Experiment with swapping adapters

## 🔗 Related Patterns

- **Domain-Driven Design (DDD)**: Entities, Value Objects, Aggregates
- **CQRS**: Command/Query separation (Application layer)
- **Ports Pattern**: Replaces traditional Repository Pattern
- **Dependency Inversion**: All dependencies point inward
- **Clean Architecture**: Similar goals, different structure

## ⚡ Quick Checklist

When adding new features:
- [ ] Is this a new domain concept? → Add to Domain/Entities or ValueObjects
- [ ] New persistence operation? → Add to Secondary Port (IPersistence)
- [ ] New API endpoint? → Use Primary Port (IManagementPort)
- [ ] New external integration? → Create new Secondary Port & Adapter
- [ ] New UI technology? → Create new Primary Adapter

---

**Quick Reference Version**: 1.0
**For**: Student Management System (Hexagonal Architecture)
**Status**: ✅ Production Ready
