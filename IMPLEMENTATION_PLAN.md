# Kế Hoạch Triển Khai DDD & Clean Architecture
## Hệ Thống Quản Lý Sinh Viên với SQLite

### **Tổng Quan Kiến Trúc Solution**

Solution tuân theo các nguyên tắc Clean Architecture với các tactical patterns của Domain-Driven Design (DDD):

```
StudentManagement/
├── src/
│   ├── StudentManagement.Domain/           # Logic nghiệp vụ cốt lõi (innermost layer)
│   │   ├── Entities/                      # Student, Course, Enrollment, Grade
│   │   ├── ValueObjects/                  # StudentId, Email, CourseCode, GPA
│   │   ├── Events/                        # Domain events
│   │   └── Repositories/                  # Repository interfaces
│   ├── StudentManagement.Application/      # Use cases và application logic
│   │   ├── Commands/                      # CQRS commands
│   │   ├── Queries/                       # CQRS queries
│   │   ├── DTOs/                         # Data transfer objects
│   │   ├── Behaviors/                     # MediatR behaviors
│   │   ├── Validators/                    # FluentValidation validators
│   │   └── Mappings/                      # AutoMapper profiles
│   ├── StudentManagement.Infrastructure/   # External concerns
│   │   ├── Data/                         # EF Core, SQLite setup
│   │   ├── Repositories/                 # Repository implementations
│   │   └── Migrations/                   # Database migrations
│   └── StudentManagement.WebApi/          # Presentation layer
│       ├── Controllers/                  # API controllers
│       ├── Middleware/                   # Error handling, logging
│       └── Program.cs                    # Startup configuration
├── tests/
│   ├── StudentManagement.Domain.Tests/    # ✅ Domain unit tests (HOÀN THÀNH)
│   ├── StudentManagement.Application.Tests/
│   ├── StudentManagement.Infrastructure.Tests/
│   └── StudentManagement.WebApi.Tests/
└── docs/
```

### **Trách Nhiệm Từng Layer**

1. **Domain Layer** - Pure business logic, entities, value objects, domain services
2. **Application Layer** - Use cases, commands/queries, interfaces cho infrastructure
3. **Infrastructure Layer** - Data persistence, external APIs, cross-cutting concerns
4. **WebApi Layer** - REST API endpoints, request/response models, dependency injection

### **Chiến Lược Authentication & Authorization**

**Phương Thức Authentication**: Custom JWT implementation (không dùng ASP.NET Core Identity)
**Database**: SQLite với Entity Framework Core
**Authorization**: Role-based với custom policies

**User Roles**:
- **Admin**: Toàn quyền truy cập hệ thống
- **Teacher**: Quản lý khóa học và điểm
- **Student**: Xem hồ sơ học tập của mình
- **Staff**: Chức năng quản trị

**Auth Flow**:
1. User registration/login
2. JWT token generation với claims
3. Token validation trên protected endpoints
4. Role/policy-based authorization

**Tính Năng Bảo Mật**:
- Password hashing tùy chỉnh
- JWT token expiration và refresh
- Rate limiting trên auth endpoints
- CORS configuration

### **NuGet Packages Theo Layer**

**Domain Layer**:
- Không có (pure .NET)

**Application Layer**:
- MediatR
- FluentValidation
- Microsoft.Extensions.DependencyInjection.Abstractions
- AutoMapper

**Infrastructure Layer**:
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design
- System.IdentityModel.Tokens.Jwt

**WebApi Layer**:
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore
- MediatR (direct reference cho v13+ compatibility)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Serilog.AspNetCore
- AutoMapper.Extensions.Microsoft.DependencyInjection

**Testing Layer**:
- xUnit
- FluentAssertions
- Microsoft.EntityFrameworkCore.InMemory

### **Các Giai Đoạn Triển Khai**

## **Phase 1: Solution Restructuring & Database Setup** ✅ HOÀN THÀNH

Tạo 4 class library projects với dependencies phù hợp, cấu hình SQLite và Entity Framework Core.

