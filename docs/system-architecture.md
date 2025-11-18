# Kiến Trúc Hệ Thống - Student Management System

## 1. Tổng Quan Kiến Trúc

### 1.1 Giới Thiệu
Student Management System được xây dựng theo **Clean Architecture** (còn gọi là Onion Architecture hoặc Hexagonal Architecture) kết hợp với các nguyên tắc **Domain-Driven Design (DDD)**. Kiến trúc này tạo ra một hệ thống:
- Độc lập với frameworks và UI
- Dễ test
- Độc lập với database
- Độc lập với external agencies
- Business rules có thể test mà không cần UI, database, web server, hoặc external elements

### 1.2 Architectural Principles

#### 1.2.1 Dependency Rule
**Nguyên tắc vàng**: Dependencies chỉ được phép trỏ vào trong (inward), không bao giờ ra ngoài (outward)

```
┌───────────────────────────────────────┐
│        WebApi (Presentation)          │
│  ┌─────────────────────────────────┐  │
│  │     Infrastructure (Data)       │  │
│  │  ┌───────────────────────────┐  │  │
│  │  │  Application (Use Cases)  │  │  │
│  │  │  ┌─────────────────────┐  │  │  │
│  │  │  │  Domain (Entities)  │  │  │  │
│  │  │  │                     │  │  │  │
│  │  │  │  - Entities         │  │  │  │
│  │  │  │  - Value Objects    │  │  │  │
│  │  │  │  - Domain Events    │  │  │  │
│  │  │  │  - Interfaces       │  │  │  │
│  │  │  └─────────────────────┘  │  │  │
│  │  │                           │  │  │
│  │  │  - Commands & Queries     │  │  │
│  │  │  - DTOs                   │  │  │
│  │  │  - Validators             │  │  │
│  │  └───────────────────────────┘  │  │
│  │                                 │  │
│  │  - DbContext                    │  │
│  │  - Repositories                 │  │
│  │  - Configurations               │  │
│  └─────────────────────────────────┘  │
│                                       │
│  - Controllers                        │
│  - Middleware                         │
│  - Filters                            │
└───────────────────────────────────────┘

Dependencies flow: Outward → Inward ONLY
```

#### 1.2.2 Separation of Concerns
Mỗi layer có responsibility riêng biệt và không overlap:

**Domain Layer**: Business logic và rules
**Application Layer**: Use cases và orchestration
**Infrastructure Layer**: External concerns (database, file system, etc.)
**Presentation Layer**: UI/API endpoints

#### 1.2.3 Dependency Inversion Principle
Các layer bên ngoài phụ thuộc vào abstractions (interfaces) được định nghĩa ở layer bên trong, không phải concrete implementations.

```csharp
// Domain defines interface
namespace Domain.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(Guid id);
    }
}

// Infrastructure implements
namespace Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        // Implementation details
    }
}

// Application depends on interface
namespace Application.Commands
{
    public class CreateStudentHandler
    {
        private readonly IStudentRepository _repository; // Depends on interface
    }
}
```

## 2. Domain Layer - Trái Tim của Hệ Thống

### 2.1 Trách Nhiệm
Domain layer là **core** của application, chứa:
- Business logic
- Business rules
- Domain models
- Domain events
- Repository interfaces

**Nguyên tắc quan trọng nhất**: Layer này KHÔNG phụ thuộc vào bất cứ thứ gì khác

### 2.2 Entities

