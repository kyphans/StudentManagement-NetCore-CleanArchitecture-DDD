# Tóm Tắt Codebase - Student Management System

## 1. Tổng Quan

### 1.1 Giới Thiệu
Student Management System là một ứng dụng web API được xây dựng theo kiến trúc Clean Architecture với các nguyên tắc Domain-Driven Design (DDD). Hệ thống được tổ chức thành 4 layers độc lập với dependency flow một chiều từ ngoài vào trong.

### 1.2 Technology Stack
- **.NET 8.0**: Framework chính
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 8.0**: ORM với SQLite provider
- **MediatR 13.0.0**: CQRS pattern implementation
- **AutoMapper 12.0.1**: Object mapping
- **FluentValidation 12.0.0**: Input validation
- **Swagger/OpenAPI**: API documentation
- **Serilog**: Structured logging

### 1.3 Kiến Trúc Tổng Thể

```
┌─────────────────────────────────────────────────┐
│           StudentManagement.WebApi              │
│  - Controllers                                  │
│  - Middleware (Exception Handling)              │
│  - Swagger Configuration                        │
│  - CORS, Compression, Health Checks             │
└───────────────┬─────────────────────────────────┘
                │ depends on
                ↓
┌─────────────────────────────────────────────────┐
│      StudentManagement.Infrastructure           │
│  - DbContext                                    │
│  - Repository Implementations                   │
│  - Entity Configurations                        │
│  - Migrations                                   │
└───────────────┬─────────────────────────────────┘
                │ depends on
                ↓
┌─────────────────────────────────────────────────┐
│       StudentManagement.Application             │
│  - Commands & Command Handlers                  │
│  - Queries & Query Handlers                     │
│  - DTOs                                         │
│  - Validators (FluentValidation)                │
│  - AutoMapper Profiles                          │
│  - Behaviors (Validation Pipeline)              │
└───────────────┬─────────────────────────────────┘
                │ depends on
                ↓
┌─────────────────────────────────────────────────┐
│         StudentManagement.Domain                │
│  - Entities (Student, Course, Enrollment)       │
│  - Value Objects (Email, GPA, CourseCode)       │
│  - Domain Events                                │
│  - Repository Interfaces                        │
│  - NO EXTERNAL DEPENDENCIES                     │
└─────────────────────────────────────────────────┘
```

## 2. Domain Layer

### 2.1 Trách Nhiệm
Domain layer là **trái tim** của ứng dụng, chứa tất cả business logic và domain models. Layer này:
- Hoàn toàn độc lập, không phụ thuộc vào bất kỳ layer nào khác
- Không có external dependencies (không có NuGet packages)
- Định nghĩa các business rules và invariants
- Chứa pure C# code

### 2.2 Entities

#### 2.2.1 BaseEntity<TId>
**Location**: `Domain/Entities/BaseEntity.cs`

**Mục đích**: Abstract base class cho tất cả entities

**Properties**:
```csharp
public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
}
```

**Chức năng**:
- Quản lý identity
- Audit fields (CreatedAt, UpdatedAt)
- Encapsulation (setters là protected/private)

#### 2.2.2 Student
**Location**: `Domain/Entities/Student.cs`

**Aggregate Root**: Student là aggregate root, quản lý enrollments của chính nó

**Key Properties**:
- `StudentId Id`: Strongly-typed ID (value object)
- `FirstName`, `LastName`: String
- `Email`: Value object với validation
- `DateOfBirth`: DateTime
- `EnrollmentDate`: Ngày đăng ký vào hệ thống
- `IsActive`: Trạng thái
- `Enrollments`: Collection của enrollments (read-only)

**Business Methods**:
- `Create()`: Factory method để tạo Student mới
- `UpdatePersonalInfo()`: Cập nhật thông tin cá nhân
- `Deactivate()/Reactivate()`: Quản lý trạng thái
- `AddEnrollment()`: Thêm enrollment với business rules
- `CalculateGPA()`: Tính GPA từ completed enrollments

