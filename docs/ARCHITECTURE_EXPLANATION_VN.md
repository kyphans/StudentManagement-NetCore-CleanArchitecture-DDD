# Giải Thích Mẫu Kiến Trúc - Student Management System

## Tổng Quan Kiến Trúc

Hệ thống Student Management sử dụng **Hexagonal Architecture (Ports & Adapters)** kết hợp với **Domain-Driven Design (DDD)** và **CQRS pattern**, được xây dựng trên .NET 8.0 với SQLite database.

## Các Mẫu Kiến Trúc Chính Được Xác Định

### 1. Hexagonal Architecture (Kiến Trúc Lục Giác) - Ports & Adapters

**Định nghĩa**: Kiến trúc tập trung vào business logic (hexagon core) với các cổng (ports) và bộ chuyển đổi (adapters) cho phép tương tác với external systems mà không làm ảnh hưởng đến core logic.

**Vị trí sử dụng**: Toàn bộ dự án được tổ chức theo Hexagonal principles

**Lý do sử dụng**:
- Tách biệt hoàn toàn business logic khỏi technical details
- Framework-agnostic và database-agnostic
- Dễ test với mocks/stubs
- Thay đổi infrastructure không ảnh hưởng domain
- Rõ ràng về data flow (inbound/outbound)

**Ví dụ Cấu Trúc**:
```
src/
├── StudentManagement.Domain/           # Core business logic (Hexagon)
├── StudentManagement.Application/      # Use cases & Primary Ports
├── StudentManagement.Adapters.Persistence/  # Secondary Adapters (Database)
└── StudentManagement.Adapters.WebApi/      # Primary Adapters (HTTP API)
```

**Sơ Đồ Hexagonal Architecture**:
```
┌─────────────────────────────────────────┐
│  Primary Adapters (Driving/Inbound)    │
│  Adapters.WebApi                        │
│  - Controllers                          │
│  - ApplicationServices                  │
└──────────────────┬──────────────────────┘
                   ↓
┌──────────────────────────────────────────┐
│  Primary Ports (Inbound Interfaces)     │
│  Application/Ports/                      │
│  - IStudentManagementPort                │
│  - ICourseManagementPort                 │
│  - IEnrollmentManagementPort             │
└──────────────────┬──────────────────────┘
                   ↓
┌─────────────────────────────────────────┐
│         APPLICATION CORE                │
│         (The Hexagon)                   │
│                                         │
│  ┌───────────────────────────────────┐ │
│  │ Domain (Pure Business Logic)      │ │
│  │ - Entities                        │ │
│  │ - Value Objects                   │ │
│  │ - Domain Events                   │ │
│  └───────────────────────────────────┘ │
│                                         │
│  ┌───────────────────────────────────┐ │
│  │ Application (Use Cases)           │ │
│  │ - Commands/Queries (CQRS)         │ │
│  │ - DTOs                            │ │
│  │ - Validators                      │ │
│  │ - Mappings                        │ │
│  └───────────────────────────────────┘ │
└──────────────────┬──────────────────────┘
                   ↓
┌──────────────────────────────────────────┐
│  Secondary Ports (Outbound Interfaces)  │
│  Domain/Ports/IPersistence/              │
│  - IStudentPersistencePort               │
│  - ICoursePersistencePort                │
│  - IEnrollmentPersistencePort            │
│  - IUnitOfWorkPort                       │
└──────────────────┬──────────────────────┘
                   ↓
┌──────────────────────────────────────────┐
│  Secondary Adapters (Driven/Outbound)   │
│  Adapters.Persistence                    │
│  - EfCoreStudentAdapter                  │
│  - EfCoreCourseAdapter                   │
│  - EfCoreEnrollmentAdapter               │
│  - DbContext, Configurations, Migrations │
└──────────────────────────────────────────┘
```

**Khái Niệm Ports & Adapters**:

- **Primary Ports** (Inbound): Interface định nghĩa các operations mà ứng dụng cung cấp ra ngoài
  - Ví dụ: `IStudentManagementPort`, `ICourseManagementPort`

- **Primary Adapters** (Driving): Implementations kết nối external actors vào application core
  - Ví dụ: `StudentsController`, `StudentApplicationService`

