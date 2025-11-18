# Tài Liệu Tổng Quan Dự Án và Yêu Cầu Phát Triển Sản Phẩm (PDR)

## 1. Tổng Quan Dự Án

### 1.1 Giới Thiệu
**Student Management System** (Hệ Thống Quản Lý Sinh Viên) là một ứng dụng web API toàn diện được xây dựng để quản lý các hoạt động học tập của sinh viên, bao gồm đăng ký khóa học, theo dõi điểm số và tính toán GPA. Dự án được thiết kế theo kiến trúc Clean Architecture và các nguyên tắc Domain-Driven Design (DDD).

### 1.2 Tầm Nhìn
Xây dựng một hệ thống quản lý sinh viên hiện đại, dễ bảo trì và mở rộng, áp dụng các mẫu thiết kế và kiến trúc phần mềm tốt nhất trong ngành, đảm bảo tính linh hoạt, khả năng mở rộng và dễ dàng thích ứng với các yêu cầu nghiệp vụ thay đổi.

### 1.3 Mục Tiêu
- **Khả năng mở rộng**: Hệ thống được thiết kế để dễ dàng thêm các tính năng mới mà không ảnh hưởng đến code hiện tại
- **Bảo trì dễ dàng**: Áp dụng Clean Architecture để tách biệt các concern và giảm coupling
- **Hiệu suất cao**: Tối ưu hóa truy vấn cơ sở dữ liệu và áp dụng caching khi cần
- **Độ tin cậy**: Xử lý lỗi toàn diện và validation nghiêm ngặt
- **Tài liệu đầy đủ**: API documentation rõ ràng thông qua Swagger/OpenAPI

## 2. Yêu Cầu Chức Năng

### 2.1 Quản Lý Sinh Viên

#### 2.1.1 Tạo Sinh Viên Mới
**Mô tả**: Cho phép tạo hồ sơ sinh viên mới trong hệ thống

**Input**:
- `firstName`: Tên (2-50 ký tự)
- `lastName`: Họ (2-50 ký tự)
- `email`: Địa chỉ email (định dạng hợp lệ, unique)
- `dateOfBirth`: Ngày sinh (sinh viên từ 13-120 tuổi)

**Output**: Thông tin sinh viên đã tạo bao gồm ID được sinh tự động

**Business Rules**:
- Email phải unique trong hệ thống
- Tuổi sinh viên phải từ 13 đến 120
- Tên và họ được trim và validate độ dài
- Ngày đăng ký tự động là thời điểm hiện tại
- Trạng thái mặc định là `IsActive = true`

#### 2.1.2 Cập Nhật Thông Tin Sinh Viên
**Mô tả**: Cập nhật thông tin cá nhân của sinh viên

**Input**:
- `id`: Student ID (GUID)
- `firstName`: Tên mới
- `lastName`: Họ mới
- `email`: Email mới

**Business Rules**:
- Chỉ cập nhật được thông tin cá nhân (không thay đổi enrollments)
- Email mới phải unique
- Tự động cập nhật timestamp

#### 2.1.3 Xóa Sinh Viên
**Mô tả**: Xóa (soft delete) sinh viên khỏi hệ thống

**Input**: Student ID

**Business Rules**:
- Thực hiện soft delete (đánh dấu `IsActive = false`)
- Không xóa vật lý để giữ lại lịch sử
- Các enrollment hiện tại vẫn được giữ lại

#### 2.1.4 Tìm Kiếm và Lọc Sinh Viên
**Mô tả**: Tìm kiếm sinh viên với các tiêu chí lọc

**Tham số lọc**:
- `searchTerm`: Tìm theo tên, họ hoặc email
- `isActive`: Lọc theo trạng thái hoạt động
- `pageNumber`: Số trang (mặc định = 1)
- `pageSize`: Số lượng mỗi trang (mặc định = 10)

**Output**: Danh sách sinh viên phân trang với tổng số lượng

#### 2.1.5 Tính GPA
**Mô tả**: Tự động tính GPA dựa trên các enrollment đã hoàn thành

**Công thức**:
```
GPA = Σ(Grade Points × Credit Hours) / Σ(Credit Hours)
```

**Business Rules**:
- Chỉ tính các enrollment có status = Completed
- Chỉ tính các enrollment có grade không null
- GPA = 0 nếu chưa có enrollment nào hoàn thành

### 2.2 Quản Lý Khóa Học

#### 2.2.1 Tạo Khóa Học Mới
**Mô tả**: Tạo khóa học mới trong hệ thống

