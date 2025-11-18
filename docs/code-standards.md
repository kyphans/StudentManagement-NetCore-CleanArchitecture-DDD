# Chuẩn Mã và Quy Ước Dự Án - Student Management System

## 1. Tổng Quan

Tài liệu này định nghĩa các coding standards, naming conventions và best practices cho Student Management System. Tuân thủ các quy tắc này đảm bảo codebase nhất quán, dễ maintain và dễ mở rộng.

## 2. Kiến Trúc và Cấu Trúc Dự Án

### 2.1 Clean Architecture Layers

#### 2.1.1 Domain Layer (Core)
**Location**: `src/StudentManagement.Domain/`

**Quy tắc nghiêm ngặt**:
- ✅ **KHÔNG** được phụ thuộc vào bất kỳ layer nào khác
- ✅ **KHÔNG** được có external dependencies (NuGet packages)
- ✅ **CHỈ** chứa pure C# code
- ✅ **CHỈ** chứa business logic và domain models
- ✅ **CHỈ** reference System namespaces

**Cấu trúc thư mục**:
```
Domain/
├── Entities/           # Domain entities (Student, Course, etc.)
├── ValueObjects/       # Value objects (Email, GPA, etc.)
├── Events/             # Domain events
└── Repositories/       # Repository interfaces
```

**Allowed**:
```csharp
// ✅ Domain entity
public class Student : BaseEntity<StudentId>
{
    public string FirstName { get; private set; }
    // Business logic
    public GPA CalculateGPA() { ... }
}
```

**Not Allowed**:
```csharp
// ❌ NO external dependencies
using Microsoft.EntityFrameworkCore;
using AutoMapper;

// ❌ NO infrastructure concerns
public class Student
{
    [Required]  // ❌ NO data annotations
    public string FirstName { get; set; }
}
```

#### 2.1.2 Application Layer
**Location**: `src/StudentManagement.Application/`

**Quy tắc**:
- ✅ **CHỈ** phụ thuộc vào Domain layer
- ✅ Chứa use cases (Commands, Queries)
- ✅ Orchestrate domain logic
- ✅ **KHÔNG** chứa business logic (đó là của Domain)

**Cấu trúc thư mục**:
```
Application/
├── Commands/
│   ├── Students/       # CreateStudentCommand, UpdateStudentCommand
│   ├── Courses/
│   └── Enrollments/
├── Queries/
│   ├── Students/       # GetStudentsQuery, GetStudentByIdQuery
│   ├── Courses/
│   └── Enrollments/
├── DTOs/               # Data Transfer Objects
├── Validators/         # FluentValidation validators
├── Mappings/           # AutoMapper profiles
└── Common/
    └── Behaviors/      # MediatR pipeline behaviors
```

**Dependencies**:
- MediatR
- AutoMapper
- FluentValidation
- Microsoft.Extensions.DependencyInjection.Abstractions

#### 2.1.3 Infrastructure Layer
**Location**: `src/StudentManagement.Infrastructure/`

**Quy tắc**:
- ✅ Phụ thuộc vào Domain và Application
- ✅ Implement repository interfaces từ Domain
- ✅ Chứa tất cả infrastructure concerns (DB, external services)

**Cấu trúc thư mục**:
```
Infrastructure/
├── Data/
│   ├── StudentManagementDbContext.cs
│   └── Configurations/  # EF Core entity configurations
├── Repositories/        # Repository implementations
└── Migrations/          # EF Core migrations
```

**Dependencies**:
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design

#### 2.1.4 WebApi Layer
**Location**: `src/StudentManagement.WebApi/`

**Quy tắc**:
- ✅ Phụ thuộc vào Application và Infrastructure
- ✅ Thin controllers
- ✅ Delegate business logic sang Application layer

**Cấu trúc thư mục**:
```
WebApi/
├── Controllers/        # API controllers
├── Middleware/         # Custom middleware
├── Program.cs          # Entry point
├── DependencyInjection.cs
└── appsettings.json
```

### 2.2 Dependency Flow

**Rule**: Dependencies flow inward only