- **Secondary Ports** (Outbound): Interface định nghĩa các operations mà core cần từ external systems
  - Ví dụ: `IStudentPersistencePort`, `ICoursePersistencePort`

- **Secondary Adapters** (Driven): Implementations kết nối core với external systems
  - Ví dụ: `EfCoreStudentAdapter`, `EfCoreCourseAdapter`

### 2. Domain-Driven Design (DDD)

**Định nghĩa**: Tập trung vào domain logic và business rules, sử dụng entities, value objects và domain events.

**Vị trí sử dụng**: Domain layer với các thành phần:

**Ví dụ Domain Entities**:
```csharp
// Rich domain model với business logic
public class Student : BaseEntity<StudentId>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }

    // Business method
    public GPA CalculateGPA()
    {
        // Business logic tính GPA
    }

    // Factory method
    public static Student Create(string firstName, string lastName, Email email, DateTime dateOfBirth)
    {
        // Domain validation và business rules
        return new Student { ... };
    }
}
```

**Value Objects**:
```csharp
public class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");

        Value = email;
    }

    private bool IsValidEmail(string email)
    {
        // Email validation logic
    }
}
```

### 3. CQRS (Command Query Responsibility Segregation)

**Định nghĩa**: Tách biệt operations đọc (Query) và ghi (Command) để tối ưu hóa performance và clarity.

**Vị trí sử dụng**: Application layer với MediatR

**Ví dụ Command Handler**:
```csharp
public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentPersistencePort _persistencePort;  // Secondary Port
    private readonly IUnitOfWorkPort _unitOfWork;
    private readonly IMapper _mapper;

    public async Task<ApiResponseDto<StudentDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        // Xử lý business logic cho việc tạo student
        var email = new Email(request.Email);
        var student = Student.Create(request.FirstName, request.LastName, email, request.DateOfBirth);

        await _persistencePort.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var studentDto = _mapper.Map<StudentDto>(student);
        return ApiResponseDto<StudentDto>.SuccessResult(studentDto);
    }
}
```

**Ví dụ Query Handler**:
```csharp
public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, ApiResponseDto<PagedResultDto<StudentSummaryDto>>>
{
    private readonly IStudentPersistencePort _persistencePort;  // Secondary Port
    private readonly IMapper _mapper;

    public async Task<ApiResponseDto<PagedResultDto<StudentSummaryDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        // Chỉ đọc dữ liệu, không modify
        var students = await _persistencePort.GetAllAsync(cancellationToken);
        var pagedResult = // ... pagination logic
        return ApiResponseDto.SuccessResult(pagedResult);
    }
}
```

### 4. Ports Pattern (Thay thế Repository Pattern)

**Định nghĩa**: Interface định nghĩa contract cho data access, tách biệt khỏi implementation details. Khác với Repository, Ports rõ ràng về direction (inbound/outbound).

**Vị trí sử dụng**:
- **Secondary Ports**: Interface trong Domain/Ports/IPersistence
- **Secondary Adapters**: Implementation trong Adapters.Persistence

**Ví dụ Secondary Port (Persistence)**:
```csharp
// Domain/Ports/IPersistence/IStudentPersistencePort.cs
public interface IStudentPersistencePort : IPersistencePort<Student, StudentId>
{
    Task<Student?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> GetActiveStudentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Student>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Student?> GetWithEnrollmentsAsync(StudentId id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, StudentId? excludeStudentId = null, CancellationToken cancellationToken = default);
}
```

**Secondary Adapter Implementation**:
```csharp
// Adapters.Persistence/Repositories/EfCoreStudentAdapter.cs
public class EfCoreStudentAdapter : EfCoreRepositoryBase<Student, StudentId>, IStudentPersistencePort
{
    private readonly StudentManagementDbContext _context;

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Student?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }
}
```

**Ví dụ Primary Port (Application Service)**:
```csharp
// Application/Ports/IStudentManagementPort.cs
public interface IStudentManagementPort
{
    Task<StudentDto> CreateStudentAsync(CreateStudentDto request, CancellationToken cancellationToken = default);
    Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto request, CancellationToken cancellationToken = default);
    Task DeleteStudentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StudentDto?> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResultDto<StudentSummaryDto>> GetStudentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
}
```

