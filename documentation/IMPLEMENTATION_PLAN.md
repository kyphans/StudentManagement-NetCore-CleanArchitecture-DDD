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

### **NuGet Packages**: Xem chi tiết đầy đủ trong [CLAUDE.md](../CLAUDE.md#dependencies).

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

## **Phase 3: Application Layer với CQRS** ✅ HOÀN THÀNH

Triển khai CQRS pattern với MediatR, FluentValidation, và AutoMapper.

**Use Cases Đã Triển Khai:**

### Student Management ✅
- [x] **Commands**: CreateStudent, UpdateStudent, DeleteStudent
- [x] **Queries**: GetStudentById, GetStudents (với pagination và filtering)
- [x] **Validators**: CreateStudentValidator, UpdateStudentValidator, DeleteStudentValidator
- [x] **DTOs**: StudentDto, CreateStudentDto, UpdateStudentDto
- [x] **Mappings**: StudentMappingProfile (AutoMapper)

### Course Management ✅
- [x] **Commands**: CreateCourse, UpdateCourse, DeleteCourse
- [x] **Queries**: GetCourseById, GetCourses (với pagination và filtering)
- [x] **Validators**: CreateCourseValidator, UpdateCourseValidator, DeleteCourseValidator
- [x] **DTOs**: CourseDto, CreateCourseDto, UpdateCourseDto
- [x] **Mappings**: CourseMappingProfile (AutoMapper)

### Enrollment Management ✅
- [x] **Commands**: CreateEnrollment, AssignGrade
- [x] **Queries**: GetEnrollmentById, GetEnrollments (với pagination và filtering)
- [x] **Validators**: CreateEnrollmentValidator, AssignGradeValidator
- [x] **DTOs**: EnrollmentDto, CreateEnrollmentDto, AssignGradeDto
- [x] **Mappings**: EnrollmentMappingProfile (AutoMapper)

### Grade Management ✅
- [x] **DTOs**: GradeDto (tích hợp trong Enrollment)
- [x] **Mappings**: GradeMappingProfile (AutoMapper)
- [x] **Commands**: AssignGrade (xử lý cả Grade entity)

**Infrastructure Đã Triển Khai:**
- [x] Setup MediatR configuration (DependencyInjection.cs)
- [x] Implement ValidationBehavior pipeline (FluentValidation integration)
- [x] Tạo AutoMapper profiles cho tất cả entities
- [x] Define DTOs cho tất cả operations (CommonDtos, StudentDtos, CourseDtos, EnrollmentDtos, GradeDtos)
- [x] Pagination và filtering support (PaginatedList, query parameters)

**Tính Năng Nổi Bật:**
- ✅ **CQRS Pattern**: Tách biệt Commands (write) và Queries (read)
- ✅ **MediatR**: 16 handlers (Commands + Queries)
- ✅ **FluentValidation**: 8 validators với validation pipeline
- ✅ **AutoMapper**: 4 mapping profiles
- ✅ **Pagination**: PaginatedList với metadata (pageNumber, pageSize, totalCount)
- ✅ **Filtering**: Search terms và status filtering
- ✅ **Response Wrapping**: ApiResponseDto<T> cho consistent API responses

**Thống Kê Implementation:**
- **Commands**: 6 (Create/Update/Delete Student, Course, Enrollment + AssignGrade)
- **Queries**: 6 (GetById, GetList cho Student, Course, Enrollment)
- **Validators**: 8 (tương ứng với các Commands)
- **DTOs**: 15+ (các DTO classes cho requests/responses)
- **Mappings**: 4 profiles (Student, Course, Enrollment, Grade)
- **Behaviors**: 1 (ValidationBehavior)
- **Total Files**: ~47 C# files trong Application layer

**Note**: Phase này đã hoàn thành với đầy đủ tính năng CQRS, validation, và mapping. Một số use cases bổ sung như WithdrawFromCourse, CompleteEnrollment có thể được thêm vào sau nếu cần.

## **Phase 4: Infrastructure Implementation** ✅ HOÀN THÀNH

Triển khai repositories, DbContext, migrations và data access layer với EF Core và SQLite.

**Đã Triển Khai:**