**Nhiệm vụ:**
- [x] Tạo StudentManagement.Domain project
- [x] Tạo StudentManagement.Application project
- [x] Tạo StudentManagement.Infrastructure project
- [x] Tạo StudentManagement.WebApi project
- [x] Thiết lập project references và dependencies
- [x] Cài đặt NuGet packages cần thiết
- [x] Cấu hình SQLite connection string

**Kết quả**: 4 projects được tạo với dependency flow đúng, SQLite configured, solution builds thành công.

## **Phase 2: Domain Layer Implementation** ✅ HOÀN THÀNH

Triển khai toàn bộ domain layer với entities, value objects, repository interfaces và comprehensive unit tests.

**Domain Entities:**
- ✅ **Student** (Aggregate Root) - 14 unit tests
- ✅ **Course** - 11 unit tests
- ✅ **Enrollment** - 9 unit tests
- ✅ **Grade** - 7 unit tests

**Value Objects:**
- ✅ StudentId - Strong-typed GUID wrapper
- ✅ CourseCode - Validated course code
- ✅ Email - Validated email với case normalization
- ✅ GPA - Calculated GPA (0.0-4.0)

**Repository Interfaces:**
- ✅ IStudentRepository
- ✅ ICourseRepository
- ✅ IEnrollmentRepository
- ✅ IGradeRepository
- ✅ IUnitOfWork

**Domain Tests (41 tests, 100% pass):**
- ✅ StudentTests: 14 test cases
  - Constructor validation
  - Personal info updates
  - Enrollment management
  - GPA calculation
  - Activation/deactivation
- ✅ CourseTests: 11 test cases
  - Course creation và validation
  - Prerequisite management
  - Enrollment capacity
  - Course updates
- ✅ EnrollmentTests: 9 test cases
  - Enrollment lifecycle
  - Status transitions
  - Grade assignment
  - Completion rules
- ✅ GradeTests: 7 test cases
  - Grade creation từ numeric score
  - Letter grade validation
  - Grade updates
  - Comments management

**Nhiệm vụ:**
- [x] Tạo base entity và aggregate root classes
- [x] Implement Student aggregate với business rules
- [x] Tạo Course và Enrollment entities
- [x] Implement value objects
- [x] Định nghĩa repository interfaces
- [x] Tạo domain events (placeholder)
- [x] Viết comprehensive unit tests (41 tests)

**Kết quả**: Domain layer hoàn chỉnh với rich business logic, 100% test coverage, tuân thủ DDD principles.

## **Phase 3: Application Layer với CQRS** ⏳ TIẾP THEO

Triển khai CQRS pattern với MediatR, FluentValidation, và AutoMapper.

**Use Cases Cần Xây Dựng:**

### Student Management
- [ ] **Commands**: CreateStudent, UpdateStudent, DeactivateStudent
- [ ] **Queries**: GetStudentById, GetStudentList, GetStudentWithEnrollments
- [ ] **Validators**: CreateStudentValidator, UpdateStudentValidator
- [ ] **DTOs**: StudentDto, CreateStudentDto, UpdateStudentDto

### Course Management
- [ ] **Commands**: CreateCourse, UpdateCourse, AddPrerequisite, RemoveCourse
- [ ] **Queries**: GetCourseById, GetCourseList, GetCourseWithEnrollments
- [ ] **Validators**: CreateCourseValidator, UpdateCourseValidator
- [ ] **DTOs**: CourseDto, CreateCourseDto, UpdateCourseDto

### Enrollment Management
- [ ] **Commands**: EnrollStudent, WithdrawFromCourse, CompleteEnrollment
- [ ] **Queries**: GetEnrollmentById, GetStudentEnrollments, GetCourseEnrollments
- [ ] **Validators**: EnrollStudentValidator
- [ ] **DTOs**: EnrollmentDto, EnrollStudentDto

### Grade Management
- [ ] **Commands**: AssignGrade, UpdateGrade
- [ ] **Queries**: GetStudentGrades, GetEnrollmentGrade
- [ ] **Validators**: AssignGradeValidator
- [ ] **DTOs**: GradeDto, AssignGradeDto