```
┌─────────┐
│ WebApi  │────┐
└─────────┘    │
               ↓
┌──────────────┐
│Infrastructure│────┐
└──────────────┘    │
                    ↓
┌─────────────┐
│ Application │────┐
└─────────────┘    │
                   ↓
┌─────────┐
│ Domain  │ (no dependencies)
└─────────┘
```

**Violation Example** (❌ KHÔNG được phép):
```csharp
// ❌ Domain referencing Application
namespace StudentManagement.Domain
{
    using StudentManagement.Application.DTOs; // ❌ WRONG!
}

// ❌ Application referencing Infrastructure
namespace StudentManagement.Application
{
    using StudentManagement.Infrastructure.Data; // ❌ WRONG!
}
```

## 3. Naming Conventions

### 3.1 General Rules

**PascalCase**:
- Classes, interfaces, enums
- Methods, properties
- Namespaces
- Public members

**camelCase**:
- Private fields
- Parameters
- Local variables

**UPPER_CASE**:
- Constants

### 3.2 Specific Conventions

#### 3.2.1 Entities
```csharp
// ✅ Singular, PascalCase
public class Student { }
public class Course { }
public class Enrollment { }

// ❌ Wrong
public class Students { }      // Plural
public class student { }       // camelCase
public class STUDENT { }       // UPPER_CASE
```

#### 3.2.2 Interfaces
```csharp
// ✅ Start with 'I'
public interface IStudentRepository { }
public interface IUnitOfWork { }

// ❌ Wrong
public interface StudentRepository { }  // Missing 'I'
```

#### 3.2.3 DTOs
```csharp
// ✅ EntityName + Dto suffix
public class StudentDto { }
public class CreateStudentDto { }
public class UpdateStudentDto { }
public class StudentSummaryDto { }
public class StudentFilterDto { }

// ❌ Wrong
public class Student_DTO { }           // Underscore
public class studentDTO { }            // camelCase
public class DtoStudent { }            // Prefix instead of suffix
```

#### 3.2.4 Commands
```csharp
// ✅ Verb + Entity + Command
public record CreateStudentCommand : IRequest<...> { }
public record UpdateStudentCommand : IRequest<...> { }
public record DeleteStudentCommand : IRequest<...> { }
public record AssignGradeCommand : IRequest<...> { }

// ❌ Wrong
public record StudentCreate { }        // Wrong order
public record CreateStudent { }        // Missing Command suffix
```

#### 3.2.5 Queries
```csharp
// ✅ Get + Entity + Query
public record GetStudentsQuery : IRequest<...> { }
public record GetStudentByIdQuery : IRequest<...> { }

// ❌ Wrong
public record StudentQuery { }         // Not descriptive
public record QueryStudents { }        // Wrong order
```

#### 3.2.6 Handlers
```csharp
// ✅ CommandName/QueryName + Handler
public class CreateStudentCommandHandler : IRequestHandler<...> { }
public class GetStudentsQueryHandler : IRequestHandler<...> { }

// ❌ Wrong
public class StudentCommandHandler { } // Not specific
public class HandlerCreateStudent { } // Wrong order
```

#### 3.2.7 Validators
```csharp
// ✅ CommandName + Validator
public class CreateStudentCommandValidator : AbstractValidator<...> { }
public class UpdateCourseCommandValidator : AbstractValidator<...> { }

// ❌ Wrong
public class StudentValidator { }     // Not specific enough
public class ValidatorCreateStudent { } // Wrong order
```

#### 3.2.8 Value Objects
```csharp
// ✅ Descriptive nouns
public record Email { }
public record GPA { }
public record CourseCode { }
public record StudentId { }

// ❌ Wrong
public record EmailValue { }           // Redundant 'Value'
public record email { }                // camelCase
```

#### 3.2.9 Domain Events
```csharp
// ✅ Past tense + Event suffix
public record StudentEnrolledEvent : IDomainEvent { }
public record GradeAssignedEvent : IDomainEvent { }
public record CourseCompletedEvent : IDomainEvent { }

// ❌ Wrong
public record EnrollStudent { }        // Present tense
public record StudentEnroll { }        // Missing Event suffix
```

#### 3.2.10 Private Fields
```csharp
// ✅ Underscore prefix + camelCase
public class Student
{
    private readonly List<Enrollment> _enrollments;
    private string _firstName;
}

// ❌ Wrong
private List<Enrollment> enrollments;  // Missing underscore
private List<Enrollment> Enrollments;  // PascalCase
```