#### 2.2.1 BaseEntity Pattern
```csharp
public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected BaseEntity() { }

    protected BaseEntity(TId id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Design Decisions**:
- Generic `TId` type cho flexibility (Guid, int, hoặc strongly-typed IDs)
- Protected setters để enforce encapsulation
- Audit fields (CreatedAt, UpdatedAt) automatic
- Protected constructor cho factory pattern

#### 2.2.2 Student Entity (Aggregate Root)

```
┌─────────────────────────────────────────────┐
│             Student (Aggregate Root)        │
├─────────────────────────────────────────────┤
│ - StudentId Id                              │
│ - string FirstName                          │
│ - string LastName                           │
│ - Email Email (Value Object)                │
│ - DateTime DateOfBirth                      │
│ - DateTime EnrollmentDate                   │
│ - bool IsActive                             │
│ - List<Enrollment> Enrollments              │
├─────────────────────────────────────────────┤
│ + Create() : Student                        │
│ + UpdatePersonalInfo()                      │
│ + AddEnrollment(Enrollment)                 │
│ + CalculateGPA() : GPA                      │
│ + Deactivate()                              │
│ + Reactivate()                              │
└─────────────────────────────────────────────┘
```

**Business Rules Enforced**:
1. **Tên hợp lệ**: 2-50 ký tự, chỉ chữ cái
2. **Email unique**: Checked tại application layer
3. **Tuổi hợp lệ**: 13-120 tuổi
4. **Enrollment không trùng**: Không thể enroll trùng course active
5. **GPA calculation**: Tự động từ completed enrollments

**Encapsulation**:
```csharp
public class Student : BaseEntity<StudentId>
{
    // Private setter - cannot be modified from outside
    public string FirstName { get; private set; } = string.Empty;

    // Private constructor - must use factory method
    private Student(...) { }

    // Public factory method - controlled creation
    public static Student Create(...)
    {
        // Validation happens here
        return new Student(...);
    }

    // Public method to modify - business rules enforced
    public void UpdatePersonalInfo(string firstName, string lastName, Email email)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        Email = email;
        UpdateTimestamp();
    }
}
```

#### 2.2.3 Course Entity

```
┌─────────────────────────────────────────────┐
│                  Course                     │
├─────────────────────────────────────────────┤
│ - Guid Id                                   │
│ - CourseCode Code (Value Object)            │
│ - string Name                               │
│ - string Description                        │
│ - int CreditHours                           │
│ - string Department                         │
│ - int MaxEnrollment                         │
│ - bool IsActive                             │
│ - List<Guid> Prerequisites                  │
│ - List<Enrollment> Enrollments              │
├─────────────────────────────────────────────┤
│ + Create() : Course                         │
│ + UpdateCourseInfo()                        │
│ + AddPrerequisite(Guid)                     │
│ + RemovePrerequisite(Guid)                  │
│ + CanEnroll() : bool                        │
│ + AddEnrollment(Enrollment)                 │
│ + Deactivate()                              │
└─────────────────────────────────────────────┘
```

**Business Rules**:
1. **CourseCode unique**: Format CS101, MATH201
2. **CreditHours**: 1-10
3. **MaxEnrollment**: 1-500
4. **Prerequisites validation**: Không thể là prerequisite của chính nó
5. **Enrollment limit**: Không vượt quá MaxEnrollment

#### 2.2.4 Enrollment Entity

```
┌─────────────────────────────────────────────┐
│                Enrollment                   │
├─────────────────────────────────────────────┤
│ - Guid Id                                   │
│ - StudentId StudentId (FK)                  │
│ - Guid CourseId (FK)                        │
│ - DateTime EnrollmentDate                   │
│ - DateTime? CompletionDate                  │
│ - EnrollmentStatus Status (Enum)            │
│ - int CreditHours                           │
│ - Grade? Grade (nullable)                   │
├─────────────────────────────────────────────┤
│ + Create() : Enrollment                     │
│ + AssignGrade(Grade)                        │
│ + Complete()                                │
│ + Withdraw()                                │
│ + Reactivate()                              │
└─────────────────────────────────────────────┘
```

**State Machine**:
```
┌────────┐   AssignGrade()   ┌──────────┐
│ Active │──────────────────→│ Active   │
│        │                   │(w/ Grade)│
└────────┘                   └──────────┘
    │                             │
    │ Withdraw()                  │ Complete()
    ↓                             ↓
┌──────────┐                 ┌───────────┐
│Withdrawn │                 │ Completed │
└──────────┘                 └───────────┘
    ↑
    │ Reactivate()
    │ (if not completed)