**Input**:
- `code`: Mã khóa học (unique, định dạng: CS101, MATH201)
- `name`: Tên khóa học (3-100 ký tự)
- `description`: Mô tả
- `creditHours`: Số tín chỉ (1-10)
- `department`: Khoa (2-50 ký tự)
- `maxEnrollment`: Số lượng sinh viên tối đa (1-500, mặc định = 30)

**Business Rules**:
- Mã khóa học phải unique
- CourseCode là value object với validation riêng
- Tự động sinh CourseId (GUID)

#### 2.2.2 Cập Nhật Khóa Học
**Mô tả**: Cập nhật thông tin khóa học

**Input**:
- `id`: Course ID
- Các trường thông tin khóa học

**Business Rules**:
- Không được thay đổi mã khóa học sau khi tạo
- Có thể cập nhật maxEnrollment nhưng phải >= số lượng đã đăng ký hiện tại

#### 2.2.3 Xóa Khóa Học
**Mô tả**: Deactivate khóa học

**Business Rules**:
- Soft delete (đánh dấu `IsActive = false`)
- Không xóa được nếu có sinh viên đang active enrollment

#### 2.2.4 Quản Lý Prerequisites
**Mô tả**: Thêm/xóa các môn tiên quyết

**Business Rules**:
- Một khóa học không thể là prerequisite của chính nó
- Không được trùng lặp prerequisite

#### 2.2.5 Kiểm Tra Khả Năng Đăng Ký
**Mô tả**: Kiểm tra xem khóa học còn chỗ trống không

**Điều kiện**:
- Khóa học phải active (`IsActive = true`)
- Số lượng đăng ký hiện tại < `MaxEnrollment`

### 2.3 Quản Lý Enrollment

#### 2.3.1 Đăng Ký Khóa Học
**Mô tả**: Đăng ký sinh viên vào một khóa học

**Input**:
- `studentId`: ID sinh viên
- `courseId`: ID khóa học
- `creditHours`: Số tín chỉ (lấy từ course)

**Business Rules**:
- Sinh viên không được đăng ký trùng khóa học đang active
- Khóa học phải còn chỗ trống
- Tự động set status = Active
- Tự động set EnrollmentDate = thời điểm hiện tại

#### 2.3.2 Chấm Điểm
**Mô tả**: Gán điểm cho một enrollment

**Input**:
- `enrollmentId`: ID enrollment
- `letterGrade`: Điểm chữ (A, A-, B+, B, B-, C+, C, C-, D+, D, F)
- `numericScore`: Điểm số (0-100)

**Business Rules**:
- Chỉ chấm được enrollment có status = Active
- Grade là value object với validation
- Tự động tính GradePoints từ LetterGrade:
  - A = 4.0, A- = 3.7
  - B+ = 3.3, B = 3.0, B- = 2.7
  - C+ = 2.3, C = 2.0, C- = 1.7
  - D+ = 1.3, D = 1.0
  - F = 0.0

#### 2.3.3 Hoàn Thành Enrollment
**Mô tả**: Đánh dấu enrollment đã hoàn thành

**Business Rules**:
- Chỉ complete được enrollment có status = Active
- Phải có grade trước khi complete
- Tự động set CompletionDate = thời điểm hiện tại
- Tự động set status = Completed

#### 2.3.4 Rút Khóa Học
**Mô tả**: Sinh viên rút khỏi khóa học

**Business Rules**:
- Không rút được enrollment đã completed
- Set status = Withdrawn
- Set CompletionDate = thời điểm rút

#### 2.3.5 Truy Vấn Enrollment
**Mô tả**: Lọc và tìm kiếm enrollment

**Tham số lọc**:
- `studentId`: Lọc theo sinh viên
- `courseId`: Lọc theo khóa học
- `status`: Lọc theo trạng thái (Active, Completed, Withdrawn)
- Pagination support

## 3. Yêu Cầu Phi Chức Năng

### 3.1 Kiến Trúc và Thiết Kế

#### 3.1.1 Clean Architecture
**Yêu cầu**:
- Tuân thủ nguyên tắc Dependency Inversion
- 4 layers rõ ràng: Domain → Application → Infrastructure → WebApi
- Domain layer không phụ thuộc vào bất kỳ thư viện external nào
- Mỗi layer chỉ phụ thuộc vào layer bên trong

#### 3.1.2 Domain-Driven Design (DDD)
**Yêu cầu**:
- **Entities**: Rich domain models với business logic
- **Value Objects**: Immutable types cho các concepts (Email, GPA, CourseCode)
- **Aggregates**: Boundaries rõ ràng (Student là aggregate root)
- **Domain Events**: Capture các sự kiện nghiệp vụ quan trọng
- **Repository Pattern**: Abstraction cho data access

