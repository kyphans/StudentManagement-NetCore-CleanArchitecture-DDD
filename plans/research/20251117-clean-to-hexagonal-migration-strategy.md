# Research Report: Clean to Hexagonal Architecture Migration Strategy

**Research Date**: 2025-11-17
**Status**: Complete
**Scope**: Migration patterns from Clean Architecture to Hexagonal Architecture in .NET

## Executive Summary

Migration from Clean to Hexagonal Architecture is NOT replacement but REFINEMENT. Clean Architecture provides layered structure (Domain, Application, Infrastructure, Presentation). Hexagonal Architecture provides explicit mechanism for implementing Clean's dependency inversion through ports and adapters.

Key insight: Clean Architecture already follows Hexagonal principles implicitly. Migration is about making ports/adapters EXPLICIT and restructuring project to emphasize inbound/outbound communication patterns.

**Recommended approach**: Incremental refactoring over "big bang" rewrite. Focus on identifying existing implicit ports, creating explicit port interfaces, then implementing adapters gradually.

## Migration Philosophy

### Core Understanding

1. **Not Mutually Exclusive**: Hexagonal IS compatible implementation of Clean Architecture
2. **Structural Refinement**: Moving from implicit to explicit ports/adapters
3. **Same Goals**: Both enforce dependency inversion, testability, separation of concerns
4. **Evolution, Not Revolution**: Build on existing Clean Architecture foundation

### What Changes

| Clean Architecture | Hexagonal Architecture |
|-------------------|------------------------|
| 4 layers (Domain, Application, Infrastructure, WebApi) | Core + Ports + Adapters |
| Repository interfaces in Domain | Secondary ports (driven) |
| Controllers in WebApi | Primary adapters (driving) |
| Application services/handlers | Application core (use cases) |
| Infrastructure implementations | Secondary adapters (driven) |

### What Stays Same

- Domain entities and business logic (untouched)
- Value objects (untouched)
- Use cases/application logic (minimal changes)
- CQRS with MediatR (fully compatible)
- DDD tactical patterns (fully compatible)

## Step-by-Step Migration Strategy

### Phase 1: Research and Preparation (1-2 days)
**Objective**: Understand current architecture, identify mapping

**Tasks**:
1. Audit current Clean Architecture structure
2. Map existing components to Hexagonal equivalents
3. Identify all external dependencies (database, APIs, file system)
4. Document current dependency flow
5. Create migration plan with phases

**Deliverables**: Migration plan document, component mapping table

### Phase 2: Define Ports (3-5 days)
**Objective**: Create explicit port interfaces for all external communications

**Inbound Ports (Primary/Driving)**:
- Identify entry points: REST API, GraphQL, CLI, message consumers
- Create port interfaces in Application layer
- Examples: `IStudentCommandPort`, `ICourseQueryPort`

**Outbound Ports (Secondary/Driven)**:
- Identify external dependencies: databases, external APIs, file systems
- Create port interfaces in Domain or Application layer
- Examples: `IStudentRepositoryPort`, `IEmailServicePort`

**Code Example**:
```csharp
// Primary Port (Driving)
public interface IStudentManagementPort
{
    Task<StudentDto> CreateStudentAsync(CreateStudentRequest request);
    Task<StudentDto> GetStudentAsync(Guid id);
}

// Secondary Port (Driven)
public interface IStudentPersistencePort
{
    Task<Student> SaveAsync(Student student);
    Task<Student?> GetByIdAsync(StudentId id);
}
```

### Phase 3: Restructure Project Layout (2-3 days)
**Objective**: Reorganize projects to reflect Hexagonal Architecture

**Recommended Structure**:
```
/StudentManagement.Core (or Domain)
  /Entities
  /ValueObjects
  /DomainEvents
  /Ports (NEW - secondary ports interfaces)

/StudentManagement.Application
  /UseCases (or Commands/Queries)
  /Ports (NEW - primary ports interfaces)
  /DTOs
  /Validators

/StudentManagement.Adapters.Persistence (was Infrastructure)
  /Database
  /Repositories (implement secondary ports)
  /Configurations

/StudentManagement.Adapters.WebApi (was WebApi)
  /Controllers (implement primary ports)
  /Middleware

/StudentManagement.Adapters.External (NEW)
  /EmailService
  /FileStorage
```

**Migration Approach**:
- Option A: Create new projects, move files gradually
- Option B: Rename existing projects, refactor in place (RECOMMENDED for minimal disruption)

### Phase 4: Create Adapters for Infrastructure (5-7 days)
**Objective**: Convert Infrastructure implementations to explicit adapters

**Tasks**:
1. Repository implementations → Secondary adapters implementing persistence ports
2. External service clients → Secondary adapters for external APIs
3. File storage → Secondary adapters for file operations
4. Email/SMS services → Secondary adapters for notifications

**Pattern**:
```csharp
// Before (Clean Architecture)
public class StudentRepository : IStudentRepository
{
    private readonly DbContext _context;
    // ...
}

// After (Hexagonal Architecture)
public class SqlServerStudentAdapter : IStudentPersistencePort
{
    private readonly DbContext _context;
    // Implements IStudentPersistencePort from Domain/Application
}
```