**Infrastructure cần thiết:**
- [ ] Setup MediatR configuration
- [ ] Tạo command/query base classes
- [ ] Implement validation behavior pipeline
- [ ] Implement logging behavior
- [ ] Tạo AutoMapper profiles
- [ ] Define DTOs cho tất cả operations

**Nhiệm vụ:**
- [ ] Setup MediatR configuration trong DI
- [ ] Tạo Commands cho Student, Course, Enrollment, Grade
- [ ] Tạo Queries cho read operations
- [ ] Implement FluentValidation validators
- [ ] Tạo DTOs và AutoMapper profiles
- [ ] Add behavior pipelines (validation, logging)
- [ ] Write application layer tests

## **Phase 4: Infrastructure Implementation** ⏳

Triển khai repositories, DbContext, migrations và data access layer.

**Nhiệm vụ:**
- [ ] Configure StudentManagementDbContext
- [ ] Create entity configurations (Fluent API)
  - [ ] StudentConfiguration
  - [ ] CourseConfiguration với Prerequisites value comparer
  - [ ] EnrollmentConfiguration
  - [ ] GradeConfiguration
- [ ] Implement repository pattern
  - [ ] StudentRepository
  - [ ] CourseRepository
  - [ ] EnrollmentRepository
  - [ ] GradeRepository
  - [ ] UnitOfWork
- [ ] Setup SQLite configuration
- [ ] Create database migrations
- [ ] Implement logging với Serilog
- [ ] Add database seed data
- [ ] Write repository integration tests

## **Phase 5: WebApi Layer** ⏳

Tạo REST API controllers, middleware, và Swagger documentation.

**Controllers Cần Xây Dựng:**
- [ ] **StudentsController**: CRUD operations
- [ ] **CoursesController**: Course management
- [ ] **EnrollmentsController**: Enrollment management
- [ ] **GradesController**: Grade assignment

**Middleware & Configuration:**
- [ ] Global exception handling middleware
- [ ] Request/response logging
- [ ] Response compression (Gzip)
- [ ] CORS configuration
- [ ] Swagger/OpenAPI setup
- [ ] Health checks

**Nhiệm vụ:**
- [ ] Tạo base API controller
- [ ] Implement CRUD controllers
- [ ] Configure middleware pipeline
- [ ] Setup Swagger documentation
- [ ] Add global error handling
- [ ] Implement response compression
- [ ] Add request/response models
- [ ] Write API integration tests

## **Phase 6: Authentication & Advanced Features** ⏳

Custom JWT authentication, authorization, caching, và advanced features.

**Authentication (Custom JWT):**
- [ ] Create User entity (domain model)
- [ ] Implement password hashing service
- [ ] Create JWT token service
- [ ] Add AuthController (login, register, refresh)
- [ ] Configure JWT middleware
- [ ] Setup authorization policies

**Advanced Features:**
- [ ] Response caching với Redis/in-memory
- [ ] Rate limiting
- [ ] Bulk operations (import/export)
- [ ] Advanced reporting queries
- [ ] Performance monitoring
- [ ] Logging và diagnostics

**Nhiệm vụ:**
- [ ] Create User aggregate trong Domain
- [ ] Implement JWT token generation
- [ ] Add authentication middleware
- [ ] Configure authorization policies
- [ ] Implement refresh token rotation
- [ ] Add response caching
- [ ] Configure rate limiting
- [ ] Create integration tests cho auth flow
- [ ] Security configuration review
- [ ] Performance optimization
- [ ] Update documentation

### **Database Schema Overview**

**Core Tables:**
- **Students** - Thông tin sinh viên
- **Courses** - Thông tin khóa học
- **Enrollments** - Quan hệ Student-Course
- **Grades** - Hệ thống chấm điểm

**Authentication Tables (Phase 6):**
- **Users** - User accounts
- **Roles** - User roles
- **UserRoles** - User-Role mapping
- **RefreshTokens** - JWT refresh tokens

### **API Endpoints Structure**