#### 3.1.3 CQRS Pattern
**Yêu cầu**:
- Tách biệt Commands (write) và Queries (read)
- Mỗi use case có một Command/Query class riêng
- Mỗi Command/Query có một Handler riêng
- Sử dụng MediatR cho pipeline

### 3.2 Validation và Error Handling

#### 3.2.1 Input Validation
**Yêu cầu**:
- FluentValidation cho tất cả Commands
- Validation pipeline trong MediatR
- Business rule validation trong Domain layer
- Trả về error messages rõ ràng và cụ thể

#### 3.2.2 Global Exception Handling
**Yêu cầu**:
- Middleware bắt tất cả exceptions
- Standardized error response format:
```json
{
  "success": false,
  "data": null,
  "message": "Error message",
  "errors": ["Detailed error 1", "Detailed error 2"],
  "timestamp": "2025-01-17T10:00:00Z"
}
```

### 3.3 Performance

#### 3.3.1 Database Performance
**Yêu cầu**:
- EF Core với SQLite
- Proper indexing cho các trường tìm kiếm thường xuyên
- Lazy loading tắt, sử dụng explicit Include
- Pagination cho tất cả list queries

#### 3.3.2 Response Compression
**Yêu cầu**:
- Gzip compression cho JSON responses
- Compression level = Optimal
- Enabled cho HTTPS

#### 3.3.3 Caching (Future)
**Kế hoạch**:
- In-memory cache cho static data
- Redis cache cho distributed scenarios
- Cache invalidation strategy

### 3.4 Security

#### 3.4.1 Authentication & Authorization (Planned)
**Yêu cầu**:
- JWT Bearer tokens
- Role-based access control (Admin, Teacher, Student, Staff)
- Token expiry và refresh mechanism

#### 3.4.2 Data Protection
**Yêu cầu**:
- Email validation và normalization
- SQL injection protection (EF Core parameterized queries)
- CORS configuration appropriate cho environment

### 3.5 API Design

#### 3.5.1 RESTful Principles
**Yêu cầu**:
- HTTP methods chuẩn (GET, POST, PUT, DELETE)
- Resource-based URLs (`/api/students`, `/api/courses`)
- Proper HTTP status codes:
  - 200 OK: Thành công
  - 201 Created: Tạo mới thành công
  - 400 Bad Request: Validation errors
  - 404 Not Found: Resource không tồn tại
  - 500 Internal Server Error: Server errors

#### 3.5.2 API Documentation
**Yêu cầu**:
- Swagger/OpenAPI 3.0
- Annotations cho controllers và models
- Example requests/responses
- Interactive API testing qua Swagger UI

### 3.6 Data Management

#### 3.6.1 Database
**Yêu cầu**:
- SQLite cho development và testing
- Entity Framework Core 8.0
- Code-first migrations
- Seed data cho testing

#### 3.6.2 Data Integrity
**Yêu cầu**:
- Foreign key constraints
- Unique constraints (Email, CourseCode)
- Soft deletes thay vì hard deletes
- Audit fields (CreatedAt, UpdatedAt)

### 3.7 Development & Deployment

#### 3.7.1 Development Workflow
**Yêu cầu**:
- .NET 8.0 SDK
- Git version control
- Clear commit messages
- Branch strategy (main, develop, feature branches)

#### 3.7.2 Build & Deployment
**Yêu cầu**:
- Successful build không warnings
- Configuration cho multiple environments (Development, Staging, Production)
- Health check endpoint (`/health`)
- Logging với Serilog

#### 3.7.3 Monitoring (Future)
**Kế hoạch**:
- Application Insights hoặc similar
- Performance metrics
- Error tracking
- Request/response logging

## 4. Technical Stack

### 4.1 Core Technologies
- **.NET 8.0**: Target framework
- **C# 12**: Programming language
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 8.0**: ORM
- **SQLite**: Database

### 4.2 Libraries & Packages

#### Application Layer
- **MediatR 13.0.0**: CQRS implementation
- **AutoMapper 12.0.1**: Object-to-object mapping
- **FluentValidation 12.0.0**: Input validation

#### Infrastructure Layer
- **Microsoft.EntityFrameworkCore.Sqlite 8.0.4**: SQLite provider
- **Microsoft.EntityFrameworkCore.Design 8.0.4**: Design-time support