#### 3.2.11 Async Methods
```csharp
// ✅ Async suffix
public async Task<Student> GetStudentAsync(Guid id) { }
public async Task SaveChangesAsync() { }

// ❌ Wrong
public async Task<Student> GetStudent(Guid id) { } // Missing Async suffix
```

### 3.3 File Naming

**Rule**: File name = Class name

```
✅ Student.cs            → class Student
✅ IStudentRepository.cs → interface IStudentRepository
✅ StudentDto.cs         → class StudentDto

❌ student.cs
❌ Student_Entity.cs
❌ student-entity.cs
```

**Multiple classes in one file** (only for closely related types):
```csharp
// StudentDtos.cs
public class StudentDto { }
public class CreateStudentDto { }
public class UpdateStudentDto { }
public class StudentSummaryDto { }
public class StudentFilterDto { }
```

## 4. Code Organization

### 4.1 Class Structure Order

```csharp
public class Student
{
    // 1. Constants
    private const int MaxNameLength = 50;

    // 2. Static fields
    private static readonly Regex EmailRegex = ...;

    // 3. Private fields
    private readonly List<Enrollment> _enrollments = new();
    private string _firstName;

    // 4. Public properties
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    // 5. Computed properties
    public string FullName => $"{FirstName} {LastName}";
    public int Age => CalculateAge();

    // 6. Constructors (protected/private for entities)
    protected Student() { }
    private Student(...) { }

    // 7. Factory methods (static)
    public static Student Create(...) { }

    // 8. Public methods
    public void UpdatePersonalInfo(...) { }
    public GPA CalculateGPA() { }

    // 9. Private methods
    private static string ValidateName(string name) { }
    private int CalculateAge() { }
}
```

### 4.2 Method Organization

```csharp
// ✅ Good: Small, focused methods
public class Student
{
    public void UpdatePersonalInfo(string firstName, string lastName, Email email)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        Email = email;
        UpdateTimestamp();
    }

    private static string ValidateName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", paramName);

        var trimmed = name.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 50)
            throw new ArgumentException("Name must be 2-50 characters", paramName);

        return trimmed;
    }
}

// ❌ Bad: Large, unfocused methods
public void UpdatePersonalInfo(string firstName, string lastName, string email)
{
    if (string.IsNullOrWhiteSpace(firstName))
        throw new ArgumentException(...);
    if (firstName.Length < 2 || firstName.Length > 50)
        throw new ArgumentException(...);
    // ... 50 more lines
}
```

### 4.3 Using Statements

**Order**:
1. System namespaces
2. Third-party namespaces
3. Project namespaces

```csharp
// ✅ Good
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MediatR;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

// ❌ Bad: Mixed order
using StudentManagement.Domain.Entities;
using System;
using MediatR;
using System.Linq;
```

**Global Usings**: Use for common namespaces
```csharp
// GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
```

## 5. Domain Layer Standards

### 5.1 Entities

#### 5.1.1 Encapsulation
```csharp
// ✅ Good: Private setters, factory methods
public class Student : BaseEntity<StudentId>
{
    public string FirstName { get; private set; }

    protected Student() { } // For EF Core

    private Student(...) { ... }

    public static Student Create(...) { ... }

    public void UpdatePersonalInfo(...) { ... }
}

// ❌ Bad: Public setters
public class Student
{
    public string FirstName { get; set; }  // ❌ Anyone can modify
}
```

#### 5.1.2 Business Logic in Entities
```csharp
// ✅ Good: Rich domain model
public class Student
{
    private readonly List<Enrollment> _enrollments = new();

    public void AddEnrollment(Enrollment enrollment)
    {
        if (enrollment.StudentId != Id)
            throw new ArgumentException("Enrollment must belong to this student");

        if (_enrollments.Any(e => e.CourseId == enrollment.CourseId && e.IsActive))
            throw new InvalidOperationException("Already enrolled in this course");

        _enrollments.Add(enrollment);
        UpdateTimestamp();
    }

    public GPA CalculateGPA()
    {
        var completed = _enrollments
            .Where(e => e.Grade != null && e.IsCompleted)
            .ToList();

        if (!completed.Any())
            return new GPA(0.0m);

        var totalPoints = completed.Sum(e => e.Grade!.GradePoints * e.CreditHours);
        var totalCredits = completed.Sum(e => e.CreditHours);

        return new GPA(totalCredits > 0 ? totalPoints / totalCredits : 0.0m);
    }
}

// ❌ Bad: Anemic domain model
public class Student
{
    public List<Enrollment> Enrollments { get; set; }
}

// Business logic in service layer ❌
public class StudentService
{
    public void AddEnrollment(Student student, Enrollment enrollment)
    {
        // Business logic should be in Student entity
    }
}
```