```

**Business Rules**:
1. **Assign grade**: Chỉ khi status = Active
2. **Complete**: Phải có grade
3. **Withdraw**: Không được nếu đã Completed
4. **Reactivate**: Không được nếu đã Completed

### 2.3 Value Objects

#### 2.3.1 Characteristics
Value objects là immutable objects được defined bởi attributes của chúng:

**Properties**:
- Immutable (không thể thay đổi sau khi tạo)
- Equality dựa trên values, không phải identity
- Self-validating (validate trong constructor)
- No identity (không có Id)

#### 2.3.2 Email Value Object

```csharp
public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        Value = ValidateAndFormat(value);
    }

    private static string ValidateAndFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        var formatted = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(formatted))
            throw new ArgumentException("Invalid email format", nameof(value));

        return formatted;
    }

    public override string ToString() => Value;

    // Implicit conversions for convenience
    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);
}
```

**Benefits**:
- Type safety (không thể nhầm Email với string)
- Validation centralized
- Immutability guaranteed
- Domain concept explicit

#### 2.3.3 GPA Value Object

```csharp
public record GPA
{
    public decimal Value { get; }

    public GPA(decimal value)
    {
        if (value < 0 || value > 4.0m)
            throw new ArgumentException("GPA must be between 0 and 4.0");

        Value = Math.Round(value, 2);
    }

    public static implicit operator decimal(GPA gpa) => gpa.Value;
}
```

#### 2.3.4 CourseCode Value Object

```csharp
public record CourseCode
{
    private static readonly Regex CodeRegex = new(
        @"^[A-Z]{2,4}[0-9]{3,4}$",
        RegexOptions.Compiled);

    public string Value { get; }

    public CourseCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Course code cannot be empty");

        var formatted = value.Trim().ToUpperInvariant();

        if (!CodeRegex.IsMatch(formatted))
            throw new ArgumentException("Invalid course code format");

        Value = formatted;
    }

    public static implicit operator string(CourseCode code) => code.Value;
    public static implicit operator CourseCode(string value) => new(value);
}
```

#### 2.3.5 Strongly-Typed IDs

```csharp
public record StudentId
{
    public Guid Value { get; }

    public StudentId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty");