#### WebApi Layer
- **Swashbuckle.AspNetCore 6.4.0**: OpenAPI/Swagger
- **Serilog.AspNetCore 9.0.0**: Structured logging

## 5. Success Criteria

### 5.1 Functional Criteria
- ✅ Tất cả CRUD operations cho Students hoạt động chính xác
- ✅ Tất cả CRUD operations cho Courses hoạt động chính xác
- ✅ Enrollment workflow hoạt động đầy đủ
- ✅ GPA calculation chính xác
- ✅ Filtering và pagination hoạt động tốt
- ✅ Validation catches tất cả invalid inputs
- ✅ Error handling trả về messages rõ ràng

### 5.2 Technical Criteria
- ✅ Clean Architecture được tuân thủ nghiêm ngặt
- ✅ Không có circular dependencies
- ✅ Domain layer hoàn toàn độc lập
- ✅ CQRS pattern implemented đúng
- ✅ Repository pattern working properly
- ✅ AutoMapper mappings correct
- ✅ FluentValidation rules comprehensive
- ✅ Swagger documentation đầy đủ

### 5.3 Quality Criteria
- ✅ Code rõ ràng, dễ đọc
- ✅ Naming conventions consistent
- ✅ No code duplication
- ✅ SOLID principles applied
- ✅ Build successful without warnings
- ✅ Migrations applied successfully

## 6. Roadmap & Phases

### Phase 1: Project Setup ✅ HOÀN THÀNH
- Solution structure
- Layer dependencies
- Basic configuration

### Phase 2: Domain Layer ✅ HOÀN THÀNH
- Entities (Student, Course, Enrollment, Grade)
- Value Objects (Email, GPA, CourseCode, etc.)
- Repository interfaces
- Domain events

### Phase 3: Application Layer ✅ HOÀN THÀNH
- Commands và Handlers
- Queries và Handlers
- DTOs
- Validators
- AutoMapper profiles
- MediatR configuration

### Phase 4: Infrastructure Layer ✅ HOÀN THÀNH
- DbContext
- Entity Configurations
- Repository implementations
- Migrations
- Unit of Work

### Phase 5: WebApi Layer ✅ HOÀN THÀNH
- Controllers
- Middleware (Exception handling)
- Swagger configuration
- CORS setup
- DI configuration

### Phase 6: Enhancements 🔄 ĐANG LÊN KẾ HOẠCH
- Unit tests
- Integration tests
- Authentication & Authorization
- Advanced filtering
- Caching
- Performance optimization
- Logging enhancements
- Docker support

## 7. Constraints & Limitations

### 7.1 Current Limitations
- Chưa có authentication/authorization
- Chưa có unit/integration tests
- Filtering và pagination đang làm in-memory (cần optimize)
- Chưa có caching layer
- Chưa có advanced reporting features
- Chưa có file upload/download

### 7.2 Technical Constraints
- SQLite có giới hạn về concurrent writes
- Không hỗ trợ stored procedures complex
- Memory-based filtering có giới hạn về dataset size

### 7.3 Future Considerations
- Migration sang SQL Server hoặc PostgreSQL cho production
- Microservices architecture nếu scale lớn
- Event sourcing cho audit trail chi tiết
- GraphQL API nếu cần flexibility hơn

## 8. Maintenance & Support

### 8.1 Documentation
- ✅ README.md với quick start guide
- ✅ CLAUDE.md cho AI assistant guidance
- ✅ API documentation qua Swagger
- ✅ Comprehensive docs trong `/docs` folder
- 🔄 Wiki cho detailed guides (planned)

### 8.2 Version Control
- Git với meaningful commit messages
- Semantic versioning (khi release)
- Changelog maintenance

### 8.3 Support Channels
- GitHub Issues cho bug reports
- Documentation cho FAQs
- Code comments cho complex logic

## 9. Glossary

- **Aggregate Root**: Entity chính trong một aggregate boundary
- **CQRS**: Command Query Responsibility Segregation
- **DDD**: Domain-Driven Design
- **DTO**: Data Transfer Object
- **Entity**: Object có identity, tồn tại qua thời gian
- **GPA**: Grade Point Average
- **ORM**: Object-Relational Mapping
- **Repository**: Pattern để abstract data access
- **Soft Delete**: Đánh dấu record là deleted thay vì xóa vật lý
- **Unit of Work**: Pattern để quản lý transactions
- **Value Object**: Object không có identity, chỉ được định nghĩa bởi giá trị

---

**Document Version**: 1.0
**Last Updated**: 2025-01-17
**Author**: Documentation Team
**Status**: Active