### Phase 5: Create Adapters for API (3-5 days)
**Objective**: Convert controllers to primary adapters

**Tasks**:
1. Controllers → Primary adapters implementing driving ports
2. API request/response mapping
3. Keep thin controllers (delegate to use cases via ports)

**Pattern**:
```csharp
// Before (Clean Architecture)
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        var command = CreateStudentCommand.FromDto(dto);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

// After (Hexagonal Architecture)
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentManagementPort _port;

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentRequest request)
    {
        var result = await _port.CreateStudentAsync(request);
        return Ok(result);
    }
}
```

### Phase 6: Migrate Domain Logic (2-3 days)
**Objective**: Ensure domain layer uses ports instead of concrete dependencies

**Tasks**:
1. Review domain entities - should have NO external dependencies (already true in Clean Architecture)
2. Update use cases to depend on port interfaces
3. Ensure all external calls go through ports

**Note**: Minimal changes needed if Clean Architecture properly implemented

### Phase 7: Update Dependency Injection (2-3 days)
**Objective**: Wire ports to adapters via DI container

**Pattern**:
```csharp
// Before
services.AddScoped<IStudentRepository, StudentRepository>();

// After
services.AddScoped<IStudentPersistencePort, SqlServerStudentAdapter>();
services.AddScoped<IStudentManagementPort, StudentApplicationService>();
```

**Best Practice**: Create separate DI extension methods per adapter package

### Phase 8: Testing and Validation (5-7 days)
**Objective**: Ensure migration didn't break functionality

**Tasks**:
1. Run existing tests (should mostly pass)
2. Update test mocks to use ports instead of old interfaces
3. Add new integration tests for adapters
4. Performance testing
5. End-to-end testing

### Phase 9: Documentation Updates (2-3 days)
**Objective**: Update all documentation to reflect new architecture

**Tasks**:
1. Update architecture diagrams
2. Update README
3. Update code standards document
4. Update codebase summary
5. Create adapter implementation guide

## CQRS + MediatR Integration

**Good News**: CQRS with MediatR is FULLY compatible with Hexagonal Architecture

**Integration Pattern**:
```csharp
// MediatR handlers become part of Application Core
// They use secondary ports for data access

public class CreateStudentCommandHandler
    : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentPersistencePort _persistencePort; // Secondary port
    private readonly IMapper _mapper;

    public async Task<ApiResponseDto<StudentDto>> Handle(...)
    {
        var student = Student.Create(...);
        await _persistencePort.SaveAsync(student);
        // ...
    }
}

// Primary adapter calls MediatR
public class StudentApplicationService : IStudentManagementPort
{
    private readonly IMediator _mediator;

    public async Task<StudentDto> CreateStudentAsync(CreateStudentRequest request)
    {
        var command = CreateStudentCommand.FromDto(request);
        var result = await _mediator.Send(command);
        return result.Data;
    }
}
```

## Risk Mitigation

### Breaking Changes
**Risk**: API contracts change during migration
**Mitigation**: Keep existing endpoints working, add new endpoints gradually

### Performance Degradation
**Risk**: Additional abstraction layers cause overhead
**Mitigation**: Benchmark critical paths, optimize adapter implementations

### Team Confusion
**Risk**: Developers unfamiliar with Hexagonal Architecture
**Mitigation**: Training sessions, pair programming, comprehensive documentation

### Incomplete Migration
**Risk**: Mixing old and new patterns causes inconsistency
**Mitigation**: Clear migration phases, code reviews, automated linting rules

## Testing Strategy During Migration

1. **Keep Existing Tests Running**: Don't break tests during migration
2. **Test Ports Independently**: Unit test port interfaces with mocks
3. **Test Adapters**: Integration tests for each adapter
4. **End-to-End Tests**: Ensure system works as whole

## Rollback Considerations

- Use feature flags for gradual rollout
- Keep old interfaces/implementations until migration complete
- Git branches for each phase
- Database migrations must be reversible
- Document rollback procedure for each phase

## Real-World Migration Patterns

### Pattern 1: Strangler Fig
- Gradually replace old implementation with new
- Old and new coexist during transition
- Eventually old code removed

### Pattern 2: Branch by Abstraction
- Create abstraction (port) first
- Implement new adapter behind abstraction
- Switch implementations via DI
- Remove old implementation

### Pattern 3: Parallel Run
- New adapters run parallel to old
- Compare results for validation
- Cut over when confidence high

## Unresolved Questions

1. How to handle shared kernel in multi-bounded-context scenarios?
2. Optimal granularity for ports (fine-grained vs coarse-grained)?
3. Versioning strategy for port interfaces?
4. Migration path for existing database migrations?

## Sources

- "Hexagonal and Clean Architecture Styles with .NET Core" - Paulovich.NET
- "Migrating Clean Architecture to Hexagonal Architecture" - ByteHide
- Software Engineering Stack Exchange discussions
- Medium articles on DDD + Hexagonal + CQRS integration
- GitHub repositories: hexagonal-architecture-acerola, clean-architecture examples