#### 5.1.3 Validation
```csharp
// ✅ Good: Validate in constructor/methods
public class Student
{
    private Student(string firstName, string lastName, ...)
    {
        FirstName = ValidateName(firstName, nameof(firstName));
        LastName = ValidateName(lastName, nameof(lastName));
        Email = email; // Email validates itself
        DateOfBirth = ValidateDateOfBirth(dateOfBirth);
    }

    private static string ValidateName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", paramName);

        var trimmed = name.Trim();
        if (trimmed.Length < 2 || trimmed.Length > 50)
            throw new ArgumentException("Name must be 2-50 characters", paramName);

        return trimmed;
    }
}

// ❌ Bad: No validation
public class Student
{
    public Student(string firstName, string lastName)
    {
        FirstName = firstName; // ❌ No validation
        LastName = lastName;   // ❌ No validation
    }
}
```

### 5.2 Value Objects

#### 5.2.1 Immutability
```csharp
// ✅ Good: Immutable value object
public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        Value = ValidateAndFormat(value);
    }

    private static string ValidateAndFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");

        var formatted = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(formatted))
            throw new ArgumentException("Invalid email format");

        return formatted;
    }
}

// ❌ Bad: Mutable value object
public class Email
{
    public string Value { get; set; } // ❌ Can be modified
}
```

#### 5.2.2 Self-Validation
```csharp
// ✅ Good: Validates itself
public record GPA
{
    public decimal Value { get; }

    public GPA(decimal value)
    {
        if (value < 0 || value > 4.0m)
            throw new ArgumentException("GPA must be between 0 and 4.0");

        Value = Math.Round(value, 2);
    }
}

// ❌ Bad: External validation
public class GPA
{
    public decimal Value { get; set; }
}

public class GPAValidator // ❌ Shouldn't need separate validator
{
    public bool IsValid(GPA gpa) { ... }
}
```

#### 5.2.3 Equality
```csharp
// ✅ Good: Record type (auto equality)
public record Email(string Value);
public record CourseCode(string Value);

// Or explicit:
public record GPA
{
    public decimal Value { get; }
    // Equality based on Value automatically
}

// Value objects are equal if all properties are equal
var email1 = new Email("test@email.com");
var email2 = new Email("test@email.com");
Assert.Equal(email1, email2); // ✅ True
```

### 5.3 Repository Interfaces

```csharp
// ✅ Good: Domain-focused interface
public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(StudentId id);
    Task<Student?> GetByEmailAsync(Email email);
    Task<Student?> GetWithEnrollmentsAsync(StudentId id);
    Task<IEnumerable<Student>> FindAsync(StudentFilterDto filter);
    Task AddAsync(Student student);
    void Update(Student student);
    void Delete(Student student);
}

// ❌ Bad: Infrastructure-leaking interface
public interface IStudentRepository
{
    Task<DataTable> GetStudentsDataTable(); // ❌ Infrastructure concern
    IQueryable<Student> GetQueryable();      // ❌ EF Core leaking
}
```

## 6. Application Layer Standards

### 6.1 Commands

#### 6.1.1 Command Pattern
```csharp
// ✅ Good: Immutable command with record
public record CreateStudentCommand : IRequest<ApiResponseDto<StudentDto>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }

    public static CreateStudentCommand FromDto(CreateStudentDto dto)
    {
        return new CreateStudentCommand
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth
        };
    }
}

// ❌ Bad: Mutable command
public class CreateStudentCommand
{
    public string FirstName { get; set; } // ❌ Can be modified
    public string LastName { get; set; }  // ❌ Can be modified
}
```