        Value = value;
    }

    public static StudentId New() => new(Guid.NewGuid());

    public static implicit operator Guid(StudentId id) => id.Value;
}
```

**Benefits**:
- Type safety: `void ProcessStudent(StudentId id)` vs `void ProcessStudent(Guid id)`
- Cannot accidentally pass CourseId where StudentId expected
- Clear intent
- Refactoring safe

### 2.4 Domain Events

#### 2.4.1 IDomainEvent Interface

```csharp
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
```

#### 2.4.2 Event Examples

```csharp
public record StudentEnrolledEvent : IDomainEvent
{
    public StudentId StudentId { get; init; }
    public Guid CourseId { get; init; }
    public DateTime EnrollmentDate { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

public record GradeAssignedEvent : IDomainEvent
{
    public Guid EnrollmentId { get; init; }
    public string LetterGrade { get; init; } = string.Empty;
    public decimal GradePoints { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

public record CourseCompletedEvent : IDomainEvent
{
    public Guid EnrollmentId { get; init; }
    public StudentId StudentId { get; init; }
    public Guid CourseId { get; init; }
    public DateTime CompletionDate { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
```

**Purpose**:
- Decouple domain logic
- Enable event-driven architecture
- Audit trail
- Trigger side effects (notifications, logging, etc.)

**Future Integration** (planned):
```csharp
public abstract class BaseEntity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

// In SaveChangesAsync
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
{
    var domainEvents = ChangeTracker.Entries<BaseEntity<Guid>>()
        .SelectMany(e => e.Entity.DomainEvents)
        .ToList();

    var result = await base.SaveChangesAsync(cancellationToken);

    // Dispatch events after successful save
    foreach (var domainEvent in domainEvents)
    {
        await _mediator.Publish(domainEvent, cancellationToken);
    }

    return result;
}
```

### 2.5 Repository Interfaces

#### 2.5.1 Generic Repository

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

#### 2.5.2 Specific Repositories

```csharp
public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByEmailAsync(Email email);
    Task<Student?> GetWithEnrollmentsAsync(StudentId id);
    Task<IEnumerable<Student>> FindAsync(StudentFilterDto filter);
}

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByCodeAsync(CourseCode code);
    Task<Course?> GetWithEnrollmentsAsync(Guid id);
    Task<IEnumerable<Course>> FindAsync(CourseFilterDto filter);
}

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(StudentId studentId);
    Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId);
    Task<IEnumerable<Enrollment>> FindAsync(EnrollmentFilterDto filter);
}
```

#### 2.5.3 Unit of Work

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

**Purpose**:
- Coordinate multiple repository operations
- Ensure transactional consistency
- Single point of commit

## 3. Application Layer - Use Cases

### 3.1 CQRS Pattern

#### 3.1.1 Command Query Separation

```
┌──────────────────────────────────────────────┐
│              Client Request                  │
└──────────────┬───────────────────────────────┘
               │
        Is it modifying data?
               │
        ┌──────┴──────┐
        │             │
       Yes            No
        │             │
        ↓             ↓
  ┌──────────┐  ┌─────────┐
  │ Command  │  │  Query  │
  └──────────┘  └─────────┘
        │             │
        ↓             ↓
  ┌──────────┐  ┌─────────┐
  │ Handler  │  │ Handler │
  └──────────┘  └─────────┘
        │             │
        ↓             ↓
  Write to DB    Read from DB
        │             │
        ↓             ↓
  Return Result  Return Data
```

**Commands**: Modify state
- CreateStudentCommand
- UpdateStudentCommand
- DeleteStudentCommand
- AssignGradeCommand

**Queries**: Read state
- GetStudentsQuery
- GetStudentByIdQuery
- GetCoursesQuery

#### 3.1.2 Command Flow

```
┌─────────────┐
│  Controller │
└──────┬──────┘
       │ 1. DTO
       ↓
┌────────────────────┐
│CreateStudentCommand│
└──────┬─────────────┘
       │ 2. MediatR.Send()
       ↓
┌───────────────────┐
│ ValidationBehavior│ (Pipeline)
└──────┬────────────┘
       │ 3. Validate
       ↓
┌───────────────────────────┐
│CreateStudentCommandHandler│
└──────┬────────────────────┘
       │ 4. Execute
       ↓
┌──────────────┐
│  Repository  │
└──────┬───────┘
       │ 5. Save
       ↓
┌──────────────┐
│   Database   │
└──────┬───────┘
       │ 6. Return
       ↓
┌──────────────┐
│   AutoMapper │
└──────┬───────┘
       │ 7. Map to DTO
       ↓
┌──────────────┐
│   Response   │
└──────────────┘
```

#### 3.1.3 Command Implementation

```csharp
// 1. Command
public record CreateStudentCommand : IRequest<ApiResponseDto<StudentDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
}

// 2. Validator
public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be 2-50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.DateOfBirth)
            .Must(BeValidAge).WithMessage("Student must be 13-120 years old");
    }

    private bool BeValidAge(DateTime dob)
    {
        var age = DateTime.UtcNow.Year - dob.Year;
        return age >= 13 && age <= 120;
    }
}

// 3. Handler
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
        var email = new Email(request.Email);
        var student = Student.Create(
            request.FirstName,
            request.LastName,
            email,
            request.DateOfBirth);

        // 2. Business validation
        var existing = await _repository.GetByEmailAsync(email);
        if (existing != null)
        {
            return ApiResponseDto<StudentDto>.Failure("Email already exists");
        }

        // 3. Save
        await _repository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Map and return
        var dto = _mapper.Map<StudentDto>(student);
        return ApiResponseDto<StudentDto>.Success(dto, "Student created successfully");
    }
}
```

### 3.2 Validation Pipeline

#### 3.2.1 ValidationBehavior

```csharp
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

