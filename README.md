# Hệ Thống Quản Lý Sinh Viên

Một hệ thống quản lý sinh viên toàn diện được xây dựng với Clean Architecture, Domain-Driven Design (DDD), và các mẫu thiết kế CQRS sử dụng .NET 8.0 và SQLite.

## 🏗️ Kiến Trúc

Dự án này triển khai **Clean Architecture** với các tầng sau:
- **Domain**: Logic nghiệp vụ cốt lõi và các thực thể
- **Application**: Use cases và CQRS handlers
- **Infrastructure**: Truy cập dữ liệu và các dịch vụ bên ngoài
- **WebApi**: REST API controllers và presentation

### Các Mẫu Thiết Kế Chính
- **Clean Architecture** với dependency inversion đúng chuẩn
- **Domain-Driven Design (DDD)** với rich domain models
- **CQRS** pattern sử dụng MediatR
- **Repository Pattern** với Unit of Work
- **AutoMapper** cho object-to-object mapping
- **FluentValidation** pipeline cho input validation

## 🚀 Tính Năng

- **Quản Lý Sinh Viên**: Tạo, đọc, cập nhật sinh viên với validation
- **Quản Lý Khóa Học**: Quản lý khóa học với prerequisite và giới hạn đăng ký
- **Hệ Thống Đăng Ký**: Xử lý đăng ký khóa học và chấm điểm
- **Xử Lý Lỗi Toàn Cục**: Middleware xử lý lỗi tập trung
- **Nén Response**: Nén Gzip để tối ưu hiệu suất
- **Tài Liệu API**: Swagger/OpenAPI documentation nâng cao
- **Validation Pipeline**: FluentValidation tích hợp với MediatR
- **Tích Hợp AutoMapper**: Tự động mapping entity-to-DTO
- **Tối Ưu Hiệu Suất**: Filtering và pagination ở database-level
- **Domain Tests**: Unit tests toàn diện cho domain layer
- **Sẵn Sàng Production**: Health checks, monitoring, logging

## 📋 Yêu Cầu Hệ Thống

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/) (đi kèm với .NET)
- IDE: [Visual Studio](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), hoặc [JetBrains Rider](https://www.jetbrains.com/rider/)

## 🛠️ Cài Đặt

### 1. Clone Repository
```bash
git clone <repository-url>
cd StudentManagement-NetCore-CleanArchitecture-DDD
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build Solution
```bash
dotnet build
```

### 4. Khởi Tạo Database
```bash
# Tạo và áp dụng migrations
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

### 5. Chạy Ứng Dụng
```bash
dotnet run --project src/StudentManagement.WebApi
```

API sẽ khả dụng tại `http://localhost:5282`

## 📖 Sử Dụng

### Tài Liệu API
Sau khi ứng dụng chạy, truy cập Swagger UI tại:
- **Swagger UI**: `http://localhost:5282/swagger`
- **API Docs**: `http://localhost:5282/swagger/v1/swagger.json`

### Các API Call Mẫu

#### Tạo Sinh Viên
```bash
curl -X POST "http://localhost:5282/api/students" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Nguyễn Văn",
    "lastName": "An",
    "email": "nvan.an@email.com",
    "dateOfBirth": "2000-01-15",
    "phoneNumber": "0912345678",
    "address": "123 Đường Chính"
  }'
```

#### Tạo Khóa Học
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

#### Lấy Danh Sách Sinh Viên (có filtering)
```bash
curl "http://localhost:5282/api/students?pageNumber=1&pageSize=10&searchTerm=An&isActive=true"
```

## 🏛️ Cấu Trúc Dự Án

```
src/
├── StudentManagement.Domain/           # Logic nghiệp vụ cốt lõi
│   ├── Entities/                      # Domain entities (Student, Course, Enrollment)
│   ├── ValueObjects/                  # Value objects (Email, GPA, CourseCode)
│   ├── Events/                        # Domain events
│   └── Repositories/                  # Repository interfaces
├── StudentManagement.Application/      # Use cases & CQRS
│   ├── Commands/                      # Write operations (Create, Update)
│   ├── Queries/                       # Read operations (Get, List)
│   ├── DTOs/                          # Data transfer objects
│   ├── Behaviors/                     # Cross-cutting concerns
│   ├── Validators/                    # FluentValidation rules
│   └── Mappings/                      # AutoMapper profiles
├── StudentManagement.Infrastructure/   # Data access & external services
│   ├── Data/                          # EF Core DbContext & configurations
│   ├── Repositories/                  # Repository implementations
│   └── Migrations/                    # Database migrations
├── StudentManagement.WebApi/          # REST API & presentation
│   ├── Controllers/                   # API controllers
│   ├── Middleware/                    # Custom middleware
│   └── Program.cs                     # Application entry point
└── StudentManagement.Domain.Tests/    # Domain layer unit tests
    └── Entities/                      # Entity behavior tests
```

## 🔧 Cấu Hình

### Database
Ứng dụng sử dụng SQLite mặc định. File database `studentmanagement.db` sẽ được tạo trong thư mục output của WebApi.

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=studentmanagement.db"
  }
}
```

### CORS
Development CORS policy cho phép tất cả origins. Cấu hình phù hợp cho production trong `appsettings.json`.

## 🧪 Testing

### Chạy Unit Tests
```bash
# Chạy tất cả tests
dotnet test

# Chạy tests với detailed output
dotnet test --verbosity detailed