**Primary Adapter Implementation**:
```csharp
// Adapters.WebApi/ApplicationServices/StudentApplicationService.cs
public class StudentApplicationService : IStudentManagementPort
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto request, CancellationToken cancellationToken = default)
    {
        var command = CreateStudentCommand.FromDto(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success || result.Data == null)
            throw new InvalidOperationException(result.Message);

        return result.Data;
    }
}
```

### 5. Unit of Work Pattern

**Định nghĩa**: Quản lý transactions và đảm bảo data consistency across multiple operations.

**Ví dụ Port Interface**:
```csharp
// Domain/Ports/IPersistence/IUnitOfWorkPort.cs
public interface IUnitOfWorkPort
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

**Ví dụ Adapter Implementation**:
```csharp
// Adapters.Persistence/Repositories/EfCoreUnitOfWorkAdapter.cs
public class EfCoreUnitOfWorkAdapter : IUnitOfWorkPort
{
    private readonly StudentManagementDbContext _context;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### 6. Mediator Pattern (MediatR)

**Định nghĩa**: Tập trung xử lý requests thông qua một mediator, giảm coupling giữa controllers và business logic.

**Ví dụ Controller sử dụng Primary Port**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentManagementPort _studentPort;  // Primary Port injection

    public StudentsController(IStudentManagementPort studentPort)
    {
        _studentPort = studentPort;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> CreateStudent(
        [FromBody] CreateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        var result = await _studentPort.CreateStudentAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetStudent), new { id = result.Id },
            ApiResponseDto<StudentDto>.SuccessResult(result));
    }
}
```

### 7. Pipeline Behavior Pattern

**Định nghĩa**: Cross-cutting concerns như validation, logging được xử lý through MediatR pipeline.

**Ví dụ Validation Behavior**:
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Validate request trước khi xử lý
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

## Đặc Điểm Kiến Trúc

### Điểm Mạnh

- **🎯 Explicit Boundaries**: Ports & Adapters làm rõ ràng boundaries giữa core và external
- **🔄 Technology Independence**: Core logic không biết về HTTP, database hay framework cụ thể
- **🛡️ Business Logic Protection**: Domain rules được bảo vệ tuyệt đối khỏi technical details
- **🧪 Superior Testability**: Mock adapters dễ dàng, test core logic độc lập
- **📈 Extreme Flexibility**: Thay đổi database/UI/framework không ảnh hưởng core
- **🔧 Maintainability**: Clear separation of concerns với ports/adapters pattern
- **🔒 Type Safety**: Strongly-typed identifiers và value objects
- **🎭 AutoMapper Integration**: Automatic object-to-object mapping
- **✅ Validation Pipeline**: Centralized validation with FluentValidation
- **🌐 Framework Agnostic**: Có thể swap ASP.NET Core → gRPC/GraphQL dễ dàng

### So Sánh với Clean Architecture

| Aspect | Clean Architecture | Hexagonal Architecture |
|--------|-------------------|------------------------|
| **Terminology** | Layers (Infrastructure, Application, Domain) | Ports & Adapters |
| **Focus** | Layer dependencies | Data flow direction (in/out) |
| **Interfaces** | Implicit boundaries | Explicit ports |
| **Adapters** | Mixed with infrastructure | Clearly separated as adapters |
| **Clarity** | Good | Excellent (more explicit) |

### Trade-offs

- **📚 More Abstractions**: Nhiều interfaces hơn (ports)
- **⏱️ Initial Complexity**: Setup ban đầu phức tạp hơn Clean Architecture
- **📄 More Files**: Ports + Adapters tách biệt → nhiều files
- **🧠 Steeper Learning Curve**: Cần hiểu Hexagonal concepts
- **🎓 Team Training**: Team cần training về ports/adapters thinking

## Chi Tiết Triển Khai

### Cấu Trúc File Đầy Đủ

```
src/
├── StudentManagement.Domain/           # 🎯 Core Business Logic (The Hexagon)
│   ├── Entities/                      # Domain entities
│   │   ├── Student.cs                 # Student entity với business logic
│   │   ├── Course.cs                  # Course entity với prerequisites
│   │   ├── Enrollment.cs              # Enrollment entity với status
│   │   ├── Grade.cs                   # Grade entity với validation
│   │   └── BaseEntity.cs              # Base entity với audit fields
│   ├── ValueObjects/                  # Value objects
│   │   ├── StudentId.cs               # Strongly-typed student identifier
│   │   ├── CourseCode.cs              # Course code value object
│   │   ├── Email.cs                   # Email value object với validation
│   │   └── GPA.cs                     # GPA value object với constraints
│   ├── Events/                        # Domain events
│   │   ├── IDomainEvent.cs            # Domain event interface
│   │   ├── StudentEnrolledEvent.cs    # Student enrollment event
│   │   ├── GradeAssignedEvent.cs      # Grade assignment event
│   │   └── CourseCompletedEvent.cs    # Course completion event
│   └── Ports/                         # 🔌 SECONDARY PORTS (Outbound)
│       └── IPersistence/              # Persistence port interfaces
│           ├── IPersistencePort.cs    # Base persistence port (generic CRUD)
│           ├── IStudentPersistencePort.cs
│           ├── ICoursePersistencePort.cs
│           ├── IEnrollmentPersistencePort.cs
│           └── IUnitOfWorkPort.cs
│
├── StudentManagement.Application/      # 🔄 Use Cases & Primary Ports
│   ├── Ports/                         # 🔌 PRIMARY PORTS (Inbound)
│   │   ├── IStudentManagementPort.cs  # Student management operations
│   │   ├── ICourseManagementPort.cs   # Course management operations
│   │   └── IEnrollmentManagementPort.cs # Enrollment management operations
│   ├── Commands/                      # Write operations (CQRS)
│   │   ├── Students/                  # Student command handlers
│   │   │   ├── CreateStudentCommand.cs
│   │   │   ├── CreateStudentCommandHandler.cs
│   │   │   ├── UpdateStudentCommand.cs
│   │   │   └── DeleteStudentCommand.cs
│   │   ├── Courses/                   # Course command handlers
│   │   └── Enrollments/               # Enrollment command handlers
│   ├── Queries/                       # Read operations (CQRS)
│   │   ├── Students/                  # Student query handlers
│   │   │   ├── GetStudentsQuery.cs
│   │   │   ├── GetStudentsQueryHandler.cs
│   │   │   └── GetStudentByIdQuery.cs
│   │   ├── Courses/                   # Course query handlers
│   │   └── Enrollments/               # Enrollment query handlers
│   ├── DTOs/                          # Data transfer objects
│   │   ├── StudentDto.cs              # Student response DTO
│   │   ├── CourseDto.cs               # Course response DTO
│   │   ├── EnrollmentDto.cs           # Enrollment response DTO
│   │   ├── ApiResponseDto.cs          # Standard API response wrapper
│   │   └── PagedResultDto.cs          # Pagination response DTO
│   ├── Common/
│   │   └── Behaviors/                 # Cross-cutting concerns
│   │       └── ValidationBehavior.cs  # FluentValidation pipeline behavior
│   ├── Validators/                    # FluentValidation validators
│   │   ├── Students/                  # Student validators
│   │   ├── Courses/                   # Course validators
│   │   └── Enrollments/               # Enrollment validators
│   └── Mappings/                      # AutoMapper profiles
│       ├── StudentMappingProfile.cs   # Student entity-DTO mappings
│       ├── CourseMappingProfile.cs    # Course entity-DTO mappings
│       └── EnrollmentMappingProfile.cs # Enrollment entity-DTO mappings
│
├── StudentManagement.Adapters.Persistence/   # 🔧 SECONDARY ADAPTERS (Driven)
│   ├── Data/                          # EF Core DbContext
│   │   ├── StudentManagementDbContext.cs # Main DbContext
│   │   └── Configurations/           # Entity configurations
│   │       ├── StudentConfiguration.cs
│   │       ├── CourseConfiguration.cs
│   │       ├── EnrollmentConfiguration.cs
│   │       └── GradeConfiguration.cs
│   ├── Repositories/                 # Persistence Adapter implementations
│   │   ├── EfCoreRepositoryBase.cs   # Generic repository base
│   │   ├── EfCoreStudentAdapter.cs   # ← implements IStudentPersistencePort
│   │   ├── EfCoreCourseAdapter.cs    # ← implements ICoursePersistencePort
│   │   ├── EfCoreEnrollmentAdapter.cs # ← implements IEnrollmentPersistencePort
│   │   └── EfCoreUnitOfWorkAdapter.cs # ← implements IUnitOfWorkPort
│   ├── Migrations/                   # Database migrations
│   │   └── 20250929080108_CleanInitialMigration.cs
│   └── DependencyInjection.cs        # Service registration for adapters
│
└── StudentManagement.Adapters.WebApi/        # 🌐 PRIMARY ADAPTERS (Driving)
    ├── Controllers/                   # REST API endpoints (Primary Adapters)
    │   ├── StudentsController.cs     # ← depends on IStudentManagementPort
    │   ├── CoursesController.cs      # ← depends on ICourseManagementPort
    │   ├── EnrollmentsController.cs  # ← depends on IEnrollmentManagementPort
    │   └── HealthController.cs
    ├── ApplicationServices/           # Primary Port implementations
    │   ├── StudentApplicationService.cs    # ← implements IStudentManagementPort
    │   ├── CourseApplicationService.cs     # ← implements ICourseManagementPort
    │   └── EnrollmentApplicationService.cs # ← implements IEnrollmentManagementPort
    ├── Middleware/                    # Custom middleware
    │   └── GlobalExceptionMiddleware.cs # Global exception handling
    ├── Program.cs                     # Application entry point & DI configuration
    ├── DependencyInjection.cs        # Service registration for WebApi
    └── appsettings.json              # Configuration settings