**Business Rules**:
- Tên phải 2-50 ký tự
- Email phải unique và valid format
- Tuổi phải từ 13-120
- Không thể enroll trùng course đang active

#### 2.2.3 Course
**Location**: `Domain/Entities/Course.cs`

**Key Properties**:
- `Guid Id`: Course identifier
- `CourseCode Code`: Value object (e.g., CS101)
- `Name`: 3-100 ký tự
- `Description`: Mô tả chi tiết
- `CreditHours`: 1-10 tín chỉ
- `Department`: Khoa
- `MaxEnrollment`: Số lượng tối đa (1-500)
- `Prerequisites`: List các course IDs tiên quyết
- `Enrollments`: Collection enrollments

**Business Methods**:
- `Create()`: Factory method
- `UpdateCourseInfo()`: Cập nhật thông tin
- `AddPrerequisite()/RemovePrerequisite()`: Quản lý prerequisites
- `CanEnroll()`: Kiểm tra còn chỗ trống
- `AddEnrollment()`: Thêm enrollment với validation

**Business Rules**:
- CourseCode phải unique
- CreditHours: 1-10
- MaxEnrollment: 1-500
- Course không thể là prerequisite của chính nó
- Không thể enroll khi full hoặc inactive

#### 2.2.4 Enrollment
**Location**: `Domain/Entities/Enrollment.cs`

**Mục đích**: Liên kết Student với Course, tracking progress và grade

**Key Properties**:
- `Guid Id`
- `StudentId`: FK to Student
- `CourseId`: FK to Course
- `EnrollmentDate`: Ngày đăng ký
- `CompletionDate`: Ngày hoàn thành (nullable)
- `Status`: Enum (Active, Completed, Withdrawn)
- `CreditHours`: Copy từ Course
- `Grade`: Value object (nullable)

**Business Methods**:
- `Create()`: Factory method
- `AssignGrade()`: Gán điểm
- `Complete()`: Đánh dấu hoàn thành
- `Withdraw()`: Rút khóa học
- `Reactivate()`: Kích hoạt lại

**Business Rules**:
- Chỉ assign grade cho Active enrollment
- Phải có grade trước khi Complete
- Không thể withdraw enrollment đã Completed
- Không thể reactivate enrollment đã Completed

#### 2.2.5 Grade
**Location**: `Domain/Entities/Grade.cs`

**Value Object Characteristics**: Grade là value object

**Key Properties**:
- `Guid Id`
- `LetterGrade`: String (A, A-, B+, etc.)
- `NumericScore`: Decimal (0-100)
- `GradePoints`: Decimal (0-4.0)
- `Comments`: String

**Grade Points Mapping**:
```
A   = 4.0    B+  = 3.3    C+  = 2.3    D+  = 1.3
A-  = 3.7    B   = 3.0    C   = 2.0    D   = 1.0
             B-  = 2.7    C-  = 1.7    F   = 0.0
```

### 2.3 Value Objects

#### 2.3.1 Email
**Location**: `Domain/ValueObjects/Email.cs`

**Mục đích**: Encapsulate email logic và validation

**Characteristics**:
- Immutable (record type)
- Self-validating
- Regex validation
- Auto-normalize (lowercase, trim)

```csharp
public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        Value = ValidateAndFormat(value);
    }
}
```

#### 2.3.2 CourseCode
**Location**: `Domain/ValueObjects/CourseCode.cs`

**Mục đích**: Validate và format course codes

**Format**: Letters + Numbers (e.g., CS101, MATH201)

**Validation**:
- Pattern: `^[A-Z]{2,4}[0-9]{3,4}$`
- Auto-uppercase

#### 2.3.3 GPA
**Location**: `Domain/ValueObjects/GPA.cs`

**Mục đích**: Represent GPA value với constraints

**Validation**:
- Value: 0.0 - 4.0
- Precision: 2 decimal places