# Chạy tests của một project cụ thể
dotnet test src/StudentManagement.Domain.Tests
```

### Manual API Testing
Dự án bao gồm testing API toàn diện thông qua Swagger UI và curl commands.

## 📚 Tài Liệu Tham Khảo API

### Students API
| Method | Endpoint | Mô Tả |
|--------|----------|-------|
| GET | `/api/students` | Lấy danh sách sinh viên có phân trang và filtering |
| GET | `/api/students/{id}` | Lấy sinh viên theo ID |
| POST | `/api/students` | Tạo sinh viên mới |
| PUT | `/api/students/{id}` | Cập nhật sinh viên |

### Courses API
| Method | Endpoint | Mô Tả |
|--------|----------|-------|
| GET | `/api/courses` | Lấy danh sách khóa học có phân trang và filtering |
| GET | `/api/courses/{id}` | Lấy khóa học theo ID |
| POST | `/api/courses` | Tạo khóa học mới |
| PUT | `/api/courses/{id}` | Cập nhật khóa học |

### Enrollments API
| Method | Endpoint | Mô Tả |
|--------|----------|-------|
| GET | `/api/enrollments` | Lấy danh sách đăng ký có phân trang và filtering |
| GET | `/api/enrollments/{id}` | Lấy đăng ký theo ID |
| POST | `/api/enrollments` | Tạo đăng ký mới |
| POST | `/api/enrollments/{id}/assign-grade` | Gán điểm cho đăng ký |

### Định Dạng Response
Tất cả API responses tuân theo cấu trúc này:
```json
{
  "success": true,
  "data": { ... },
  "message": "Thao tác hoàn thành thành công",
  "errors": [],
  "timestamp": "2025-12-02T10:13:34.914429Z"
}
```

## 🛡️ Xử Lý Lỗi

Ứng dụng bao gồm xử lý lỗi toàn diện:
- **Global Exception Middleware**: Bắt và format tất cả exceptions
- **Validation Errors**: FluentValidation errors trả về với status 400
- **Not Found**: 404 errors cho resources không tồn tại
- **Server Errors**: 500 errors cho unexpected exceptions

## 📦 Dependencies

### Core Dependencies
- **.NET 8.0**: Target framework
- **MediatR**: CQRS pattern implementation
- **Entity Framework Core**: ORM và SQLite provider
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation

### Development Dependencies
- **Swashbuckle.AspNetCore**: API documentation
- **Microsoft.EntityFrameworkCore.Tools**: EF Core CLI tools
- **xUnit**: Testing framework
- **FluentAssertions**: Test assertions

## 🚀 Deployment

### Development
```bash
dotnet run --project src/StudentManagement.WebApi
```

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Database Migrations
```bash
# Thêm migration mới
dotnet ef migrations add <MigrationName> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Cập nhật database
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

## 🔄 Development Workflow

### Các Lệnh Cần Thiết
```bash
# Build và run
dotnet build
dotnet run --project src/StudentManagement.WebApi

# Chạy tests
dotnet test

# Database operations
dotnet ef migrations add <Name> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Clean và rebuild
dotnet clean
dotnet build
```

## 📈 Hiệu Suất

### Đặc Điểm Hiệu Suất Hiện Tại
- **Database-Level Operations**: Filtering và pagination thực hiện ở database
- **Response Compression**: Nén Gzip được bật
- **AutoMapper**: Object mapping được tối ưu
- **Connection Pooling**: EF Core connection pooling được bật
- **Async/Await**: Tất cả operations đều async

## 🤝 Đóng Góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/tinh-nang-tuyet-voi`)
3. Tuân theo code style và architecture patterns hiện có
4. Đảm bảo tất cả builds pass (`dotnet build`)
5. Test thay đổi của bạn kỹ lưỡng
6. Commit thay đổi (`git commit -m 'Thêm tính năng tuyệt vời'`)
7. Push lên branch (`git push origin feature/tinh-nang-tuyet-voi`)
8. Tạo Pull Request

### Code Style
- Tuân theo nguyên tắc Clean Architecture
- Sử dụng nguyên tắc thiết kế SOLID
- Duy trì separation of concerns
- Bao gồm validation phù hợp
- Tuân theo C# naming conventions

## 📄 License

Dự án này được cấp phép theo MIT License - xem file [LICENSE](LICENSE) để biết chi tiết.

## 📞 Tài Liệu Tham Khảo

Để biết thông tin chi tiết về dự án, vui lòng tham khảo các tài liệu sau:

- **[CLAUDE.md](CLAUDE.md)** - Hướng dẫn cho Claude Code, quy tắc phát triển và best practices
- **[DATABASE_STRUCTURE.md](DATABASE_STRUCTURE.md)** - Cấu trúc database chi tiết, entities và relationships
- **[IMPLEMENTATION_PLAN.md](IMPLEMENTATION_PLAN.md)** - Kế hoạch triển khai từng phase, trạng thái hiện tại và roadmap
- **[ARCHITECTURE_EXPLANATION_VN.md](ARCHITECTURE_EXPLANATION_VN.md)** - Giải thích chi tiết về kiến trúc (nếu có)

### API Documentation
- Kiểm tra API documentation tại `/swagger` khi chạy ứng dụng

---

**Xây Dựng Với Clean Architecture 🏗️ | Domain-Driven Design 🎯 | CQRS ⚡**