```

### Quan Hệ Chính trong Hexagonal Architecture

1. **HTTP Request** → **Controller** (Primary Adapter)
2. **Controller** → **Primary Port** (IStudentManagementPort)
3. **Primary Port** → **Application Service** → **MediatR**
4. **MediatR** → **Command/Query Handlers**
5. **Handlers** → **Domain Entities** + **Secondary Ports** (IPersistencePort)
6. **Secondary Ports** → **Secondary Adapters** (EfCoreAdapter)
7. **Adapters** → **DbContext** → **Database**

### Request Processing Pipeline (Hexagonal Flow)

```
HTTP Request (External Actor)
     ↓
┌────────────────────────────────────┐
│ PRIMARY ADAPTER                    │
│ StudentsController                 │
│ (Adapters.WebApi/Controllers)     │
└────────────┬───────────────────────┘
             ↓
┌────────────────────────────────────┐
│ PRIMARY PORT                       │
│ IStudentManagementPort             │
│ (Application/Ports)                │
└────────────┬───────────────────────┘
             ↓
┌────────────────────────────────────┐
│ PRIMARY ADAPTER IMPLEMENTATION     │
│ StudentApplicationService          │
│ (Adapters.WebApi/ApplicationServices)│
└────────────┬───────────────────────┘
             ↓
