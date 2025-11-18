# Research Report: Hexagonal Architecture .NET 8 Implementation

**Research Date**: 2025-11-17
**Status**: Complete
**Scope**: .NET 8 specific patterns, project structure, code examples for Hexagonal Architecture

## Executive Summary

.NET 8 provides excellent support for Hexagonal Architecture through built-in dependency injection, interface-based programming, and clean separation of concerns. Modern .NET conventions align naturally with Hexagonal principles.

Key tools for .NET Hexagonal Architecture: MediatR (CQRS), AutoMapper (DTOs), FluentValidation (input validation), EF Core (persistence), Moq (testing). All integrate seamlessly with ports/adapters pattern.

**2024 Best Practices**: Keep minimal layers, embrace DI frameworks, write comprehensive tests, gradual migration, minimize layer-hopping for performance.

## Project Structure Conventions

### Recommended .NET Solution Structure

```
/StudentManagement.sln
├── /src
│   ├── /StudentManagement.Domain (Core - no dependencies)
│   │   ├── /Entities
│   │   ├── /ValueObjects
│   │   ├── /DomainEvents
│   │   └── /Ports (Secondary port interfaces)
│   │       ├── IPersistence
│   │       ├── IEmailService
│   │       └── IFileStorage
│   │
│   ├── /StudentManagement.Application (Use Cases)
│   │   ├── /Ports (Primary port interfaces)
│   │   │   ├── IStudentManagementPort.cs
│   │   │   └── ICourseManagementPort.cs
│   │   ├── /UseCases (or Commands/Queries)
│   │   │   ├── /Students
│   │   │   └── /Courses
│   │   ├── /DTOs
│   │   ├── /Validators
│   │   └── /Mappings
│   │
│   ├── /StudentManagement.Infrastructure (Secondary Adapters)
│   │   ├── /Persistence (Database adapter)
│   │   │   ├── DbContext
│   │   │   ├── /Configurations
│   │   │   ├── /Repositories (implement persistence ports)
│   │   │   └── /Migrations
│   │   ├── /Email (Email adapter)
│   │   ├── /Storage (File storage adapter)
│   │   └── /External (External API adapters)
│   │
│   └── /StudentManagement.WebApi (Primary Adapter)
│       ├── /Controllers (REST adapter)
│       ├── /GraphQL (optional - GraphQL adapter)
│       ├── /Middleware
│       ├── /ApplicationServices (implement primary ports)
│       └── Program.cs
│
└── /tests
    ├── /StudentManagement.Domain.Tests
    ├── /StudentManagement.Application.Tests
    ├── /StudentManagement.Infrastructure.Tests
    └── /StudentManagement.WebApi.Tests
```

### Alternative Naming Conventions

**Option A: Explicit Adapter Naming**
```
/StudentManagement.Core
/StudentManagement.Adapters.WebApi
/StudentManagement.Adapters.Persistence
/StudentManagement.Adapters.Email
```

**Option B: Traditional with Ports**
```
/StudentManagement.Domain
/StudentManagement.Application
  /Ports
/StudentManagement.Infrastructure
/StudentManagement.WebApi
```

## Port Interface Patterns

### Primary Ports (Driving)

**Location**: `Application/Ports/`

```csharp
// Student management operations (inbound)
public interface IStudentManagementPort
{
    Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request);
    Task<StudentResponse> GetStudentByIdAsync(Guid id);
    Task<PagedResult<StudentSummary>> GetStudentsAsync(StudentFilter filter);
    Task<StudentResponse> UpdateStudentAsync(Guid id, UpdateStudentRequest request);
    Task DeleteStudentAsync(Guid id);
}

// Course management operations (inbound)
public interface ICourseManagementPort
{
    Task<CourseResponse> CreateCourseAsync(CreateCourseRequest request);
    Task<CourseResponse> GetCourseByIdAsync(Guid id);
    Task<PagedResult<CourseSummary>> GetCoursesAsync(CourseFilter filter);
}
```

**Characteristics**:
- Define business operations from external actor's perspective
- Located in Application layer
- Technology-agnostic (no HTTP, no framework dependencies)
- Return DTOs, not domain entities

### Secondary Ports (Driven)

**Location**: `Domain/Ports/` or `Application/Ports/`

```csharp
// Persistence port (outbound)
public interface IStudentPersistencePort
{
    Task<Student> SaveAsync(Student student);
    Task<Student?> GetByIdAsync(StudentId id);
    Task<Student?> GetByEmailAsync(Email email);
    Task<IEnumerable<Student>> FindAsync(StudentFilter filter);
    Task<bool> ExistsAsync(StudentId id);
    Task DeleteAsync(Student student);
}

// Email service port (outbound)
public interface IEmailServicePort
{
    Task SendWelcomeEmailAsync(string to, string studentName);
    Task SendEnrollmentConfirmationAsync(string to, string courseName);
}

// File storage port (outbound)
public interface IFileStoragePort
{
    Task<string> SaveAsync(Stream fileStream, string fileName);
    Task<Stream> GetAsync(string fileId);
    Task DeleteAsync(string fileId);
}
```