### Database Context & Configuration ✅
- [x] **StudentManagementDbContext**: DbContext với DbSet cho tất cả entities
- [x] **Entity Configurations** (Fluent API):
  - [x] StudentConfiguration - với unique email index, default values
  - [x] CourseConfiguration - với Prerequisites collection handling
  - [x] EnrollmentConfiguration - với proper relationships
  - [x] GradeConfiguration - với foreign key constraints
- [x] **Value Object Conversions**: Email, StudentId, CourseCode
- [x] **Relationships**: Cascade delete, navigation properties
- [x] **SQLite Configuration**: Connection string, migrations setup

### Repository Pattern ✅
- [x] **Generic Repository**: Base Repository<TEntity, TId>
- [x] **StudentRepository**: Với 7 specialized methods
  - GetByEmailAsync, GetActiveStudentsAsync
  - SearchByNameAsync, GetWithEnrollmentsAsync
  - GetStudentsByEnrollmentDateRangeAsync
  - IsEmailUniqueAsync
- [x] **CourseRepository**: Với course-specific queries
  - GetByCodeAsync, GetWithPrerequisitesAsync
  - GetAvailableCoursesAsync, IsCodeUniqueAsync
- [x] **EnrollmentRepository**: Với enrollment queries
  - GetStudentEnrollmentsAsync, GetCourseEnrollmentsAsync
  - GetActiveEnrollmentsAsync
- [x] **UnitOfWork**: Transaction management, SaveChangesAsync
  - BeginTransaction, CommitTransaction, RollbackTransaction

### Database Migrations ✅
- [x] **Initial Migration**: CleanInitialMigration (2025-09-29)
- [x] **ModelSnapshot**: EF Core model snapshot
- [x] **SQLite Database**: studentmanagement.db

### Dependency Injection ✅
- [x] **Infrastructure DI**: DependencyInjection.cs
- [x] **Repository Registration**: Scoped lifetime
- [x] **DbContext Registration**: SQLite configuration

**Thống Kê:**
- **Total Files**: ~14 C# files
- **Repositories**: 4 (Student, Course, Enrollment + base Repository)
- **Configurations**: 4 (Student, Course, Enrollment, Grade)
- **Migrations**: 1 initial migration
- **UnitOfWork**: Transaction support implemented

**Note**: Logging với Serilog và database seed data có thể được thêm vào sau. Repository integration tests cần bổ sung.

## **Phase 5: WebApi Layer** ✅ HOÀN THÀNH

Tạo REST API controllers, middleware, và Swagger documentation với best practices.

**Controllers Đã Triển Khai:**

### API Controllers ✅
- [x] **BaseApiController**: Base controller class (nếu cần)
- [x] **StudentsController**: Full CRUD operations
  - GET /api/students (with filtering, pagination)
  - GET /api/students/{id}
  - POST /api/students
  - PUT /api/students/{id}
  - DELETE /api/students/{id}
- [x] **CoursesController**: Course management
  - GET /api/courses (with filtering, pagination)
  - GET /api/courses/{id}
  - POST /api/courses
  - PUT /api/courses/{id}
  - DELETE /api/courses/{id}
- [x] **EnrollmentsController**: Enrollment management
  - GET /api/enrollments (with filtering, pagination)
  - GET /api/enrollments/{id}
  - POST /api/enrollments
  - POST /api/enrollments/{id}/assign-grade
- [x] **HealthController**: Health check endpoint

**Middleware & Infrastructure ✅**
- [x] **GlobalExceptionMiddleware**: Comprehensive error handling
  - ValidationException → 400 Bad Request
  - ArgumentException → 400 Bad Request
  - KeyNotFoundException → 404 Not Found
  - UnauthorizedAccessException → 401 Unauthorized
  - Generic Exception → 500 Internal Server Error
  - ApiErrorResponse với ProblemDetails format
- [x] **Response Compression**: Gzip compression
  - EnableForHttps: true
  - CompressionLevel: Optimal
  - MimeTypes: JSON, XML, plain text
- [x] **CORS Configuration**: "AllowAll" policy cho development
- [x] **Swagger/OpenAPI**: Full API documentation
  - SwaggerDoc v1 với descriptions
  - XML comments support
  - EnableAnnotations
  - Swagger UI tại /swagger
- [x] **Health Checks**: /health endpoint
- [x] **Memory Cache**: In-memory caching setup