#### 2.3.4 Strongly-Typed IDs
**Location**: `Domain/ValueObjects/`

**Files**:
- `StudentId.cs`: Wrapper around Guid cho Student
- `CourseId.cs`: Wrapper around Guid cho Course
- `EnrollmentId.cs`: Wrapper around Guid cho Enrollment
- `GradeId.cs`: Wrapper around Guid cho Grade

**Benefits**:
- Type safety (không thể nhầm StudentId với CourseId)
- Clear intent
- Refactoring safe

### 2.4 Domain Events

**Location**: `Domain/Events/`

**Interface**: `IDomainEvent` - Marker interface

**Implementations**:
1. **StudentEnrolledEvent**: Raise khi student enroll vào course
2. **GradeAssignedEvent**: Raise khi grade được assign
3. **CourseCompletedEvent**: Raise khi enrollment completed

**Purpose**: Decouple domain logic, enable event-driven architecture

### 2.5 Repository Interfaces

**Location**: `Domain/Repositories/`

**Pattern**: Repository pattern interfaces

**Files**:
- `IRepository<T>`: Generic base repository
- `IStudentRepository`: Student-specific operations
- `ICourseRepository`: Course-specific operations
- `IEnrollmentRepository`: Enrollment-specific operations
- `IUnitOfWork`: Transaction management

**Design Philosophy**:
- Interfaces ở Domain layer
- Implementations ở Infrastructure layer
- Dependency Inversion Principle

## 3. Application Layer

### 3.1 Trách Nhiệm
Application layer orchestrates business logic và use cases:
- Định nghĩa use cases (Commands và Queries)
- Coordinate repositories và domain objects
- Transform entities sang DTOs
- Validate inputs
- **Không chứa business logic** (đó là việc của Domain)

### 3.2 CQRS Pattern

#### 3.2.1 Commands (Write Operations)
**Location**: `Application/Commands/`

**Structure**: Theo entity (Students/, Courses/, Enrollments/)

**Pattern**:
```
CreateStudentCommand.cs          (Command class)
CreateStudentCommandHandler.cs   (Handler class)
```

**Example - CreateStudentCommand**:
```csharp
public record CreateStudentCommand : IRequest<ApiResponseDto<StudentDto>>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public DateTime DateOfBirth { get; init; }
}
```

**Commands Available**:

**Students**:
- `CreateStudentCommand` → Tạo student mới
- `UpdateStudentCommand` → Cập nhật student
- `DeleteStudentCommand` → Soft delete student

**Courses**:
- `CreateCourseCommand` → Tạo course mới
- `UpdateCourseCommand` → Cập nhật course
- `DeleteCourseCommand` → Soft delete course

**Enrollments**:
- `CreateEnrollmentCommand` → Đăng ký course
- `AssignGradeCommand` → Chấm điểm

#### 3.2.2 Queries (Read Operations)
**Location**: `Application/Queries/`

**Pattern**:
```
GetStudentsQuery.cs              (Query class)
GetStudentsQueryHandler.cs       (Handler class)
```

**Queries Available**:

**Students**:
- `GetStudentsQuery` → List students với filtering/pagination
- `GetStudentByIdQuery` → Get student by ID

**Courses**:
- `GetCoursesQuery` → List courses với filtering/pagination
- `GetCourseByIdQuery` → Get course by ID

**Enrollments**:
- `GetEnrollmentsQuery` → List enrollments với filtering
- `GetEnrollmentByIdQuery` → Get enrollment by ID

#### 3.2.3 Handlers
**Responsibility**: Mỗi Command/Query có một Handler riêng

**Handler Pattern**:
```csharp
public class CreateStudentCommandHandler
    : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public async Task<ApiResponseDto<StudentDto>> Handle(
        CreateStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create domain entity
        var student = Student.Create(...);

        // 2. Save to repository
        await _repository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();

        // 3. Map to DTO
        var dto = _mapper.Map<StudentDto>(student);

        // 4. Return response
        return ApiResponseDto<StudentDto>.Success(dto);
    }
}
```