**Characteristics**:
- Define operations domain needs from external systems
- Located in Domain or Application layer
- Work with domain entities or primitives
- Technology-agnostic

## Adapter Implementation Patterns

### Primary Adapter (Web API)

**Location**: `WebApi/Controllers/` + `WebApi/ApplicationServices/`

```csharp
// Application Service (implements primary port)
public class StudentApplicationService : IStudentManagementPort
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public StudentApplicationService(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        var command = _mapper.Map<CreateStudentCommand>(request);
        var result = await _mediator.Send(command);

        if (!result.Success)
            throw new BusinessException(result.Message, result.Errors);

        return _mapper.Map<StudentResponse>(result.Data);
    }

    public async Task<StudentResponse> GetStudentByIdAsync(Guid id)
    {
        var query = new GetStudentByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
            throw new NotFoundException($"Student {id} not found");

        return _mapper.Map<StudentResponse>(result.Data);
    }
}

// Controller (uses primary port)
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentManagementPort _studentPort;

    public StudentsController(IStudentManagementPort studentPort)
    {
        _studentPort = studentPort;
    }

    [HttpPost]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var result = await _studentPort.CreateStudentAsync(request);
        return CreatedAtAction(nameof(GetStudent), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var result = await _studentPort.GetStudentByIdAsync(id);
        return Ok(result);
    }
}
```

### Secondary Adapter (Persistence)

**Location**: `Infrastructure/Persistence/Repositories/`

```csharp
// EF Core adapter (implements secondary port)
public class EfCoreStudentAdapter : IStudentPersistencePort
{
    private readonly StudentManagementDbContext _context;

    public EfCoreStudentAdapter(StudentManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Student> SaveAsync(Student student)
    {
        var exists = await _context.Students.AnyAsync(s => s.Id == student.Id);

        if (exists)
            _context.Students.Update(student);
        else
            await _context.Students.AddAsync(student);

        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student?> GetByIdAsync(StudentId id)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByEmailAsync(Email email)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Student>> FindAsync(StudentFilter filter)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(s =>
                s.FirstName.Contains(filter.SearchTerm) ||
                s.LastName.Contains(filter.SearchTerm) ||
                s.Email.Value.Contains(filter.SearchTerm));
        }

        if (filter.IsActive.HasValue)
            query = query.Where(s => s.IsActive == filter.IsActive.Value);

        return await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }
}
```

### Secondary Adapter (Email Service)

**Location**: `Infrastructure/Email/`

```csharp
// SendGrid adapter (implements email port)
public class SendGridEmailAdapter : IEmailServicePort
{
    private readonly ISendGridClient _client;
    private readonly ILogger<SendGridEmailAdapter> _logger;
    private readonly EmailSettings _settings;

    public SendGridEmailAdapter(
        ISendGridClient client,
        ILogger<SendGridEmailAdapter> logger,
        IOptions<EmailSettings> settings)
    {
        _client = client;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task SendWelcomeEmailAsync(string to, string studentName)
    {
        var msg = new SendGridMessage
        {
            From = new EmailAddress(_settings.FromEmail, _settings.FromName),
            Subject = "Welcome to Student Management System",
            PlainTextContent = $"Hello {studentName}, welcome aboard!"
        };
        msg.AddTo(to);

        var response = await _client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send email to {Email}", to);
            throw new EmailException($"Failed to send email: {response.StatusCode}");
        }
    }
}
```

## Use Cases with CQRS Pattern

**Location**: `Application/UseCases/` or `Application/Commands/` + `Application/Queries/`

```csharp
// Command (write operation)
public record CreateStudentCommand : IRequest<ApiResponseDto<StudentDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
}

// Command Handler (uses secondary ports)
public class CreateStudentCommandHandler
    : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentPersistencePort _persistencePort;
    private readonly IEmailServicePort _emailPort;
    private readonly IMapper _mapper;

    public async Task<ApiResponseDto<StudentDto>> Handle(
        CreateStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create domain entity
        var email = new Email(request.Email);

        // 2. Check uniqueness via secondary port
        var existing = await _persistencePort.GetByEmailAsync(email);
        if (existing != null)
            return ApiResponseDto<StudentDto>.Failure("Email already exists");

        // 3. Create student
        var student = Student.Create(
            request.FirstName,
            request.LastName,
            email,
            request.DateOfBirth);

        // 4. Save via secondary port
        await _persistencePort.SaveAsync(student);

        // 5. Send welcome email via secondary port
        await _emailPort.SendWelcomeEmailAsync(
            email.Value,
            student.FullName);

        // 6. Map and return
        var dto = _mapper.Map<StudentDto>(student);
        return ApiResponseDto<StudentDto>.Success(dto);
    }
}

// Query (read operation)
public record GetStudentsQuery : IRequest<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

// Query Handler
public class GetStudentsQueryHandler
    : IRequestHandler<GetStudentsQuery, ApiResponseDto<PagedResultDto<StudentSummaryDto>>>
{
    private readonly IStudentPersistencePort _persistencePort;
    private readonly IMapper _mapper;

    public async Task<ApiResponseDto<PagedResultDto<StudentSummaryDto>>> Handle(
        GetStudentsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = _mapper.Map<StudentFilter>(request);
        var students = await _persistencePort.FindAsync(filter);
        var dtos = _mapper.Map<List<StudentSummaryDto>>(students);

        var result = new PagedResultDto<StudentSummaryDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = dtos.Count
        };

        return ApiResponseDto<PagedResultDto<StudentSummaryDto>>.Success(result);
    }
}
```