**Registration**:
```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(assembly);
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    });

    services.AddValidatorsFromAssembly(assembly);

    return services;
}
```

### 3.3 DTOs and Mapping

#### 3.3.1 DTO Types

**1. Entity DTOs** (full representation):
```csharp
public class StudentDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public DateTime EnrollmentDate { get; init; }
    public bool IsActive { get; init; }
    public decimal GPA { get; init; }
}
```

**2. Create DTOs** (input):
```csharp
public class CreateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}
```

**3. Summary DTOs** (list views):
```csharp
public class StudentSummaryDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int EnrollmentCount { get; init; }
}
```

**4. Filter DTOs** (query parameters):
```csharp
public class StudentFilterDto
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

**5. Response DTOs** (API responses):
```csharp
public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponseDto<T> Success(T data, string message = "")
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponseDto<T> Failure(string message, List<string>? errors = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
```

**6. Paged Result DTO**:
```csharp
public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

#### 3.3.2 AutoMapper Profiles

```csharp
public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        // Entity → DTO
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Email,
                      opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.GPA,
                      opt => opt.MapFrom(src => src.CalculateGPA().Value));

        // Entity → Summary DTO
        CreateMap<Student, StudentSummaryDto>()
            .ForMember(dest => dest.FullName,
                      opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.EnrollmentCount,
                      opt => opt.MapFrom(src => src.Enrollments.Count(e => e.IsActive)));

        // Value Objects
        CreateMap<Email, string>()
            .ConvertUsing(src => src.Value);

        CreateMap<StudentId, Guid>()
            .ConvertUsing(src => src.Value);
    }
}
```

## 4. Infrastructure Layer - External Concerns

### 4.1 Database Architecture

#### 4.1.1 Entity Framework Core DbContext

```csharp
public class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(StudentManagementDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity<Guid> entity)
            {
                if (entry.State == EntityState.Added)
                {
                    entity.GetType().GetProperty("CreatedAt")
                        ?.SetValue(entity, DateTime.UtcNow);
                }

                entity.GetType().GetProperty("UpdatedAt")
                    ?.SetValue(entity, DateTime.UtcNow);
            }
        }
    }
}
```

#### 4.1.2 Entity Configurations

```csharp
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Table name
        builder.ToTable("Students");

        // Primary key
        builder.HasKey(s => s.Id);

        // Value object conversion (StudentId)
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new StudentId(value))
            .IsRequired();

        // Owned entity (Email value object)
        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(100);

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        // Properties
        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.DateOfBirth)
            .IsRequired();

        builder.Property(s => s.EnrollmentDate)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit fields
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Relationships
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.LastName);
        builder.HasIndex(s => s.IsActive);
    }
}
```

#### 4.1.3 Database Schema

```sql
-- Students Table
CREATE TABLE Students (
    Id BLOB NOT NULL PRIMARY KEY,  -- Guid stored as BLOB
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    DateOfBirth TEXT NOT NULL,      -- DateTime stored as TEXT
    EnrollmentDate TEXT NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);

-- Courses Table
CREATE TABLE Courses (
    Id BLOB NOT NULL PRIMARY KEY,
    Code TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    Description TEXT,
    CreditHours INTEGER NOT NULL,
    Department TEXT NOT NULL,
    MaxEnrollment INTEGER NOT NULL DEFAULT 30,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);

-- Enrollments Table
CREATE TABLE Enrollments (
    Id BLOB NOT NULL PRIMARY KEY,
    StudentId BLOB NOT NULL,
    CourseId BLOB NOT NULL,
    EnrollmentDate TEXT NOT NULL,
    CompletionDate TEXT,
    Status INTEGER NOT NULL,
    CreditHours INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);

-- Grades Table
CREATE TABLE Grades (
    Id BLOB NOT NULL PRIMARY KEY,
    EnrollmentId BLOB NOT NULL,
    LetterGrade TEXT NOT NULL,
    NumericScore REAL NOT NULL,
    GradePoints REAL NOT NULL,
    Comments TEXT,
    AssignedDate TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(Id) ON DELETE CASCADE
);

-- Indexes
CREATE INDEX IX_Students_LastName ON Students(LastName);
CREATE INDEX IX_Students_IsActive ON Students(IsActive);
CREATE INDEX IX_Students_Email ON Students(Email);
CREATE INDEX IX_Courses_Code ON Courses(Code);
CREATE INDEX IX_Enrollments_StudentId ON Enrollments(StudentId);
CREATE INDEX IX_Enrollments_CourseId ON Enrollments(CourseId);
CREATE INDEX IX_Enrollments_Status ON Enrollments(Status);
```

### 4.2 Repository Pattern Implementation

#### 4.2.1 Base Repository

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly StudentManagementDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(StudentManagementDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
```

#### 4.2.2 StudentRepository

```csharp
public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(StudentManagementDbContext context)
        : base(context)
    {
    }

    public async Task<Student?> GetByEmailAsync(Email email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student?> GetWithEnrollmentsAsync(StudentId id)
    {
        return await _dbSet
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Grade)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Student>> FindAsync(StudentFilterDto filter)
    {
        var query = _dbSet.AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchLower = filter.SearchTerm.ToLower();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(searchLower) ||
                s.LastName.ToLower().Contains(searchLower) ||
                s.Email.Value.ToLower().Contains(searchLower));
        }

        // Active filter
        if (filter.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == filter.IsActive.Value);
        }

        // Pagination
        var students = await query
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return students;
    }
}
```

### 4.3 Unit of Work Pattern

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly StudentManagementDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(StudentManagementDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
```