**Configuration & Setup ✅**
- [x] **Program.cs**: Clean startup configuration
  - Layer-based DI registration (Application → Infrastructure → WebApi)
  - Middleware pipeline configuration
  - Development vs Production setup
- [x] **DependencyInjection.cs**: WebApi layer DI
  - Controllers configuration
  - Response compression
  - Swagger setup
  - CORS policies
  - Health checks

**API Features:**
- ✅ **Thin Controllers**: Delegate to MediatR
- ✅ **Consistent Responses**: ApiResponseDto<T> wrapper
- ✅ **Validation**: ModelState validation
- ✅ **Status Codes**: Proper HTTP status codes (200, 201, 400, 404)
- ✅ **Async/Await**: All endpoints are async
- ✅ **CancellationToken**: Support for request cancellation
- ✅ **Route Attributes**: RESTful routing conventions
- ✅ **XML Documentation**: Swagger comments

**Thống Kê:**
- **Total Files**: ~8 C# files
- **Controllers**: 4 (Students, Courses, Enrollments, Health)
- **Middleware**: 1 (GlobalExceptionMiddleware)
- **Endpoints**: ~15 API endpoints
- **Documentation**: Swagger UI fully configured

**Note**: Request/response logging có thể được thêm vào. API integration tests cần bổ sung. Grade assignment được xử lý qua EnrollmentsController thay vì GradesController riêng.

## **Phase 6: Authentication & Advanced Features** 🔨 PARTIAL (Prepared)

Custom JWT authentication, authorization, caching, và advanced features.

**Trạng Thái: PREPARED - Chưa Implement**

### Đã Chuẩn Bị:
- [x] **JwtSettings Configuration**: appsettings.json đã có JWT config
  - Secret key (256-bit)
  - Issuer, Audience
  - ExpiryMinutes (60), RefreshTokenExpiryDays (7)
- [x] **Serilog Package**: Installed (v9.0.0)
  - Serilog.AspNetCore với sinks (Console, Debug, File)
  - Chưa configure trong Program.cs
- [x] **Memory Cache**: AddMemoryCache() đã setup
  - In-memory caching ready to use
  - Chưa implement response caching strategies

### Chưa Implement:

**Authentication (Custom JWT):**
- [ ] Create User entity (domain model)
- [ ] Implement password hashing service (BCrypt/PBKDF2)
- [ ] Create JWT token service (generate, validate, refresh)
- [ ] Add AuthController (login, register, refresh token)
- [ ] Configure JWT Bearer authentication middleware
- [ ] Setup authorization policies (Admin, Teacher, Student, Staff roles)
- [ ] Add [Authorize] attributes to protected endpoints

**Advanced Features:**
- [ ] Configure Serilog trong Program.cs
  - Console, File, Seq/ELK logging
  - Request/response logging
  - Structured logging
- [ ] Response caching strategies
  - OutputCache middleware (.NET 8)
  - Cache profiles cho các endpoints
- [ ] Rate limiting
  - Fixed window, sliding window, token bucket
  - Per-user, per-IP rate limits
- [ ] Bulk operations (import/export CSV/Excel)
- [ ] Advanced reporting queries
- [ ] Performance monitoring (Application Insights, Prometheus)
- [ ] Database seeding

**Nhiệm vụ Chi Tiết:**
1. **Domain Layer**: User aggregate, Role value object, RefreshToken entity
2. **Application Layer**: Auth commands/queries, JWT service interface
3. **Infrastructure Layer**: JWT service implementation, password hasher
4. **WebApi Layer**: AuthController, configure authentication/authorization
5. **Logging**: Configure Serilog với multiple sinks
6. **Caching**: Implement caching strategies
7. **Rate Limiting**: Configure rate limiting policies
8. **Testing**: Auth flow tests, security tests
9. **Documentation**: Auth documentation, security guidelines

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