#### 6.1.2 Command Handler Pattern
```csharp
// ✅ Good: Single responsibility handler
public class CreateStudentCommandHandler
    : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStudentCommandHandler(
        IStudentRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

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

        // 2. Check uniqueness
        var existingStudent = await _repository.GetByEmailAsync(email);
        if (existingStudent != null)
        {
            return ApiResponseDto<StudentDto>.Failure(
                "Email already exists");
        }

        // 3. Save
        await _repository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Map to DTO
        var dto = _mapper.Map<StudentDto>(student);

        // 5. Return response
        return ApiResponseDto<StudentDto>.Success(
            dto,
            "Student created successfully");
    }
}
```

### 6.2 Queries

```csharp
// ✅ Good: Focused query
public record GetStudentsQuery : IRequest<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public static GetStudentsQuery FromDto(StudentFilterDto filter)
    {
        return new GetStudentsQuery
        {
            SearchTerm = filter.SearchTerm,
            IsActive = filter.IsActive,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
}

public class GetStudentsQueryHandler
    : IRequestHandler<GetStudentsQuery, ApiResponseDto<PagedResultDto<StudentSummaryDto>>>
{
    public async Task<...> Handle(GetStudentsQuery request, ...)
    {
        var students = await _repository.FindAsync(...);
        var dtos = _mapper.Map<List<StudentSummaryDto>>(students);
        var pagedResult = new PagedResultDto<StudentSummaryDto>(...);
        return ApiResponseDto<...>.Success(pagedResult);
    }
}
```

### 6.3 DTOs

```csharp
// ✅ Good: Clean DTO with init-only properties
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

// ❌ Bad: Domain concerns in DTO
public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }

    // ❌ Business logic shouldn't be in DTO
    public GPA CalculateGPA() { ... }

    // ❌ Navigation properties
    public List<Enrollment> Enrollments { get; set; }
}
```

### 6.4 Validators

```csharp
// ✅ Good: Comprehensive validation
public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be 2-50 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(2, 50).WithMessage("Last name must be 2-50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .Must(BeValidAge).WithMessage("Student must be between 13 and 120 years old");
    }

    private bool BeValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.UtcNow.Year - dateOfBirth.Year;
        return age >= 13 && age <= 120;
    }
}
```

### 6.5 AutoMapper Profiles

```csharp
// ✅ Good: Explicit mapping
public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        // Entity to DTO
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Email,
                      opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.GPA,
                      opt => opt.MapFrom(src => src.CalculateGPA().Value));

        CreateMap<Student, StudentSummaryDto>()
            .ForMember(dest => dest.FullName,
                      opt => opt.MapFrom(src => src.FullName));

        // Value object mappings
        CreateMap<Email, string>()
            .ConvertUsing(src => src.Value);
    }
}

// ❌ Bad: Unmapped properties causing runtime errors
public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentDto>();
        // ❌ Will fail because Email is value object
        // ❌ Will fail because GPA needs to be calculated
    }
}
```

## 7. Infrastructure Layer Standards

### 7.1 DbContext

```csharp
// ✅ Good: Clean DbContext
public class StudentManagementDbContext : DbContext
{
    public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from separate files
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new GradeConfiguration());

        // Or apply all from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudentManagementDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity<Guid> || e.Entity is BaseEntity<StudentId>);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((dynamic)entry.Entity).CreatedAt = DateTime.UtcNow;
                ((dynamic)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                ((dynamic)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

// ❌ Bad: Configurations in DbContext
public class StudentManagementDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ❌ Configurations should be in separate files
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.FirstName).IsRequired();
            // ... 100 more lines
        });
    }
}
```

### 7.2 Entity Configurations

```csharp
// ✅ Good: Separate configuration file
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Primary key
        builder.HasKey(s => s.Id);

        // Value object conversion (StudentId)
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new StudentId(value));

        // Owned entity (Email)
        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(100);
        });

        // Regular properties
        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(s => s.Email)
            .IsUnique();

        // Relationships
        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table name
        builder.ToTable("Students");
    }
}
```

### 7.3 Repository Implementation