**Usage**:
```csharp
public class CreateEnrollmentCommandHandler : IRequestHandler<...>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public async Task<...> Handle(...)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Multiple operations
            var student = await _studentRepository.GetByIdAsync(...);
            var course = await _courseRepository.GetByIdAsync(...);
            var enrollment = Enrollment.Create(...);

            student.AddEnrollment(enrollment);
            course.AddEnrollment(enrollment);

            await _enrollmentRepository.AddAsync(enrollment);

            await _unitOfWork.CommitTransactionAsync();

            return ...;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

### 4.4 Migrations

#### 4.4.1 Migration Workflow

```bash
# 1. Add migration
dotnet ef migrations add InitialCreate \
    -p src/StudentManagement.Infrastructure \
    -s src/StudentManagement.WebApi

# 2. Review generated migration
# Check Up() and Down() methods

# 3. Apply migration
dotnet ef database update \
    -p src/StudentManagement.Infrastructure \
    -s src/StudentManagement.WebApi

# 4. Remove migration (if needed, before applying)
dotnet ef migrations remove \
    -p src/StudentManagement.Infrastructure \
    -s src/StudentManagement.WebApi
```

#### 4.4.2 Migration Structure

```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Students",
            columns: table => new
            {
                Id = table.Column<byte[]>(nullable: false),
                FirstName = table.Column<string>(maxLength: 50, nullable: false),
                LastName = table.Column<string>(maxLength: 50, nullable: false),
                Email = table.Column<string>(maxLength: 100, nullable: false),
                DateOfBirth = table.Column<DateTime>(nullable: false),
                EnrollmentDate = table.Column<DateTime>(nullable: false),
                IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                CreatedAt = table.Column<DateTime>(nullable: false),
                UpdatedAt = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Students", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Students_Email",
            table: "Students",
            column: "Email",
            unique: true);

        // ... more tables and indexes
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Students");
        // ... drop other tables
    }
}
```

## 5. Presentation Layer (WebApi)

### 5.1 API Design

#### 5.1.1 RESTful Principles

**Resource-Based URLs**:
```
GET    /api/students           - List students
GET    /api/students/{id}      - Get student
POST   /api/students           - Create student
PUT    /api/students/{id}      - Update student
DELETE /api/students/{id}      - Delete student