### 3.3 DTOs (Data Transfer Objects)

**Location**: `Application/DTOs/`

**Files**:
- `StudentDtos.cs`: StudentDto, CreateStudentDto, UpdateStudentDto, StudentSummaryDto, StudentFilterDto
- `CourseDtos.cs`: CourseDto, CreateCourseDto, UpdateCourseDto, CourseSummaryDto, CourseFilterDto
- `EnrollmentDtos.cs`: EnrollmentDto, CreateEnrollmentDto, AssignGradeDto, EnrollmentFilterDto
- `GradeDtos.cs`: GradeDto
- `CommonDtos.cs`: ApiResponseDto<T>, PagedResultDto<T>, ValidationError

**Purpose**:
- Decouple API contracts từ domain models
- Control data exposure
- Version API without breaking domain

**Example - ApiResponseDto**:
```csharp
public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### 3.4 Validators

**Location**: `Application/Validators/`

**Framework**: FluentValidation

**Structure**: Theo entity và command

**Example - CreateStudentCommandValidator**:
```csharp
public class CreateStudentCommandValidator
    : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be 2-50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.DateOfBirth)
            .Must(BeValidAge).WithMessage("Student must be between 13 and 120 years old");
    }
}
```

**Validators Available**:
- Students: Create, Update, Delete validators
- Courses: Create, Update, Delete validators
- Enrollments: Create, AssignGrade validators

### 3.5 AutoMapper Profiles

**Location**: `Application/Mappings/`

**Files**:
- `StudentMappingProfile.cs`
- `CourseMappingProfile.cs`
- `EnrollmentMappingProfile.cs`
- `GradeMappingProfile.cs`

**Purpose**: Define entity-to-DTO mapping rules

**Example**:
```csharp
public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Email,
                      opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.GPA,
                      opt => opt.MapFrom(src => src.CalculateGPA().Value));
    }
}
```

### 3.6 Behaviors (MediatR Pipeline)

**Location**: `Application/Common/Behaviors/`

**File**: `ValidationBehavior.cs`

**Purpose**: Intercept requests để run validation trước khi handler execute

**Flow**:
```
Request → ValidationBehavior → Handler → Response
          ↓ (if validation fails)
          ValidationException