## Dependency Injection Configuration

**Location**: Each adapter project has DI extension method

```csharp
// Domain (no DI needed - pure business logic)

// Application Layer DI
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}

// Infrastructure Layer DI
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<StudentManagementDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Secondary Ports -> Adapters
        services.AddScoped<IStudentPersistencePort, EfCoreStudentAdapter>();
        services.AddScoped<ICoursePersistencePort, EfCoreCourseAdapter>();
        services.AddScoped<IEnrollmentPersistencePort, EfCoreEnrollmentAdapter>();

        // Email service
        services.AddScoped<IEmailServicePort, SendGridEmailAdapter>();
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

// WebApi Layer DI
public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        // Controllers
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // Primary Ports -> Application Services
        services.AddScoped<IStudentManagementPort, StudentApplicationService>();
        services.AddScoped<ICourseManagementPort, CourseApplicationService>();

        // Swagger
        services.AddSwaggerGen();

        // Middleware
        services.AddResponseCompression();
        services.AddHealthChecks();
        services.AddCors();

        return services;
    }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebApi();

var app = builder.Build();
// Middleware pipeline...
app.Run();
```

## Testing Patterns

### Unit Testing Ports

```csharp
public class StudentManagementPortTests
{
    [Fact]
    public async Task CreateStudent_ValidData_ReturnsSuccess()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var mockMapper = new Mock<IMapper>();

        mockMediator
            .Setup(m => m.Send(It.IsAny<CreateStudentCommand>(), default))
            .ReturnsAsync(ApiResponseDto<StudentDto>.Success(new StudentDto()));

        var port = new StudentApplicationService(mockMediator.Object, mockMapper.Object);
        var request = new CreateStudentRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@email.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        // Act
        var result = await port.CreateStudentAsync(request);

        // Assert
        Assert.NotNull(result);
        mockMediator.Verify(m => m.Send(It.IsAny<CreateStudentCommand>(), default), Times.Once);
    }
}
```

### Integration Testing Adapters

```csharp
public class EfCoreStudentAdapterTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public EfCoreStudentAdapterTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SaveAsync_NewStudent_SavesSuccessfully()
    {
        // Arrange
        var adapter = new EfCoreStudentAdapter(_fixture.Context);
        var student = Student.Create(
            "John",
            "Doe",
            new Email("john@email.com"),
            new DateTime(2000, 1, 1));

        // Act
        var result = await adapter.SaveAsync(student);

        // Assert
        Assert.NotNull(result);
        var saved = await adapter.GetByIdAsync(student.Id);
        Assert.NotNull(saved);
        Assert.Equal("John", saved.FirstName);
    }
}
```

## .NET 8 Specific Features

### Minimal APIs Alternative

```csharp
// Minimal API as primary adapter
app.MapPost("/api/students", async (
    CreateStudentRequest request,
    IStudentManagementPort port) =>
{
    var result = await port.CreateStudentAsync(request);
    return Results.Created($"/api/students/{result.Id}", result);
});

app.MapGet("/api/students/{id:guid}", async (
    Guid id,
    IStudentManagementPort port) =>
{
    var result = await port.GetStudentByIdAsync(id);
    return Results.Ok(result);
});
```

### Native AOT Support

- Avoid reflection in port/adapter initialization
- Use source generators for DI when possible
- Trim-friendly implementations

## Performance Considerations

1. **Minimize Layer Hopping**: Keep adapter logic thin
2. **Async All The Way**: Use async/await throughout stack
3. **Lazy Loading**: Load related entities only when needed
4. **Caching**: Implement caching in adapters, not domain
5. **Connection Pooling**: Use DbContext pooling for EF Core

## Common Pitfalls

1. **Over-Engineering**: Too many ports for simple operations
2. **Anemic Adapters**: Business logic leaking into adapters
3. **Port Proliferation**: Creating port for every minor operation
4. **Tight Coupling**: Ports depending on concrete types
5. **Ignoring Performance**: Too many abstraction layers

## Unresolved Questions

1. How to handle distributed transactions across multiple adapters?
2. Best approach for adapter versioning in microservices?
3. Caching strategy: at port level or adapter level?
4. Error handling: domain exceptions vs adapter exceptions?

## Sources

- "Hexagonal Architectural Pattern in C# - Full Guide 2024" - ByteHide
- GitHub: ivanpaulovich/hexagonal-architecture-acerola
- GitHub: bitloops/ddd-hexagonal-cqrs-es-eda
- Awesome Software Architecture - Hexagonal Architecture section
- .NET 8 Microservices course materials
- Code Maze C# Hexagonal Pattern guide