GET    /api/courses            - List courses
POST   /api/courses            - Create course
...

GET    /api/enrollments        - List enrollments
POST   /api/enrollments        - Create enrollment
POST   /api/enrollments/{id}/assign-grade  - Assign grade
```

**HTTP Status Codes**:
- `200 OK`: Success
- `201 Created`: Resource created
- `400 Bad Request`: Validation error
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

#### 5.1.2 Controller Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all students with optional filtering and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResultDto<StudentSummaryDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>> GetStudents(
        [FromQuery] StudentFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = GetStudentsQuery.FromDto(filter);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Create a new student
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<StudentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), 400)]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> CreateStudent(
        [FromBody] CreateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = CreateStudentCommand.FromDto(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(
            nameof(GetStudent),
            new { id = result.Data!.Id },
            result);
    }
}
```

### 5.2 Middleware Pipeline

```
┌──────────────────┐
│  HTTP Request    │
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  UseHttpsRedirection
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  UseResponseCompression
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  GlobalExceptionMiddleware
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  UseCors         │
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  UseRouting      │
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  UseAuthorization│ (future)
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  MapControllers  │
└────────┬─────────┘
         │
         ↓
┌──────────────────┐
│  HTTP Response   │
└──────────────────┘
```

#### 5.2.1 Global Exception Middleware

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

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
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
        var response = ApiResponseDto<object>.Failure(
            "Validation failed",
            errors);

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception occurred");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = ApiResponseDto<object>.Failure(
            "An error occurred processing your request");

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

### 5.3 Swagger Configuration

```csharp
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student Management API",
        Version = "v1",
        Description = "Clean Architecture API for Student Management",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@studentmanagement.com"
        }
    });

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Enable annotations
    options.EnableAnnotations();

    // JWT Authentication (future)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});
```

### 5.4 Dependency Injection Configuration

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services by layer
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebApi();

var app = builder.Build();

// Configure middleware pipeline
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

## 6. Cross-Cutting Concerns

### 6.1 Logging (Serilog)

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

### 6.2 Response Compression

```csharp
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = new[]
    {
        "application/json",
        "application/xml",
        "text/plain"
    };
});

services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});
```