```

**Code**:
```csharp
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(...)
    {
        if (!_validators.Any()) return await next();

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

### 3.7 Dependency Injection

**File**: `Application/DependencyInjection.cs`

**Method**: `AddApplication(this IServiceCollection services)`

**Registers**:
- MediatR với all handlers từ assembly
- ValidationBehavior vào MediatR pipeline
- FluentValidation validators
- AutoMapper profiles

## 4. Infrastructure Layer

### 4.1 Trách Nhiệm
Infrastructure layer chứa tất cả implementation details:
- Database access (EF Core)
- Repository implementations
- External service integrations
- Data persistence

### 4.2 Database Context

**File**: `Infrastructure/Data/StudentManagementDbContext.cs`

**Inherits**: `DbContext`

**DbSets**:
```csharp
public DbSet<Student> Students { get; set; }
public DbSet<Course> Courses { get; set; }
public DbSet<Enrollment> Enrollments { get; set; }
public DbSet<Grade> Grades { get; set; }
```

**Configuration**:
- Apply configurations từ `IEntityTypeConfiguration`
- Automatic SaveChanges với timestamp updates
- Query filters (có thể dùng cho soft delete)

### 4.3 Entity Configurations

**Location**: `Infrastructure/Data/Configurations/`

**Pattern**: Fluent API configuration, một file per entity

**Files**:
- `StudentConfiguration.cs`
- `CourseConfiguration.cs`
- `EnrollmentConfiguration.cs`
- `GradeConfiguration.cs`

**Example - StudentConfiguration**:
```csharp
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        // Value object conversion
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new StudentId(value));

        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(100);
        });

        // Index for performance
        builder.HasIndex(s => s.Email)
            .IsUnique();

        // Relationships
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Key Configuration Aspects**:
- Value object conversions
- Constraints (required, max length)
- Indexes (unique, performance)
- Relationships (one-to-many, foreign keys)
- Cascade behaviors

### 4.4 Repository Implementations

**Location**: `Infrastructure/Repositories/`

**Pattern**: Concrete implementations của interfaces từ Domain

**Files**:
- `Repository.cs`: Generic base repository
- `StudentRepository.cs`: Student-specific
- `CourseRepository.cs`: Course-specific
- `EnrollmentRepository.cs`: Enrollment-specific
- `UnitOfWork.cs`: Transaction coordinator

**Example - Repository<T>**:
```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly StudentManagementDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
```

**StudentRepository Specific Methods**:
- `GetByEmailAsync(Email email)`
- `GetWithEnrollmentsAsync(StudentId id)`
- `FindAsync(StudentFilterDto filter)` - With filtering logic

### 4.5 Unit of Work

**File**: `Infrastructure/Repositories/UnitOfWork.cs`

**Purpose**: Coordinate multiple repository operations trong một transaction

**Interface**:
```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### 4.6 Migrations

**Location**: `Infrastructure/Migrations/`

**Current Migration**: `20250929080108_CleanInitialMigration`

**Purpose**: Version control cho database schema

**Commands**:
```bash
# Add migration
dotnet ef migrations add <Name> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Apply migration
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

### 4.7 Dependency Injection

**File**: `Infrastructure/DependencyInjection.cs`

**Method**: `AddInfrastructure(this IServiceCollection services, IConfiguration configuration)`

**Registers**:
- DbContext với SQLite connection
- Repository implementations
- Unit of Work

## 5. WebApi Layer

### 5.1 Trách Nhiệm
WebApi layer là presentation layer:
- HTTP endpoints (Controllers)
- Request/Response handling
- Middleware
- API documentation
- CORS, Compression, Health checks

### 5.2 Controllers

**Location**: `WebApi/Controllers/`

**Base**: `ControllerBase` hoặc custom `BaseApiController`

**Files**:
- `StudentsController.cs`
- `CoursesController.cs`
- `EnrollmentsController.cs`
- `HealthController.cs`

**Pattern**: Thin controllers, delegate to MediatR

**Example - StudentsController**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>>
        GetStudents([FromQuery] StudentFilterDto filter)
    {
        var query = GetStudentsQuery.FromDto(filter);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>>
        CreateStudent([FromBody] CreateStudentDto dto)
    {
        var command = CreateStudentCommand.FromDto(dto);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetStudent),
                              new { id = result.Data!.Id },
                              result);
    }
}
```

**Endpoints Summary**:

**Students**:
- `GET /api/students` - List với filtering
- `GET /api/students/{id}` - Get by ID
- `POST /api/students` - Create
- `PUT /api/students/{id}` - Update
- `DELETE /api/students/{id}` - Soft delete

**Courses**:
- `GET /api/courses` - List với filtering
- `GET /api/courses/{id}` - Get by ID
- `POST /api/courses` - Create
- `PUT /api/courses/{id}` - Update
- `DELETE /api/courses/{id}` - Soft delete

**Enrollments**:
- `GET /api/enrollments` - List với filtering
- `GET /api/enrollments/{id}` - Get by ID
- `POST /api/enrollments` - Create enrollment
- `POST /api/enrollments/{id}/assign-grade` - Assign grade

**Health**:
- `GET /health` - Health check endpoint

### 5.3 Middleware

**Location**: `WebApi/Middleware/`

**File**: `GlobalExceptionMiddleware.cs`

**Purpose**: Catch tất cả unhandled exceptions và format response

**Exception Handling**:
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
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

**Response Format**:
- ValidationException → 400 Bad Request
- NotFoundException → 404 Not Found
- Other exceptions → 500 Internal Server Error

### 5.4 Program.cs

**File**: `WebApi/Program.cs`

**Purpose**: Application entry point và configuration

**Configuration Flow**:
```csharp
// 1. Build services
var builder = WebApplication.CreateBuilder(args);
services.AddApplication();           // Application layer
services.AddInfrastructure(config);  // Infrastructure layer
services.AddWebApi();                // WebApi layer

// 2. Build app
var app = builder.Build();

// 3. Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowAll");
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
```

### 5.5 Swagger Configuration

**Location**: `WebApi/DependencyInjection.cs`

**Features**:
- OpenAPI 3.0 documentation
- Annotations support
- XML comments (nếu enabled)
- Interactive testing UI

**Swagger Endpoint**: `/swagger` (development only)

### 5.6 Additional Features

**Response Compression**:
- Gzip compression
- Enabled cho HTTPS
- Optimal compression level
- Applies to JSON, XML, text

**CORS**:
- Policy name: "AllowAll"
- Development: Allow all origins
- Production: Configure appropriately

**Health Checks**:
- Endpoint: `/health`
- Can extend với database checks, etc.

**Memory Cache**:
- Registered but not yet used
- Available cho future optimization

### 5.7 Dependency Injection

**File**: `WebApi/DependencyInjection.cs`

**Method**: `AddWebApi(this IServiceCollection services)`

**Registers**:
- Controllers
- Swagger/OpenAPI
- Response compression
- Memory cache
- Health checks
- CORS policies

## 6. Cross-Cutting Concerns

### 6.1 Dependency Flow

**Rule**: Dependencies chỉ flow một chiều từ ngoài vào trong

```
WebApi → Infrastructure → Application → Domain
  ✓          ✓              ✓          (no deps)
```

**Prohibited**:
- Domain → Application ❌
- Application → Infrastructure ❌
- Domain → Infrastructure ❌

### 6.2 Configuration Management

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=studentmanagement.db"
  },
  "JwtSettings": {
    "Secret": "...",
    "Issuer": "StudentManagement",
    "Audience": "StudentManagementUsers",
    "ExpiryMinutes": 60
  },
  "Logging": { ... }
}
```

**Environment-specific**: `appsettings.Development.json`

### 6.3 Error Handling Strategy

**Layers**:
1. **Domain**: Throw domain exceptions (ArgumentException, InvalidOperationException)
2. **Application**: ValidationException từ FluentValidation
3. **Infrastructure**: Database exceptions
4. **WebApi**: GlobalExceptionMiddleware catches all

**Response Format**: Consistent ApiResponseDto

### 6.4 Logging

**Framework**: Serilog

**Configuration**: In `Program.cs`

**Levels**:
- Information: General flow
- Warning: Unexpected situations
- Error: Exceptions
- Debug: Development only

### 6.5 Testing Strategy (Planned)

**Unit Tests**:
- Domain entities business logic
- Value objects validation
- Handlers logic

**Integration Tests**:
- API endpoints
- Database operations
- End-to-end scenarios

**Test Projects** (to be created):
- `StudentManagement.Domain.Tests`
- `StudentManagement.Application.Tests`
- `StudentManagement.Integration.Tests`

## 7. Database

### 7.1 Provider
**SQLite**: File-based database

**Location**: `src/StudentManagement.WebApi/bin/Debug/net8.0/studentmanagement.db`

**Connection String**: `Data Source=studentmanagement.db`

### 7.2 Schema

**Tables**:
- `Students`: Student records
- `Courses`: Course catalog
- `Enrollments`: Student-Course associations
- `Grades`: Grade records

**Relationships**:
- Student 1→N Enrollments
- Course 1→N Enrollments
- Enrollment 1→1 Grade (optional)

### 7.3 Indexes

**Performance Indexes**:
- `Students.Email` (unique)
- `Courses.Code` (unique)
- Foreign keys automatically indexed

### 7.4 Migrations

**Current**: `CleanInitialMigration`

**Workflow**:
1. Modify entity
2. Update configuration
3. Add migration
4. Review generated code
5. Apply to database

## 8. Key Patterns & Practices

### 8.1 SOLID Principles

**Single Responsibility**: Mỗi class có một responsibility duy nhất

**Open/Closed**: Open for extension, closed for modification

**Liskov Substitution**: Derived classes substitutable cho base classes

**Interface Segregation**: Specific interfaces thay vì fat interfaces

**Dependency Inversion**: Depend on abstractions, not concretions

### 8.2 DDD Patterns

**Entities**: Objects với identity

**Value Objects**: Objects defined by values

**Aggregates**: Cluster of objects treated as a unit

**Repositories**: Abstract data access

**Domain Events**: Capture domain occurrences

**Factory Methods**: Static Create() methods

### 8.3 CQRS Pattern

**Separation**: Commands vs Queries

**Benefits**:
- Clear intent
- Optimized reads vs writes
- Scalability
- Testability

### 8.4 Repository Pattern

**Abstraction**: IRepository<T> interfaces

**Benefits**:
- Testability (mocking)
- Swappable implementations
- Centralized data access logic

### 8.5 Unit of Work Pattern

**Purpose**: Coordinate multiple repository operations

**Benefits**:
- Transaction management
- Consistent state
- Atomic operations

## 9. Important Conventions

### 9.1 Naming

**Entities**: PascalCase singular (Student, Course)

**DTOs**: EntityNameDto (StudentDto, CreateStudentDto)

**Commands**: VerbEntityCommand (CreateStudentCommand)

**Queries**: GetEntityQuery (GetStudentsQuery)

**Handlers**: CommandNameHandler (CreateStudentCommandHandler)

**Validators**: CommandNameValidator (CreateStudentCommandValidator)

### 9.2 File Organization

**By Feature**: Commands và Queries organized by entity

**By Type**: Controllers, Validators, Mappings organized by type

**Separation**: Interface và implementation ở different layers

### 9.3 Access Modifiers

**Entities**:
- Properties: `private set` or `protected set`
- Constructors: `private` hoặc `protected` (use factory methods)

**Value Objects**: `public` properties, `init` setters

**DTOs**: `public` properties, `init` setters

### 9.4 Async/Await

**Convention**: All I/O operations async

**Pattern**: `...Async` suffix

**CancellationToken**: Always include parameter

## 10. Opportunities for Improvement

### 10.1 Performance

**Current Limitations**:
- In-memory filtering (should be database-level)
- No caching
- No query optimization

**Recommendations**:
- Move filtering to LINQ queries
- Add response caching
- Implement Redis caching
- Add database indexes

### 10.2 Security

**Missing**:
- Authentication
- Authorization
- Input sanitization
- Rate limiting

**Recommendations**:
- Implement JWT authentication
- Add role-based authorization
- Add anti-forgery tokens
- Implement rate limiting

### 10.3 Testing

**Missing**:
- Unit tests
- Integration tests
- E2E tests

**Recommendations**:
- Add xUnit test projects
- Mock repositories cho unit tests
- WebApplicationFactory cho integration tests
- Add test coverage reporting

### 10.4 Documentation

**Current**:
- ✅ Swagger documentation
- ✅ XML comments trong code
- ✅ README.md

**Recommendations**:
- Add architecture diagrams
- Add sequence diagrams
- Add API usage examples
- Create wiki

### 10.5 DevOps

**Missing**:
- CI/CD pipeline
- Docker support
- Kubernetes manifests
- Monitoring

**Recommendations**:
- Add GitHub Actions
- Create Dockerfile
- Add docker-compose
- Implement Application Insights

---

**Document Version**: 1.0
**Last Updated**: 2025-01-17
**Status**: Active