Xem API reference đầy đủ trong [README.md](../README.md#-tài-liệu-tham-khảo-api). Phase 6 sẽ thêm authentication endpoints.

### **Development Commands**

Xem tất cả các lệnh phát triển trong [CLAUDE.md](../CLAUDE.md#các-lệnh-cần-thiết).

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
3. **Phase 3**: Application Layer với CQRS (100%)
   - MediatR setup với 16 handlers
   - 6 Commands và 6 Queries
   - 8 FluentValidation validators
   - 4 AutoMapper profiles
   - ValidationBehavior pipeline
   - Pagination và filtering support
   - ~47 C# files

4. **Phase 4**: Infrastructure Implementation (100%)
   - DbContext với 4 entity configurations
   - 4 Repository implementations
   - 1 Database migration
   - Unit of Work với transaction support
   - ~14 C# files
5. **Phase 5**: WebApi Layer (100%)
   - 4 Controllers với ~15 endpoints
   - Global exception middleware
   - Response compression (Gzip)
   - Swagger/OpenAPI documentation
   - Health checks
   - ~8 C# files

#### 🎯 Phase Đang Thực Hiện
6. **Phase 6**: Authentication & Advanced Features (~10% - Prepared)
   - ✅ JWT settings configured (appsettings.json)
   - ✅ Serilog package installed (v9.0.0)
   - ✅ Memory cache setup
   - ⏳ User entity & authentication flow
   - ⏳ JWT service implementation
   - ⏳ Authorization policies
   - ⏳ Serilog configuration
   - ⏳ Response caching strategies
   - ⏳ Rate limiting

### **Metrics & Statistics**

**Code Quality:**
- ✅ Clean Architecture principles tuân thủ
- ✅ SOLID principles applied
- ✅ DDD tactical patterns implemented
- ✅ Comprehensive unit test coverage (41 tests)
- ✅ No external dependencies trong Domain layer

**Test Coverage:**
- Domain Layer: ✅ 100% (41/41 tests pass)
- Application Layer: ✅ Implemented (unit tests cần bổ sung)
- Infrastructure Layer: ✅ Implemented (integration tests cần bổ sung)
- WebApi Layer: ✅ Implemented (API tests cần bổ sung)

**Implementation Progress:**
- Phase 1: ✅ 100% Complete - Solution Structure
- Phase 2: ✅ 100% Complete - Domain Layer (41 tests)
- Phase 3: ✅ 100% Complete - Application Layer (CQRS)
- Phase 4: ✅ 100% Complete - Infrastructure (EF Core + Repositories)
- Phase 5: ✅ 100% Complete - WebApi (Controllers + Middleware)
- Phase 6: 🔨 ~10% Complete - Authentication & Advanced Features (Prepared)
  - JWT settings configured
  - Serilog package installed
  - Memory cache setup

**Overall Progress**: ~85% (5/6 phases hoàn thành + Phase 6 prepared)

**Code Statistics:**
- **Total C# Files**: ~115+ files (excluding obj/bin)
- **Domain**: 19 files (entities, value objects, interfaces)
- **Application**: 47 files (commands, queries, DTOs, validators, mappings)
- **Infrastructure**: 14 files (DbContext, repositories, configurations, migrations)
- **WebApi**: 8 files (controllers, middleware, startup)
- **Tests**: 27 files (domain tests)

### **Các Bước Tiếp Theo**

1. ✅ **Phase 4 & 5 Verified**: Infrastructure và WebApi đã được verify hoàn chỉnh
2. **Write Tests**: Thêm comprehensive tests
   - Application layer unit tests (handlers, validators)
   - Infrastructure integration tests (repositories, DbContext)
   - WebApi API tests (endpoints, middleware)
3. **Phase 6: Authentication & Authorization**
   - Implement custom JWT authentication
   - Create User entity và authentication flows
   - Add authorization policies (Admin, Teacher, Student, Staff)
   - Refresh token rotation
4. **Phase 6: Advanced Features**
   - Redis caching hoặc advanced memory caching
   - Rate limiting trên endpoints
   - Logging với Serilog
   - Advanced reporting queries
   - Performance monitoring
5. **Production Readiness**
   - Security audit
   - Performance testing và optimization
   - Docker containerization
   - CI/CD pipeline setup
6. **Documentation**
   - API documentation enhancement
   - Developer guide
   - Deployment guide

Kế hoạch này cung cấp nền tảng vững chắc cho một hệ thống quản lý sinh viên production-ready tuân theo các best practices trong .NET development, security, và architecture.

---

**Cập nhật lần cuối**: 2025-12-03
**Phiên bản**: 2.0
**Trạng thái**: Phases 1-5 Complete (83% overall), Phase 6 Ready to Start