MediatR Send Command/Query
     ↓
ValidationBehavior (FluentValidation)
     ↓
┌────────────────────────────────────┐
│ APPLICATION CORE                   │
│ Command/Query Handler              │
│ (Application/Commands or Queries)  │
└────────────┬───────────────────────┘
             ↓
┌────────────────────────────────────┐
│ DOMAIN LOGIC                       │
│ Business Rules & Domain Entities   │
│ (Domain/Entities)                  │
└────────────┬───────────────────────┘
             ↓
┌────────────────────────────────────┐
│ SECONDARY PORT                     │
│ IStudentPersistencePort            │
│ (Domain/Ports/IPersistence)        │
└────────────┬───────────────────────┘
             ↓
┌────────────────────────────────────┐
│ SECONDARY ADAPTER                  │
│ EfCoreStudentAdapter               │
│ (Adapters.Persistence/Repositories)│
└────────────┬───────────────────────┘
             ↓
DbContext & Entity Framework
     ↓
SQLite Database
     ↓
AutoMapper (Entity → DTO)
     ↓
ApiResponseDto Wrapper
     ↓
JSON Response → HTTP Response
```

### Dependency Injection Setup (Hexagonal Style)

```csharp
// Program.cs - Service Registration theo Hexagonal layers
var builder = WebApplication.CreateBuilder(args);

