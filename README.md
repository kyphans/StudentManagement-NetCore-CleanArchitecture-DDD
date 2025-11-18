# Student Management System

Một hệ thống quản lý sinh viên toàn diện được xây dựng với **Hexagonal Architecture (Ports & Adapters)**, **Domain-Driven Design (DDD)** và **CQRS pattern** sử dụng .NET 8.0.

## 🎯 Tính Năng Chính

- ✅ **Quản Lý Sinh Viên**: CRUD operations với validation toàn diện
- ✅ **Quản Lý Khóa Học**: Tạo và quản lý courses với prerequisites
- ✅ **Hệ Thống Đăng Ký**: Enrollment workflow với grade tracking
- ✅ **Tính GPA Tự Động**: Real-time GPA calculation
- ✅ **API RESTful**: Comprehensive endpoints với Swagger documentation
- ✅ **Hexagonal Architecture**: Ports & Adapters pattern với explicit boundaries
- ✅ **CQRS Pattern**: Command/Query separation với MediatR
- ✅ **Validation Pipeline**: FluentValidation integrated
- ✅ **AutoMapper**: Automatic DTO mapping
- ✅ **Global Exception Handling**: Centralized error handling

## 🏗️ Kiến Trúc Hexagonal (Ports & Adapters)

```
┌──────────────────────────────────────────────┐
│  PRIMARY ADAPTERS (Driving/Inbound)         │
│  Adapters.WebApi                             │
│  - Controllers                               │
│  - ApplicationServices                       │
└──────────────────┬───────────────────────────┘
                   ↓
┌──────────────────────────────────────────────┐
│  PRIMARY PORTS (Inbound Interfaces)         │
│  Application/Ports/                          │
│  - IStudentManagementPort                    │
│  - ICourseManagementPort                     │
│  - IEnrollmentManagementPort                 │
└──────────────────┬───────────────────────────┘
                   ↓
┌─────────────────────────────────────────────┐
│         APPLICATION CORE (Hexagon)          │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │ Domain (Pure Business Logic)         │  │
│  │ - Entities, Value Objects, Events    │  │
│  └──────────────────────────────────────┘  │
│                                             │
│  ┌──────────────────────────────────────┐  │
│  │ Application (Use Cases)              │  │
│  │ - Commands/Queries, DTOs, Validators│  │
│  └──────────────────────────────────────┘  │
└──────────────────┬──────────────────────────┘
                   ↓
┌──────────────────────────────────────────────┐
│  SECONDARY PORTS (Outbound Interfaces)      │
│  Domain/Ports/IPersistence/                  │
│  - IStudentPersistencePort                   │
│  - ICoursePersistencePort                    │
│  - IEnrollmentPersistencePort                │
│  - IUnitOfWorkPort                           │
└──────────────────┬───────────────────────────┘
                   ↓
┌──────────────────────────────────────────────┐
│  SECONDARY ADAPTERS (Driven/Outbound)       │
│  Adapters.Persistence                        │
│  - EfCore*Adapter (implements Ports)         │
│  - DbContext, Configurations, Migrations     │
└──────────────────────────────────────────────┘
```

**Dependency Flow**: Adapters → Ports → Application Core → Domain

### Technology Stack
- **.NET 8.0** - Framework
- **ASP.NET Core** - Web API
- **Entity Framework Core 8.0** - ORM
- **SQLite** - Database (development)
- **MediatR** - CQRS implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Structured logging

## 📚 Tài Liệu Chi Tiết

Tài liệu đầy đủ bằng tiếng Việt có trong thư mục `docs/`:

- **[Tổng Quan Dự Án & PDR](docs/project-overview-pdr.md)** - Vision, goals, yêu cầu chức năng & phi chức năng
- **[Tóm Tắt Codebase](docs/codebase-summary.md)** - High-level overview của codebase, các layer và components
- **[Chuẩn Mã](docs/code-standards.md)** - Coding standards, naming conventions, best practices
- **[Kiến Trúc Hệ Thống](docs/system-architecture.md)** - Chi tiết architecture, patterns và design decisions

## 🚀 Quick Start