```csharp
// ✅ Good: Clean repository implementation
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

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(s =>
                s.FirstName.Contains(filter.SearchTerm) ||
                s.LastName.Contains(filter.SearchTerm) ||
                s.Email.Value.Contains(filter.SearchTerm));
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == filter.IsActive.Value);
        }

        // Apply pagination
        var students = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return students;
    }
}

// ❌ Bad: Generic repository exposing IQueryable
public class Repository<T>
{
    public IQueryable<T> GetAll() // ❌ Leaks EF Core details
    {
        return _dbSet.AsQueryable();
    }
}
```

## 8. WebApi Layer Standards

### 8.1 Controllers

```csharp
// ✅ Good: Thin controller
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
    /// <param name="filter">Filter criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of students</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResultDto<StudentSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(typeof(ApiResponseDto<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
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

// ❌ Bad: Business logic in controller
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _repository;

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        // ❌ Business logic in controller
        if (string.IsNullOrWhiteSpace(dto.FirstName))
            return BadRequest("First name is required");

        // ❌ Direct repository access
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        await _repository.AddAsync(student);

        return Ok(student);
    }
}
```

### 8.2 Middleware

```csharp
// ✅ Good: Focused middleware
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

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

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        _logger.LogWarning(ex, "Validation error occurred");

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();

        var response = ApiResponseDto<object>.Failure(
            "Validation failed",
            errors);

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = ApiResponseDto<object>.Failure(
            "An error occurred processing your request");

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

### 8.3 Dependency Injection

```csharp
// ✅ Good: Extension methods per layer
public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddResponseCompression(...);
        services.AddMemoryCache();
        services.AddHealthChecks();
        services.AddSwaggerGen(...);
        services.AddCors(...);

        return services;
    }
}

// In Program.cs
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);
builder.Services.AddWebApi();

// ❌ Bad: All registrations in Program.cs
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddMediatR(...);
builder.Services.AddAutoMapper(...);
// ... hundreds of lines
```

## 9. Testing Standards (Planned)

### 9.1 Unit Tests

```csharp
// ✅ Good: AAA pattern (Arrange, Act, Assert)
public class StudentTests
{
    [Fact]
    public void Create_ValidData_ReturnsStudent()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new Email("john@email.com");
        var dateOfBirth = new DateTime(2000, 1, 1);

        // Act
        var student = Student.Create(firstName, lastName, email, dateOfBirth);

        // Assert
        Assert.NotNull(student);
        Assert.Equal(firstName, student.FirstName);
        Assert.Equal(lastName, student.LastName);
        Assert.Equal(email, student.Email);
    }

    [Theory]
    [InlineData("A")]           // Too short
    [InlineData("")]            // Empty
    [InlineData(null)]          // Null
    public void Create_InvalidFirstName_ThrowsException(string firstName)
    {
        // Arrange
        var lastName = "Doe";
        var email = new Email("john@email.com");
        var dateOfBirth = new DateTime(2000, 1, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Student.Create(firstName, lastName, email, dateOfBirth));
    }
}
```

### 9.2 Integration Tests

```csharp
// ✅ Good: Integration test with WebApplicationFactory
public class StudentsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public StudentsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStudents_ReturnsSuccessStatusCode()
    {
        // Arrange
        var url = "/api/students";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("success", content.ToLower());
    }

    [Fact]
    public async Task CreateStudent_ValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new CreateStudentDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@email.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/students", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## 10. Comments và Documentation

### 10.1 XML Documentation Comments

```csharp
// ✅ Good: XML comments cho public APIs
/// <summary>
/// Creates a new student with the specified information.
/// </summary>
/// <param name="firstName">The student's first name (2-50 characters)</param>
/// <param name="lastName">The student's last name (2-50 characters)</param>
/// <param name="email">The student's email address (must be unique)</param>
/// <param name="dateOfBirth">The student's date of birth (age 13-120)</param>
/// <returns>A new student instance</returns>
/// <exception cref="ArgumentException">
/// Thrown when firstName, lastName, or dateOfBirth is invalid
/// </exception>
public static Student Create(
    string firstName,
    string lastName,
    Email email,
    DateTime dateOfBirth)
{
    // Implementation
}

// ❌ Bad: Redundant comments
// Sets the first name
public string FirstName { get; set; } // ❌ Obvious

// Creates a student
public static Student Create(...) // ❌ Redundant
```

