# CLAUDE.md

File này cung cấp hướng dẫn cho Claude Code (claude.ai/code) khi làm việc với code trong repository này.

## Tổng Quan Dự Án

Đây là **Hệ Thống Quản Lý Sinh Viên** được xây dựng sử dụng **Clean Architecture** với các nguyên tắc **Domain-Driven Design (DDD)** trong .NET 8.0.

**Kiến trúc**: Clean Architecture 4 tầng (Domain → Application → Infrastructure → WebApi)
**Database**: SQLite với Entity Framework Core
**Patterns**: CQRS qua MediatR, Repository Pattern, Domain Events

## Các Lệnh Cần Thiết

### Khởi Động Nhanh
```bash
# Build và run
dotnet build
dotnet run --project src/StudentManagement.WebApi

# Database migrations
dotnet ef migrations add <Name> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Chạy tests
dotnet test
```

## Quy Tắc Kiến Trúc Cốt Lõi

### Luồng Dependency (Nghiêm Ngặt)
- **Domain** → Không có external dependencies
- **Application** → Chỉ Domain
- **Infrastructure** → Domain + Application
- **WebApi** → Application + Infrastructure

### Các Pattern Chính
- **CQRS**: Commands (ghi) và Queries (đọc) qua MediatR
- **Repository**: Interfaces trong Domain, implementations trong Infrastructure
- **Rich Domain Models**: Business logic trong entities, không phải services
- **Value Objects**: Immutable objects cho concepts như Email, CourseCode, GPA
- **Domain Events**: Xử lý side effects và cross-aggregate operations

## Cấu Hình

### Database
- **File**: `studentmanagement.db` (SQLite, được tạo trong thư mục output của WebApi)
- **Connection**: `Data Source=studentmanagement.db` trong `appsettings.json`

### Entity Framework Core
```bash
# Thêm migration
dotnet ef migrations add <MigrationName> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Áp dụng migrations
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Xóa migration cuối
dotnet ef migrations remove -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

## Cấu Trúc Dự Án

```
src/
├── StudentManagement.Domain/           # Logic nghiệp vụ cốt lõi
│   ├── Entities/                      # Domain entities
│   ├── ValueObjects/                  # Value objects
│   ├── Events/                        # Domain events
│   ├── Repositories/                  # Repository interfaces
│   └── Common/                        # Base classes, enums
│
├── StudentManagement.Application/      # Use cases & CQRS
│   ├── Commands/                      # CQRS commands
│   ├── Queries/                       # CQRS queries
│   ├── DTOs/                          # Data transfer objects
│   ├── Behaviors/                     # MediatR behaviors
│   ├── Validators/                    # FluentValidation
│   └── Mappings/                      # AutoMapper profiles
│
├── StudentManagement.Infrastructure/   # Data access & external services
│   ├── Data/                          # DbContext, configurations
│   ├── Repositories/                  # Repository implementations
│   └── Migrations/                    # EF Core migrations
│
├── StudentManagement.WebApi/          # REST API & presentation
│   ├── Controllers/                   # API endpoints
│   ├── Middleware/                    # Custom middleware
│   └── Program.cs                     # Startup configuration
│
└── StudentManagement.Domain.Tests/    # Domain layer unit tests
    └── Entities/                      # Entity behavior tests
```

## Quy Tắc Phát Triển

### Domain Layer
- **Không có dependencies**: Pure C# code, không thư viện ngoài
- **Rich domain models**: Business logic trong entities
- **Value objects**: Immutable, validated
- **Guard clauses**: Validation trong constructors
- **Domain events**: Để communicate changes

### Application Layer
- **CQRS pattern**: Tách Commands và Queries
- **MediatR handlers**: Một handler cho mỗi use case
- **FluentValidation**: Validate inputs trước khi xử lý
- **DTOs**: Không expose domain entities ra ngoài
- **AutoMapper**: Tự động map giữa entities và DTOs

### Infrastructure Layer
- **Repository pattern**: Implement interfaces từ Domain
- **Unit of Work**: Transaction management
- **EF Core**: ORM cho data access
- **Migrations**: Version control cho database schema

### WebApi Layer
- **Controllers**: Thin controllers, delegate to MediatR
- **Exception handling**: Global middleware
- **Response format**: Consistent API response structure
- **Swagger**: Auto-generated API documentation

## Best Practices

### Khi Thêm Entity Mới
1. Tạo entity trong `Domain/Entities`
2. Thêm validation rules trong constructor
3. Implement business methods
4. Tạo repository interface trong `Domain/Repositories`
5. Viết unit tests trong `Domain.Tests`
6. Tạo entity configuration trong `Infrastructure/Data/Configurations`
7. Add migration: `dotnet ef migrations add Add<EntityName>`

### Khi Thêm Use Case Mới
1. Tạo Command/Query trong `Application/Commands` hoặc `Queries`
2. Tạo Handler cho Command/Query
3. Tạo DTO nếu cần
4. Thêm FluentValidation validator
5. Update AutoMapper profile
6. Tạo controller endpoint trong `WebApi/Controllers`

### Testing Strategy
- **Unit Tests**: Domain logic (entities, value objects)
- **Integration Tests**: Use cases với in-memory database
- **API Tests**: End-to-end testing qua HTTP

## Dependencies

### Domain
- Không có external dependencies

### Application
- `MediatR` - CQRS implementation
- `FluentValidation` - Input validation
- `AutoMapper` - Object mapping

### Infrastructure
- `Microsoft.EntityFrameworkCore.Sqlite` - SQLite provider
- `Microsoft.EntityFrameworkCore.Design` - EF Core tools

### WebApi
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `MediatR` - Direct reference

### Testing
- `xUnit` - Test framework
- `FluentAssertions` - Test assertions
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database for testing

## Tài Liệu Tham Khảo

Để biết thông tin chi tiết về dự án, vui lòng tham khảo các tài liệu sau:

- **[README.md](README.md)** - Tổng quan dự án, hướng dẫn cài đặt và sử dụng
- **[DATABASE_STRUCTURE.md](DATABASE_STRUCTURE.md)** - Cấu trúc database chi tiết, entities và relationships
- **[IMPLEMENTATION_PLAN.md](IMPLEMENTATION_PLAN.md)** - Kế hoạch triển khai từng phase, trạng thái hiện tại và roadmap
- **[ARCHITECTURE_EXPLANATION_VN.md](ARCHITECTURE_EXPLANATION_VN.md)** - Giải thích chi tiết về kiến trúc (nếu có)

---

**Cập nhật lần cuối**: 2025-12-02
**Phiên bản**: 1.0