### Yêu Cầu
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- IDE: [Visual Studio](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), hoặc [JetBrains Rider](https://www.jetbrains.com/rider/)

### Cài Đặt

```bash
# 1. Clone repository
git clone <repository-url>
cd StudentManagement

# 2. Restore dependencies
dotnet restore

# 3. Build solution
dotnet build

# 4. Apply database migrations
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# 5. Run application
dotnet run --project src/StudentManagement.WebApi
```

API sẽ chạy tại: `http://localhost:5282`

### Swagger UI
Truy cập API documentation tại: `http://localhost:5282/swagger`

## 📖 API Usage Examples

### Tạo Student
```bash
curl -X POST "http://localhost:5282/api/students" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Nguyễn",
    "lastName": "Văn A",
    "email": "nguyenvana@email.com",
    "dateOfBirth": "2000-01-15"
  }'
```

### Lấy Danh Sách Students (có filter)
```bash
curl "http://localhost:5282/api/students?searchTerm=Nguyen&isActive=true&pageNumber=1&pageSize=10"
```

### Tạo Course
```bash
curl -X POST "http://localhost:5282/api/courses" \
  -H "Content-Type: application/json" \
  -d '{
    "code": "CS101",
    "name": "Nhập Môn Khoa Học Máy Tính",
    "description": "Các khái niệm cơ bản về khoa học máy tính",
    "creditHours": 3,
    "department": "Khoa Học Máy Tính",
    "maxEnrollment": 30
  }'
```

### Đăng Ký Khóa Học
```bash
curl -X POST "http://localhost:5282/api/enrollments" \
  -H "Content-Type: application/json" \
  -d '{
    "studentId": "student-guid-here",
    "courseId": "course-guid-here"
  }'
```

### Chấm Điểm
```bash
curl -X POST "http://localhost:5282/api/enrollments/{enrollment-id}/assign-grade" \
  -H "Content-Type: application/json" \
  -d '{
    "letterGrade": "A",
    "numericScore": 95.5,
    "comments": "Excellent performance"
  }'
```

## 🗂️ Cấu Trúc Dự Án (Hexagonal Architecture)

```
StudentManagement/
├── src/
│   ├── StudentManagement.Domain/                      # 🎯 Domain Core (Hexagon Center)
│   │   ├── Entities/                                 # Student, Course, Enrollment, Grade
│   │   ├── ValueObjects/                             # Email, GPA, CourseCode, StudentId
│   │   ├── Events/                                   # Domain events
│   │   ├── Services/                                 # Domain services
│   │   └── Ports/                                    # 🔌 SECONDARY PORTS (Outbound)
│   │       └── IPersistence/                         # Persistence port interfaces
│   │           ├── IStudentPersistencePort.cs
│   │           ├── ICoursePersistencePort.cs
│   │           ├── IEnrollmentPersistencePort.cs
│   │           └── IUnitOfWorkPort.cs
│   │
│   ├── StudentManagement.Application/                 # 🔄 Application Core (Use Cases)
│   │   ├── Ports/                                    # 🔌 PRIMARY PORTS (Inbound)
│   │   │   ├── IStudentManagementPort.cs
│   │   │   ├── ICourseManagementPort.cs
│   │   │   └── IEnrollmentManagementPort.cs
│   │   ├── Commands/                                 # Write operations (CQRS)
│   │   │   ├── Students/                             # CreateStudentCommand, etc.
│   │   │   ├── Courses/
│   │   │   └── Enrollments/
│   │   ├── Queries/                                  # Read operations (CQRS)
│   │   │   ├── Students/                             # GetStudentsQuery, etc.
│   │   │   ├── Courses/
│   │   │   └── Enrollments/
│   │   ├── DTOs/                                     # Data transfer objects
│   │   ├── Validators/                               # FluentValidation rules
│   │   ├── Mappings/                                 # AutoMapper profiles
│   │   └── Common/Behaviors/                         # MediatR pipeline behaviors
│   │
│   ├── StudentManagement.Adapters.Persistence/       # 🔧 SECONDARY ADAPTERS (Driven)
│   │   ├── Data/                                     # DbContext & configurations
│   │   │   ├── StudentManagementDbContext.cs
│   │   │   └── Configurations/                       # EF Core entity configs
│   │   ├── Repositories/                             # Persistence adapter implementations
│   │   │   ├── EfCoreRepositoryBase.cs
│   │   │   ├── EfCoreStudentAdapter.cs              # ← implements IStudentPersistencePort
│   │   │   ├── EfCoreCourseAdapter.cs               # ← implements ICoursePersistencePort
│   │   │   ├── EfCoreEnrollmentAdapter.cs           # ← implements IEnrollmentPersistencePort
│   │   │   └── EfCoreUnitOfWorkAdapter.cs           # ← implements IUnitOfWorkPort
│   │   └── Migrations/                               # EF Core migrations
│   │
│   └── StudentManagement.Adapters.WebApi/            # 🌐 PRIMARY ADAPTERS (Driving)
│       ├── Controllers/                              # REST API endpoints
│       │   ├── StudentsController.cs                 # ← depends on IStudentManagementPort
│       │   ├── CoursesController.cs
│       │   └── EnrollmentsController.cs
│       ├── ApplicationServices/                      # Primary port implementations
│       │   ├── StudentApplicationService.cs          # ← implements IStudentManagementPort
│       │   ├── CourseApplicationService.cs
│       │   └── EnrollmentApplicationService.cs
│       ├── Middleware/                               # Exception handling, etc.
│       ├── Program.cs                                # Application entry point
│       └── DependencyInjection.cs                    # DI configuration
│
├── docs/                                              # Documentation (Vietnamese)
│   ├── project-overview-pdr.md                       # Project overview & PDR
│   ├── codebase-summary.md                           # Codebase summary
│   ├── code-standards.md                             # Coding standards
│   ├── system-architecture.md                        # Architecture details
│   └── ARCHITECTURE_EXPLANATION_VN.md                # Hexagonal architecture explanation
│
├── README.md
└── CLAUDE.md                                          # AI assistant guidance
```

## 🔧 Database Operations

### Tạo Migration Mới
```bash
dotnet ef migrations add <MigrationName> \
    -p src/StudentManagement.Adapters.Persistence \
    -s src/StudentManagement.Adapters.WebApi
```

### Apply Migrations
```bash
dotnet ef database update \
    -p src/StudentManagement.Adapters.Persistence \
    -s src/StudentManagement.Adapters.WebApi
```

### Remove Last Migration
```bash
dotnet ef migrations remove \
    -p src/StudentManagement.Adapters.Persistence \
    -s src/StudentManagement.Adapters.WebApi
```

## 📊 API Endpoints

### Students API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/students` | Lấy danh sách students (có filter & pagination) |
| GET | `/api/students/{id}` | Lấy student theo ID |
| POST | `/api/students` | Tạo student mới |
| PUT | `/api/students/{id}` | Cập nhật student |
| DELETE | `/api/students/{id}` | Xóa student (soft delete) |

### Courses API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/courses` | Lấy danh sách courses |
| GET | `/api/courses/{id}` | Lấy course theo ID |
| POST | `/api/courses` | Tạo course mới |
| PUT | `/api/courses/{id}` | Cập nhật course |
| DELETE | `/api/courses/{id}` | Xóa course (soft delete) |

### Enrollments API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/enrollments` | Lấy danh sách enrollments |
| GET | `/api/enrollments/{id}` | Lấy enrollment theo ID |
| POST | `/api/enrollments` | Tạo enrollment mới |
| POST | `/api/enrollments/{id}/assign-grade` | Chấm điểm cho enrollment |

### Health Check
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Kiểm tra trạng thái hệ thống |

## ✨ Key Design Patterns

### Hexagonal Architecture (Ports & Adapters)
- **Domain Core**: Pure business logic, không dependencies
- **Application Core**: Use cases, orchestration logic
- **Primary Ports**: Inbound interfaces (IStudentManagementPort, etc.)
- **Secondary Ports**: Outbound interfaces (IPersistencePort, etc.)
- **Primary Adapters**: HTTP API (Controllers, ApplicationServices)
- **Secondary Adapters**: Database (EfCore*Adapter)

**Benefits**:
- Framework-agnostic core logic
- Database-agnostic persistence
- Easy to test (mock adapters)
- Clear boundaries between layers
- Technology independence

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Modify data (CreateStudentCommand, UpdateCourseCommand)
- **Queries**: Read data (GetStudentsQuery, GetCourseByIdQuery)
- **Handlers**: One handler per command/query
- **MediatR**: Pipeline implementation

### Ports Pattern (thay thế Repository Pattern)
- **Primary Ports**: Application service interfaces
- **Secondary Ports**: Persistence interfaces trong Domain
- **Adapters**: Concrete implementations
- **Rõ ràng về direction**: Inbound vs Outbound

### Domain-Driven Design
- **Entities**: Rich domain models (Student, Course, Enrollment)
- **Value Objects**: Immutable types (Email, GPA, CourseCode, StudentId)
- **Aggregates**: Aggregate roots với boundaries rõ ràng
- **Domain Events**: Capture business events
- **Domain Services**: Complex business logic không thuộc về entity

## 🔒 Security (Planned)

Authentication và Authorization sẽ được implement trong Phase 6:
- JWT Bearer tokens
- Role-based access control (Admin, Teacher, Student, Staff)
- Password hashing
- Token refresh mechanism

## 🧪 Testing (Planned)

Test projects sẽ được thêm trong Phase 6:
- **Unit Tests**: Domain entities, value objects, handlers
- **Integration Tests**: API endpoints, repositories
- **E2E Tests**: Full workflow testing

## 📈 Performance Features

**Current**:
- ✅ Async/await throughout
- ✅ Response compression (Gzip)
- ✅ EF Core connection pooling
- ✅ AutoMapper optimizations

**Planned**:
- Database-level filtering
- Response caching
- Redis distributed cache
- Query optimization
- Database indexing

## 🛡️ Error Handling

### Standardized Response Format
```json
{
  "success": true/false,
  "data": { ... },
  "message": "Operation message",
  "errors": ["Error 1", "Error 2"],
  "timestamp": "2025-01-17T10:00:00Z"
}
```

### HTTP Status Codes
- **200 OK**: Thành công
- **201 Created**: Resource created
- **400 Bad Request**: Validation errors
- **404 Not Found**: Resource không tồn tại
- **500 Internal Server Error**: Server errors

## 🤝 Contributing

1. Fork repository
2. Tạo feature branch: `git checkout -b feature/amazing-feature`
3. Tuân thủ [coding standards](docs/code-standards.md)
4. Commit changes: `git commit -m 'feat: Add amazing feature'`
5. Push to branch: `git push origin feature/amazing-feature`
6. Tạo Pull Request

### Commit Message Format
```
type: description

Types: feat, fix, refactor, docs, test, chore, style, perf
```

## 📝 Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=studentmanagement.db"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-here",
    "Issuer": "StudentManagement",
    "Audience": "StudentManagementUsers",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Database Location
Development: `src/StudentManagement.WebApi/bin/Debug/net8.0/studentmanagement.db`

## 🗺️ Roadmap

- ✅ **Phase 1**: Project setup & architecture
- ✅ **Phase 2**: Domain layer implementation
- ✅ **Phase 3**: Application layer (CQRS)
- ✅ **Phase 4**: Infrastructure layer → **Migrated to Adapters.Persistence**
- ✅ **Phase 5**: WebApi layer → **Migrated to Adapters.WebApi**
- ✅ **Phase 6**: **Hexagonal Architecture Migration** ✨
  - ✅ Repository interfaces → Persistence Ports
  - ✅ Infrastructure → Adapters.Persistence
  - ✅ WebApi → Adapters.WebApi
  - ✅ Application Services → Primary Ports
  - ✅ EfCore*Adapter → Secondary Adapters
  - ✅ Documentation update
- 🔄 **Phase 7**: Testing & Enhancements
  - Unit & integration tests
  - JWT authentication & authorization
  - Advanced filtering & search
  - Caching layer
  - Performance optimization
  - Docker support
  - CI/CD pipeline

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details

## 📄 Documentation

- **API Documentation**: Swagger UI tại `/swagger`
- **Tổng quan dự án**: [docs/project-overview-pdr.md](docs/project-overview-pdr.md)
- **Codebase summary**: [docs/codebase-summary.md](docs/codebase-summary.md)
- **Coding standards**: [docs/code-standards.md](docs/code-standards.md)
- **System architecture**: [docs/system-architecture.md](docs/system-architecture.md)
- **AI Assistant Guide**: [CLAUDE.md](CLAUDE.md)

## 🎓 Learning Resources

Dự án này là ví dụ tốt để học:
- **Hexagonal Architecture (Ports & Adapters)** ⭐
- **Domain-Driven Design (DDD)**
- **CQRS pattern** với MediatR
- **Ports Pattern** (thay thế Repository Pattern)
- **Unit of Work pattern**
- **Value Objects** & **Strongly-typed IDs**
- **Entity Framework Core** với Value Object conversions
- **MediatR** với Pipeline Behaviors
- **FluentValidation** trong pipeline
- **AutoMapper** với custom value object mappings
- **Dependency Injection** theo layers
- **Clean Code** & **SOLID Principles**

## 📚 Additional Resources

- [Hexagonal Architecture Explanation (Vietnamese)](docs/ARCHITECTURE_EXPLANATION_VN.md)
- [System Architecture Details](docs/system-architecture.md)
- [Coding Standards](docs/code-standards.md)
- [Codebase Summary](docs/codebase-summary.md)

---
**Version**: 1.0.0
**Last Updated**: 2025-01-17