### 10.2 Inline Comments

```csharp
// ✅ Good: Explain WHY, not WHAT
public GPA CalculateGPA()
{
    var completed = _enrollments
        .Where(e => e.Grade != null && e.IsCompleted)
        .ToList();

    // Return 0 GPA if no completed courses to avoid division by zero
    if (!completed.Any())
        return new GPA(0.0m);

    var totalPoints = completed.Sum(e => e.Grade!.GradePoints * e.CreditHours);
    var totalCredits = completed.Sum(e => e.CreditHours);

    return new GPA(totalCredits > 0 ? totalPoints / totalCredits : 0.0m);
}

// ❌ Bad: Explain WHAT (code is self-explanatory)
// Loop through enrollments
foreach (var enrollment in _enrollments) // ❌ Obvious
{
    // Add enrollment to list
    list.Add(enrollment); // ❌ Obvious
}
```

### 10.3 TODO Comments

```csharp
// ✅ Good: Actionable TODOs
// TODO: Implement caching for frequently accessed students
// TODO: Add bulk enrollment feature (Issue #123)
// FIXME: Handle concurrent enrollment edge case
// HACK: Temporary fix until EF Core bug is resolved

// ❌ Bad: Vague TODOs
// TODO: Fix this
// TODO: Optimize
// TODO: Check
```

## 11. General Best Practices

### 11.1 SOLID Principles

#### Single Responsibility
```csharp
// ✅ Good: Single responsibility
public class CreateStudentCommandHandler
{
    // Only handles creating students
}

// ❌ Bad: Multiple responsibilities
public class StudentHandler
{
    public void Create() { }
    public void Update() { }
    public void Delete() { }
    public void SendEmail() { } // ❌ Email is separate concern
}
```

#### Dependency Inversion
```csharp
// ✅ Good: Depend on abstractions
public class CreateStudentCommandHandler
{
    private readonly IStudentRepository _repository; // Interface
    private readonly IUnitOfWork _unitOfWork;        // Interface
}

// ❌ Bad: Depend on concrete classes
public class CreateStudentCommandHandler
{
    private readonly StudentRepository _repository;  // ❌ Concrete class
    private readonly UnitOfWork _unitOfWork;         // ❌ Concrete class
}
```

### 11.2 Null Handling

```csharp
// ✅ Good: Null checking with nullable reference types
public class Student
{
    public Email Email { get; private set; } = null!;

    private Student(Email email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
}

// ✅ Good: Null-conditional operator
var email = student?.Email?.Value;

// ✅ Good: Null-coalescing operator
var name = student.FirstName ?? "Unknown";

// ❌ Bad: No null checking
public Student(Email email)
{
    Email = email; // ❌ Could be null
}
```

### 11.3 String Handling

```csharp
// ✅ Good: Use string interpolation
var message = $"Student {student.FullName} created successfully";

// ✅ Good: Use StringBuilder for loops
var sb = new StringBuilder();
foreach (var student in students)
{
    sb.AppendLine($"{student.FirstName} {student.LastName}");
}

// ❌ Bad: String concatenation in loops
var result = "";
foreach (var student in students)
{
    result += student.FirstName + " " + student.LastName + "\n"; // ❌ Inefficient
}
```

### 11.4 DateTime Handling

```csharp
// ✅ Good: Always use UTC
public class Student
{
    public DateTime EnrollmentDate { get; private set; }

    public static Student Create(...)
    {
        var student = new Student();
        student.EnrollmentDate = DateTime.UtcNow; // ✅ UTC
        return student;
    }
}

// ❌ Bad: Using local time
public DateTime EnrollmentDate { get; set; } = DateTime.Now; // ❌ Local time
```

### 11.5 Exception Handling

```csharp
// ✅ Good: Specific exceptions
public static Email Create(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("Email cannot be empty", nameof(value));

    if (!EmailRegex.IsMatch(value))
        throw new ArgumentException("Invalid email format", nameof(value));

    return new Email(value);
}

// ❌ Bad: Generic exceptions
public static Email Create(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new Exception("Error"); // ❌ Too generic, no context
}

// ❌ Bad: Swallowing exceptions
try
{
    await _repository.SaveAsync();
}
catch
{
    // ❌ Silent failure
}
```