// Application Core (Use Cases)
builder.Services.AddApplication();
// - MediatR
// - Validators
// - AutoMapper

// Secondary Adapters (Persistence)
builder.Services.AddPersistence(builder.Configuration);
// - DbContext
// - IStudentPersistencePort → EfCoreStudentAdapter
// - ICoursePersistencePort → EfCoreCourseAdapter
// - IUnitOfWorkPort → EfCoreUnitOfWorkAdapter

// Primary Adapters (WebApi)
builder.Services.AddWebApi();
// - Controllers
// - IStudentManagementPort → StudentApplicationService
// - ICourseManagementPort → CourseApplicationService
// - Middleware, Swagger, CORS
```

**Chi tiết DI Registration**:

```csharp
// Adapters.Persistence/DependencyInjection.cs
public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<StudentManagementDbContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

    // Secondary Adapters → Secondary Ports
    services.AddScoped<IStudentPersistencePort, EfCoreStudentAdapter>();
    services.AddScoped<ICoursePersistencePort, EfCoreCourseAdapter>();
    services.AddScoped<IEnrollmentPersistencePort, EfCoreEnrollmentAdapter>();
    services.AddScoped<IUnitOfWorkPort, EfCoreUnitOfWorkAdapter>();

    return services;
}

// Adapters.WebApi/DependencyInjection.cs
public static IServiceCollection AddWebApi(this IServiceCollection services)
{
    services.AddControllers();
    services.AddSwaggerGen();

    // Primary Adapters → Primary Ports
    services.AddScoped<IStudentManagementPort, StudentApplicationService>();
    services.AddScoped<ICourseManagementPort, CourseApplicationService>();
    services.AddScoped<IEnrollmentManagementPort, EnrollmentApplicationService>();

    return services;
}
```

## API Endpoints Đã Triển Khai

### Students API
- `GET /api/students` - Lấy danh sách students với pagination và filtering
- `GET /api/students/{id}` - Lấy student theo ID
- `POST /api/students` - Tạo student mới
- `PUT /api/students/{id}` - Cập nhật student
- `DELETE /api/students/{id}` - Xóa student

### Courses API
- `GET /api/courses` - Lấy danh sách courses với pagination và filtering
- `GET /api/courses/{id}` - Lấy course theo ID
- `POST /api/courses` - Tạo course mới
- `PUT /api/courses/{id}` - Cập nhật course
- `DELETE /api/courses/{id}` - Xóa course

### Enrollments API
- `GET /api/enrollments` - Lấy danh sách enrollments với pagination và filtering
- `GET /api/enrollments/{id}` - Lấy enrollment theo ID
- `POST /api/enrollments` - Tạo enrollment mới
- `POST /api/enrollments/{id}/assign-grade` - Gán grade cho enrollment

## Cross-Cutting Concerns

### 1. Global Exception Handling
```csharp
public class GlobalExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericException(context, ex);
        }
    }
}
```

### 2. Response Compression
- Gzip compression được enable cho tất cả responses
- Giảm bandwidth usage và cải thiện performance

### 3. Swagger Documentation
- Enhanced API documentation với examples
- Comprehensive schema descriptions
- Request/response models documentation

## Database Design

### Entity Relationships
```
Student ||--o{ Enrollment }o--|| Course
Enrollment ||--o| Grade

Student:
- StudentId (PK)
- FirstName, LastName
- Email (Value Object)
- EnrollmentDate
- IsActive

Course:
- CourseId (PK)
- Code (Value Object)
- Name, Description
- CreditHours
- Prerequisites (List<CourseId>)

Enrollment:
- EnrollmentId (PK)
- StudentId (FK)
- CourseId (FK)
- EnrollmentDate
- Status