### 6.3 CORS

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// In production
services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins("https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

### 6.4 Health Checks

```csharp
services.AddHealthChecks()
    .AddDbContextCheck<StudentManagementDbContext>();

// Usage
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});
```

## 7. Security Architecture (Planned)

### 7.1 JWT Authentication

```csharp
// appsettings.json
{
  "JwtSettings": {
    "Secret": "your-256-bit-secret",
    "Issuer": "StudentManagement",
    "Audience": "StudentManagementUsers",
    "ExpiryMinutes": 60
  }
}

// Configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });
```

### 7.2 Authorization

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteStudent(Guid id)
{
    // Only admins can delete students
}

[Authorize(Roles = "Admin,Teacher")]
[HttpPost("assign-grade")]
public async Task<ActionResult> AssignGrade(...)
{
    // Admins and Teachers can assign grades
}

[Authorize]
[HttpGet]
public async Task<ActionResult> GetStudents()
{
    // Any authenticated user can view students
}
```

## 8. Performance Considerations

### 8.1 Current Implementation

**Strengths**:
- EF Core change tracking optimized
- Async/await throughout
- Response compression enabled
- Connection pooling (EF Core default)

**Weaknesses**:
- In-memory filtering (should be database-level)
- No caching
- No query optimization
- N+1 query potential

### 8.2 Optimization Strategies

#### 8.2.1 Database-Level Filtering

```csharp
// ❌ Current: In-memory
public async Task<IEnumerable<Student>> FindAsync(StudentFilterDto filter)
{
    var allStudents = await _dbSet.ToListAsync(); // Gets all
    return allStudents.Where(s => ...); // Filters in memory
}

// ✅ Optimized: Database-level
public async Task<IEnumerable<Student>> FindAsync(StudentFilterDto filter)
{
    var query = _dbSet.AsQueryable();

    if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
    {
        query = query.Where(s => s.FirstName.Contains(filter.SearchTerm));
    }

    return await query.ToListAsync(); // Filters in database
}
```

#### 8.2.2 Caching Strategy

```csharp
public class GetCoursesQueryHandler : IRequestHandler<...>
{
    private readonly IMemoryCache _cache;

    public async Task<...> Handle(...)
    {
        var cacheKey = $"courses-{filter.PageNumber}-{filter.PageSize}";

        if (!_cache.TryGetValue(cacheKey, out List<CourseDto> courses))
        {
            courses = await _repository.FindAsync(filter);

            _cache.Set(cacheKey, courses, TimeSpan.FromMinutes(5));
        }

        return ApiResponseDto<...>.Success(courses);
    }
}
```

#### 8.2.3 Eager Loading

```csharp
// Prevent N+1 queries
public async Task<Student?> GetWithEnrollmentsAsync(StudentId id)
{
    return await _dbSet
        .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
        .Include(s => s.Enrollments)
            .ThenInclude(e => e.Grade)
        .FirstOrDefaultAsync(s => s.Id == id);
}
```

#### 8.2.4 Projection

```csharp
// Only select needed columns
public async Task<List<StudentSummaryDto>> GetSummariesAsync()
{
    return await _dbSet
        .Select(s => new StudentSummaryDto
        {
            Id = s.Id.Value,
            FullName = $"{s.FirstName} {s.LastName}",
            Email = s.Email.Value,
            IsActive = s.IsActive
        })
        .ToListAsync();
}
```

## 9. Testing Architecture (Planned)

### 9.1 Unit Tests

```
Tests/
├── StudentManagement.Domain.Tests/
│   ├── Entities/
│   │   ├── StudentTests.cs
│   │   ├── CourseTests.cs
│   │   └── EnrollmentTests.cs
│   └── ValueObjects/
│       ├── EmailTests.cs
│       ├── GPATests.cs
│       └── CourseCodeTests.cs
├── StudentManagement.Application.Tests/
│   ├── Commands/
│   │   ├── CreateStudentCommandHandlerTests.cs
│   │   └── ...
│   ├── Queries/
│   │   ├── GetStudentsQueryHandlerTests.cs
│   │   └── ...
│   └── Validators/
│       └── CreateStudentCommandValidatorTests.cs
└── StudentManagement.Integration.Tests/
    ├── Controllers/
    │   ├── StudentsControllerTests.cs
    │   └── CoursesControllerTests.cs
    └── Database/
        └── RepositoryTests.cs
```

### 9.2 Test Doubles

```csharp
// Mock repository for unit tests
public class MockStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new();

    public async Task<Student?> GetByIdAsync(StudentId id)
    {
        return await Task.FromResult(
            _students.FirstOrDefault(s => s.Id == id));
    }

    public async Task AddAsync(Student student)
    {
        _students.Add(student);
        await Task.CompletedTask;
    }

    // ... other methods
}
```

## 10. Deployment Architecture

### 10.1 Development

```
Developer Machine
├── .NET 8.0 SDK
├── SQLite Database (local file)
├── Visual Studio/Rider/VS Code
└── Git
```

### 10.2 Production (Planned)

```
┌─────────────────┐
│  Load Balancer  │
└────────┬────────┘
         │
    ┌────┴────┐
    │         │
┌───▼───┐ ┌──▼────┐
│ API 1 │ │ API 2 │
└───┬───┘ └──┬────┘
    │        │
    └───┬────┘
        │
┌───────▼────────┐
│   SQL Server   │
│   PostgreSQL   │
└────────────────┘
```

### 10.3 Docker Support (Planned)

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/StudentManagement.WebApi/StudentManagement.WebApi.csproj", "StudentManagement.WebApi/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/StudentManagement.WebApi"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentManagement.WebApi.dll"]
```

---

**Document Version**: 1.0
**Last Updated**: 2025-01-17
**Author**: Architecture Team
**Status**: Active