## 12. Thêm Tính Năng Mới

### 12.1 Workflow

**1. Domain Layer** (nếu cần entity/value object mới):
```
1. Tạo entity trong Domain/Entities/
2. Tạo value objects trong Domain/ValueObjects/
3. Tạo repository interface trong Domain/Repositories/
4. Tạo domain events trong Domain/Events/
```

**2. Infrastructure Layer**:
```
1. Tạo entity configuration trong Infrastructure/Data/Configurations/
2. Implement repository trong Infrastructure/Repositories/
3. Add DbSet vào DbContext
4. Create migration
5. Apply migration
```

**3. Application Layer**:
```
1. Tạo DTOs trong Application/DTOs/
2. Tạo Command trong Application/Commands/[Entity]/
3. Tạo CommandHandler
4. Tạo Validator trong Application/Validators/
5. Tạo Query trong Application/Queries/[Entity]/
6. Tạo QueryHandler
7. Tạo/cập nhật AutoMapper profile trong Application/Mappings/
```

**4. WebApi Layer**:
```
1. Tạo/cập nhật Controller trong WebApi/Controllers/
2. Add endpoints với proper HTTP methods
3. Add XML documentation comments
4. Test qua Swagger UI
```

### 12.2 Example: Thêm Department Entity

**Step 1: Domain**
```csharp
// Domain/Entities/Department.cs
public class Department : BaseEntity<Guid>
{
    public string Name { get; private set; }
    public string Code { get; private set; }

    private readonly List<Course> _courses = new();
    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    public static Department Create(string name, string code)
    {
        return new Department
        {
            Name = name,
            Code = code
        };
    }
}

// Domain/Repositories/IDepartmentRepository.cs
public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department?> GetByCodeAsync(string code);
}
```

**Step 2: Infrastructure**
```csharp
// Infrastructure/Data/Configurations/DepartmentConfiguration.cs
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Code).IsRequired().HasMaxLength(10);
        builder.HasIndex(d => d.Code).IsUnique();

        builder.HasMany(d => d.Courses)
            .WithOne()
            .HasForeignKey("DepartmentId");
    }
}

// Infrastructure/Repositories/DepartmentRepository.cs
public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public async Task<Department?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Code == code);
    }
}

// Add to DbContext
public DbSet<Department> Departments => Set<Department>();

// Create migration
dotnet ef migrations add AddDepartmentEntity -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

**Step 3: Application**
```csharp
// Application/DTOs/DepartmentDtos.cs
public class DepartmentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Code { get; init; }
}

// Application/Commands/Departments/CreateDepartmentCommand.cs
public record CreateDepartmentCommand : IRequest<ApiResponseDto<DepartmentDto>>
{
    public string Name { get; init; }
    public string Code { get; init; }
}

// Application/Commands/Departments/CreateDepartmentCommandHandler.cs
public class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, ApiResponseDto<DepartmentDto>>
{
    // Implementation
}

// Application/Validators/Departments/CreateDepartmentCommandValidator.cs
public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    // Validation rules
}

// Application/Mappings/DepartmentMappingProfile.cs
public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        CreateMap<Department, DepartmentDto>();
    }
}
```

**Step 4: WebApi**
```csharp
// WebApi/Controllers/DepartmentsController.cs
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<DepartmentDto>>>> GetDepartments()
    {
        // Implementation
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<DepartmentDto>>> CreateDepartment(
        [FromBody] CreateDepartmentDto dto)
    {
        // Implementation
    }
}
```

## 13. Git Commit Messages

```
✅ Good commit messages:
feat: Add Department entity with CRUD operations
fix: Correct GPA calculation for withdrawn courses
refactor: Extract validation logic to separate methods
docs: Update API documentation for enrollment endpoints
test: Add unit tests for Student entity
chore: Update dependencies to latest versions

❌ Bad commit messages:
Update code
Fix bug
Changes
WIP
asdf
```

**Format**: `type: description`

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code refactoring
- `docs`: Documentation
- `test`: Tests
- `chore`: Maintenance
- `style`: Formatting
- `perf`: Performance

---

**Document Version**: 1.0
**Last Updated**: 2025-01-17
**Status**: Active