```
/api/students/
  GET / (Admin/Teacher only)
  GET /{id} (Admin/Teacher hoặc own record)
  POST / (Admin only)
  PUT /{id} (Admin hoặc own record)
  DELETE /{id} (Admin only)

/api/courses/
  GET /
  GET /{id}
  POST / (Admin/Teacher only)
  PUT /{id} (Admin/Teacher only)
  DELETE /{id} (Admin only)

/api/enrollments/
  GET /student/{studentId} (Admin/Teacher hoặc own records)
  POST / (Admin/Teacher only)
  POST /{id}/assign-grade (Teacher/Admin only)
  DELETE /{id} (Admin/Teacher only)

/api/grades/
  GET /student/{studentId} (Admin/Teacher hoặc own records)
  POST / (Teacher/Admin only)
  PUT /{id} (Teacher/Admin only)

/api/auth/ (Phase 6)
  POST /register
  POST /login
  POST /refresh-token
  POST /logout
```

### **Development Commands**

```bash
# Build solution
dotnet build

# Run application
dotnet run --project src/StudentManagement.WebApi

# Run tests
dotnet test

# Run tests với detailed output
dotnet test --verbosity detailed

# Database migrations
dotnet ef migrations add <MigrationName> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
dotnet ef migrations remove -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Watch for changes
dotnet watch --project src/StudentManagement.WebApi

# Clean và rebuild
dotnet clean
dotnet build --configuration Release
```

### **Configuration Settings**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=studentmanagement.db"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-here",
    "Issuer": "StudentManagement",
    "Audience": "StudentManagementUsers",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### **Tiến Độ Hiện Tại**

#### ✅ Phase Hoàn Thành
1. **Phase 1**: Solution Restructuring & Database Setup (100%)
2. **Phase 2**: Domain Layer Implementation (100%)
   - 4 entities với rich business logic
   - 4 value objects
   - 5 repository interfaces
   - 41 unit tests (100% pass)

#### 🎯 Phase Tiếp Theo
3. **Phase 3**: Application Layer với CQRS (0%)
   - MediatR setup
   - Commands & Queries
   - FluentValidation
   - AutoMapper
   - Behavior pipelines

#### ⏳ Phase Chưa Bắt Đầu
4. **Phase 4**: Infrastructure Implementation
5. **Phase 5**: WebApi Layer
6. **Phase 6**: Authentication & Advanced Features

### **Metrics & Statistics**

**Code Quality:**
- ✅ Clean Architecture principles tuân thủ
- ✅ SOLID principles applied
- ✅ DDD tactical patterns implemented
- ✅ Comprehensive unit test coverage (41 tests)
- ✅ No external dependencies trong Domain layer

**Test Coverage:**
- Domain Layer: 100% (41/41 tests pass)
- Application Layer: 0% (chưa implement)
- Infrastructure Layer: 0% (chưa implement)
- WebApi Layer: 0% (chưa implement)

**Implementation Progress:**
- Phase 1: ✅ 100% Complete
- Phase 2: ✅ 100% Complete
- Phase 3: ⏳ 0% Complete
- Phase 4: ⏳ 0% Complete
- Phase 5: ⏳ 0% Complete
- Phase 6: ⏳ 0% Complete

**Overall Progress**: ~33% (2/6 phases hoàn thành)

### **Các Bước Tiếp Theo**

1. **Bắt đầu Phase 3**: Application Layer với CQRS
2. **Setup MediatR**: Configure trong DI container
3. **Tạo Commands**: Implement write operations
4. **Tạo Queries**: Implement read operations
5. **Add Validation**: FluentValidation cho tất cả inputs
6. **Create DTOs**: Data transfer objects và mappings
7. **Write Tests**: Application layer unit tests

Kế hoạch này cung cấp nền tảng vững chắc cho một hệ thống quản lý sinh viên production-ready tuân theo các best practices trong .NET development, security, và architecture.

---

**Cập nhật lần cuối**: 2025-12-02
**Phiên bản**: 1.0
**Trạng thái**: Phase 2 Complete, Ready for Phase 3