Grade:
- GradeId (PK)
- EnrollmentId (FK)
- GradeValue
- AssignedDate
```

## Hexagonal Architecture: Lợi Ích Thực Tế

### 1. 🔄 Framework Independence
```csharp
// Có thể thay ASP.NET Core → gRPC
// Chỉ cần tạo new Primary Adapter:
// Adapters.Grpc/GrpcStudentService.cs implements IStudentManagementPort
// Core logic không thay đổi!
```

### 2. 🗄️ Database Independence
```csharp
// Có thể thay SQLite → PostgreSQL hoặc MongoDB
// Chỉ cần tạo new Secondary Adapter:
// Adapters.Persistence.Mongo/MongoStudentAdapter.cs implements IStudentPersistencePort
// Core logic không thay đổi!
```

### 3. 🧪 Testing Independence
```csharp
// Unit test core logic với in-memory adapters
public class InMemoryStudentAdapter : IStudentPersistencePort
{
    private List<Student> _students = new();
    // Mock implementation
}

// Integration test với real database
public class StudentIntegrationTests
{
    [Fact]
    public async Task CreateStudent_ShouldPersistToDatabase()
    {
        // Use real EfCoreStudentAdapter
    }
}
```

## Khuyến Nghị Cải Tiến

### Patterns Có Thể Cải Thiện

1. **🏃 Performance Optimization**
   - Implement repository query optimization at adapter level
   - Add database indexes cho common queries
   - Caching layer as new secondary adapter

2. **🧪 Testing Strategy**
   - Unit tests cho domain logic (isolated from adapters)
   - Adapter tests (test each adapter independently)
   - Integration tests (full hexagon with real adapters)
   - End-to-end tests (complete system)

3. **📊 Observability**
   - Logging adapter (secondary adapter for ILogger port)
   - Metrics adapter (secondary adapter for IMetrics port)
   - Health checks for each adapter

4. **🔒 Security**
   - Authentication adapter (new primary adapter)
   - Authorization policies in application layer
   - Security headers middleware

### Mở Rộng Tiềm Năng

5. **📧 External Services**
```
Adapters.Email/          # New secondary adapter
  ├── SmtpEmailAdapter.cs        # implements IEmailPort
  └── SendGridEmailAdapter.cs    # alternative implementation
```

6. **📦 Event Publishing**
```
Domain/Ports/IMessaging/
  └── IEventPublisherPort.cs

Adapters.Messaging/
  ├── RabbitMqEventAdapter.cs
  └── AzureServiceBusAdapter.cs
```

7. **💾 Caching**
```
Domain/Ports/ICache/
  └── ICachePort.cs

Adapters.Cache/
  ├── RedisCacheAdapter.cs
  └── InMemoryCacheAdapter.cs
```

## Kết Luận

Kiến trúc Hexagonal của Student Management System cung cấp:

- **✅ Explicit Boundaries**: Ports & Adapters pattern làm rõ ràng dependencies
- **✅ Technology Agnostic**: Core logic hoàn toàn độc lập với frameworks
- **✅ Superior Testability**: Mock adapters dễ dàng, test từng component riêng biệt
- **✅ Flexibility**: Thay đổi database/UI/framework không ảnh hưởng core
- **✅ Maintainability**: Clear separation of concerns, dễ hiểu và maintain
- **✅ Scalability**: Dễ mở rộng với adapters mới (email, messaging, cache, etc.)
- **✅ SOLID Principles**: Tuân thủ tất cả SOLID principles
- **✅ DDD Integration**: Hexagonal architecture là perfect fit cho DDD

### Migration từ Clean Architecture

Hệ thống đã được migrate từ Clean Architecture sang Hexagonal Architecture:
- ✅ Repository interfaces → Persistence Ports
- ✅ Infrastructure layer → Adapters.Persistence (Secondary Adapters)
- ✅ WebApi layer → Adapters.WebApi (Primary Adapters)
- ✅ Application Services → Primary Port implementations
- ✅ Explicit port interfaces → Rõ ràng về data flow direction

Hệ thống hiện tại sẵn sàng cho:
- 🚀 Production deployment
- 📈 Horizontal scaling
- 🔧 Easy maintenance và enhancement
- 🧪 Comprehensive testing
- 🌍 Multi-platform support (Web API, gRPC, GraphQL)
