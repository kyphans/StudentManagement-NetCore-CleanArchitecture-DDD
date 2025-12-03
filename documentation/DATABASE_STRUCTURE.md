# Hệ Thống Quản Lý Sinh Viên - Cấu Trúc Database & Entities

## Tổng Quan

Hệ Thống Quản Lý Sinh Viên sử dụng database **SQLite** với **Entity Framework Core 9.0** và tuân theo các nguyên tắc **Domain-Driven Design (DDD)**. Database schema được triển khai sử dụng các mẫu thiết kế Clean Architecture với strongly-typed entities và value objects.

**File Database**: `studentmanagement.db` (SQLite)
**Vị trí**: Thư mục output của WebApi
**Connection String**: `Data Source=studentmanagement.db`

## Entity Relationship Diagram

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Student   │────────▶│  Enrollment  │◀────────│   Course    │
│             │         │              │         │             │
│ + StudentId │         │ + Id (Guid)  │         │ + Id (Guid) │
│ + FirstName │         │ + StudentId  │         │ + Code      │
│ + LastName  │         │ + CourseId   │         │ + Name      │
│ + Email     │         │ + Status     │         │ + Department│
│ + DOB       │         │ + Grade      │         │ + Credits   │
│ + IsActive  │         │              │         │ + IsActive  │
└─────────────┘         └──────────────┘         └─────────────┘
                                │                        │
                                ▼                        │
                        ┌──────────────┐                 │
                        │    Grade     │                 │
                        │              │                 │
                        │ + Id (Guid)  │                 │
                        │ + Letter     │                 │
                        │ + Points     │                 │
                        │ + Score      │                 │
                        │ + Comments   │                 │
                        └──────────────┘                 │
                                                         │
                                                         ▼
                                                ┌──────────────┐
                                                │Prerequisites │
                                                │ (Self-Ref)   │
                                                │              │
                                                │ Course ──▶   │
                                                │ Course       │
                                                └──────────────┘
```

## Core Entities

### 1. Student Entity

**Bảng**: `Students`
**Primary Key**: `StudentId` (Strong-typed GUID wrapper)

| Cột | Kiểu | Ràng buộc | Mô tả |
|-----|------|-----------|-------|
| Id | GUID | PK, NOT NULL | ID duy nhất của sinh viên |
| FirstName | NVARCHAR(50) | NOT NULL | Tên của sinh viên (2-50 ký tự) |
| LastName | NVARCHAR(50) | NOT NULL | Họ của sinh viên (2-50 ký tự) |
| Email | NVARCHAR(255) | NOT NULL, UNIQUE | Email của sinh viên (đã validate) |
| DateOfBirth | DATE | NOT NULL | Ngày sinh (13-120 tuổi) |
| EnrollmentDate | DATETIME | NOT NULL | Ngày đăng ký ban đầu |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Cờ trạng thái active |
| CreatedAt | DATETIME | NOT NULL | Timestamp tạo record |
| UpdatedAt | DATETIME | NOT NULL | Timestamp cập nhật cuối |

**Quy Tắc Domain**:
- Tên phải có 2-50 ký tự, được trim
- Tuổi phải từ 13-120 năm
- Validate format email với ràng buộc uniqueness
- Quản lý timestamp tự động
- Soft delete qua cờ `IsActive`

**Phương Thức Nghiệp Vụ**:
- `CalculateGPA()` - Tính GPA từ các enrollments đã hoàn thành
- `AddEnrollment()` - Validate và thêm đăng ký khóa học
- `UpdatePersonalInfo()` - Cập nhật tên và email với validation
- `Activate()` / `Deactivate()` - Quản lý trạng thái active

### 2. Course Entity

**Bảng**: `Courses`
**Primary Key**: `Id` (GUID)

| Cột | Kiểu | Ràng buộc | Mô tả |
|-----|------|-----------|-------|
| Id | GUID | PK, NOT NULL | ID duy nhất của khóa học |
| Code | NVARCHAR(20) | NOT NULL, UNIQUE | Mã khóa học (vd: "CS101") |
| Name | NVARCHAR(100) | NOT NULL | Tên khóa học (3-100 ký tự) |
| Description | NVARCHAR(500) | NULL | Mô tả khóa học |
| CreditHours | INT | NOT NULL | Số tín chỉ (1-10) |
| Department | NVARCHAR(50) | NOT NULL | Tên khoa (2-50 ký tự) |
| MaxEnrollment | INT | NOT NULL, DEFAULT 30 | Số sinh viên tối đa (1-500) |
| Prerequisites | NVARCHAR(MAX) | NULL | Danh sách course IDs (phân tách bằng dấu phẩy) |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Cờ trạng thái active |
| CreatedAt | DATETIME | NOT NULL | Timestamp tạo record |
| UpdatedAt | DATETIME | NOT NULL | Timestamp cập nhật cuối |

**Quy Tắc Domain**:
- Mã khóa học tuân theo định dạng validation của tổ chức
- Số tín chỉ giới hạn từ 1-10
- Validate tên khoa (2-50 ký tự)
- Prerequisites được lưu dưới dạng serialized GUID list
- Validate sức chứa tối đa (1-500)

**Phương Thức Nghiệp Vụ**:
- `CanEnroll()` - Kiểm tra tính khả dụng và trạng thái active
- `AddPrerequisite()` / `RemovePrerequisite()` - Quản lý prerequisites
- `AddEnrollment()` - Validate sức chứa đăng ký
- `UpdateCourseInfo()` - Cập nhật chi tiết khóa học với validation

### 3. Enrollment Entity

**Bảng**: `Enrollments`
**Primary Key**: `Id` (GUID)

| Cột | Kiểu | Ràng buộc | Mô tả |
|-----|------|-----------|-------|
| Id | GUID | PK, NOT NULL | ID duy nhất của đăng ký |
| StudentId | GUID | FK, NOT NULL | Tham chiếu đến Student |
| CourseId | GUID | FK, NOT NULL | Tham chiếu đến Course |
| EnrollmentDate | DATETIME | NOT NULL | Timestamp đăng ký |
| CompletionDate | DATETIME | NULL | Ngày hoàn thành/rút lui |
| Status | INT | NOT NULL | Enum trạng thái đăng ký |
| CreditHours | INT | NOT NULL | Số tín chỉ cho đăng ký này |
| GradeId | GUID | FK, NULL | Tham chiếu đến Grade (optional) |
| CreatedAt | DATETIME | NOT NULL | Timestamp tạo record |
| UpdatedAt | DATETIME | NOT NULL | Timestamp cập nhật cuối |

**Enrollment Status Enum**:
- `Active (1)` - Đang đăng ký
- `Completed (2)` - Đã hoàn thành thành công
- `Withdrawn (3)` - Đã rút khỏi khóa học

**Quy Tắc Domain**:
- Validate số tín chỉ (1-10)
- Một enrollment active cho mỗi sinh viên mỗi khóa học
- Chuyển đổi trạng thái: Active → Completed/Withdrawn
- Yêu cầu điểm trước khi hoàn thành
- Tự động set completion date khi thay đổi trạng thái

**Phương Thức Nghiệp Vụ**:
- `AssignGrade()` - Gán điểm cho enrollment active
- `Complete()` - Đánh dấu là đã hoàn thành (yêu cầu điểm)
- `Withdraw()` - Rút khỏi khóa học
- `Reactivate()` - Kích hoạt lại enrollment đã rút

### 4. Grade Entity

**Bảng**: `Grades`
**Primary Key**: `Id` (GUID)

| Cột | Kiểu | Ràng buộc | Mô tả |
|-----|------|-----------|-------|
| Id | GUID | PK, NOT NULL | ID duy nhất của điểm |
| LetterGrade | NVARCHAR(5) | NOT NULL | Điểm chữ (A+, A, B+, v.v.) |
| GradePoints | DECIMAL(3,2) | NOT NULL | Điểm GPA (0.0-4.0) |
| NumericScore | DECIMAL(5,2) | NULL | Điểm số (0-100) |
| Comments | NVARCHAR(500) | NULL | Nhận xét của giảng viên |
| GradedDate | DATETIME | NOT NULL | Ngày chấm điểm |
| GradedBy | NVARCHAR(100) | NOT NULL | ID giảng viên |
| CreatedAt | DATETIME | NOT NULL | Timestamp tạo record |
| UpdatedAt | DATETIME | NOT NULL | Timestamp cập nhật cuối |

**Điểm Chữ**: A+, A, A-, B+, B, B-, C+, C, C-, D+, D, D-, F, I (Incomplete), W (Withdrawn)

**Thang Điểm GPA**:
- A+ / A: 4.0
- A-: 3.7
- B+: 3.3
- B: 3.0
- B-: 2.7
- C+: 2.3
- C: 2.0
- C-: 1.7
- D+: 1.3
- D: 1.0
- D-: 0.7
- F: 0.0

**Quy Tắc Domain**:
- Validate điểm chữ với các giá trị được phép
- Validate phạm vi grade points (0.0-4.0)
- Validate điểm số (0-100)
- Giới hạn độ dài nhận xét (500 ký tự)
- Tự động tính grade points từ điểm số

**Phương Thức Nghiệp Vụ**:
- `CreateFromNumericScore()` - Tự động chuyển đổi điểm số sang điểm chữ
- `UpdateGrade()` - Cập nhật thông tin điểm với validation
- `UpdateComments()` - Cập nhật nhận xét của giảng viên

## Value Objects

### 1. StudentId
Strong-typed wrapper cho GUID đảm bảo type safety cho student identifiers.

### 2. Email
Value object với validation format email tích hợp và chuẩn hóa case.

### 3. CourseCode
Mã khóa học đã validate tuân theo tiêu chuẩn tổ chức (vd: "CS101", "MATH200").

### 4. GPA
Calculated GPA value object với validation precision (phạm vi 0.0-4.0).

## Base Entity

Tất cả entities kế thừa từ `BaseEntity<TId>` cung cấp:

- **Id**: Primary key kiểu `TId`
- **CreatedAt**: Timestamp tạo tự động
- **UpdatedAt**: Timestamp cập nhật tự động
- **UpdateTimestamp()**: Cập nhật timestamp thủ công
- **Equality**: So sánh equality dựa trên ID
- **Operators**: Overload toán tử == và !=

## Mối Quan Hệ Database

### Quan Hệ One-to-Many

1. **Student → Enrollments**
   - Một sinh viên có thể có nhiều enrollments
   - Foreign Key: `Enrollment.StudentId → Student.Id`
   - Navigation: `Student.Enrollments` (read-only collection)

2. **Course → Enrollments**
   - Một khóa học có thể có nhiều enrollments
   - Foreign Key: `Enrollment.CourseId → Course.Id`
   - Navigation: `Course.Enrollments` (read-only collection)

### Quan Hệ One-to-One

3. **Enrollment → Grade**
   - Một enrollment có thể có không hoặc một điểm
   - Foreign Key: `Enrollment.GradeId → Grade.Id`
   - Navigation: `Enrollment.Grade` (nullable)

### Quan Hệ Self-Referencing

4. **Course → Prerequisites**
   - Many-to-many self-reference thông qua serialized GUID list
   - Lưu trữ: JSON array trong cột `Course.Prerequisites`
   - Navigation: `Course.Prerequisites` (read-only GUID collection)

## Cấu Hình Entity Framework

### Tính Năng Chính

- **Strong Typing**: Custom value objects cho IDs và domains
- **Value Conversions**: Chuyển đổi Email, CourseCode, StudentId
- **JSON Serialization**: Prerequisites được lưu dưới dạng JSON array
- **Cascade Rules**: Cấu hình cascade delete phù hợp
- **Indexes**: Tối ưu hiệu suất với indexing chiến lược
- **Constraints**: Thực thi validation ở database-level

### File Cấu Hình

- `StudentConfiguration.cs` - Mapping Student entity
- `CourseConfiguration.cs` - Mapping Course entity và prerequisites với value comparer
- `EnrollmentConfiguration.cs` - Relationships và constraints của Enrollment
- `GradeConfiguration.cs` - Mapping và validations của Grade entity

### Cải Tiến Gần Đây

- **Value Comparer cho Prerequisites**: Thêm so sánh collection phù hợp cho Course prerequisites để loại bỏ cảnh báo EF Core và đảm bảo change tracking chính xác

## Cấu Trúc Dữ Liệu Mẫu

```json
{
  "Students": [
    {
      "Id": "550e8400-e29b-41d4-a716-446655440001",
      "FirstName": "Nguyễn Văn",
      "LastName": "An",
      "Email": "nvan.an@university.edu",
      "DateOfBirth": "2000-01-15",
      "EnrollmentDate": "2024-09-01",
      "IsActive": true
    }
  ],
  "Courses": [
    {
      "Id": "550e8400-e29b-41d4-a716-446655440002",
      "Code": "CS101",
      "Name": "Nhập Môn Khoa Học Máy Tính",
      "Description": "Các khái niệm lập trình cơ bản",
      "CreditHours": 3,
      "Department": "Khoa Học Máy Tính",
      "MaxEnrollment": 30,
      "Prerequisites": "[]",
      "IsActive": true
    }
  ],
  "Enrollments": [
    {
      "Id": "550e8400-e29b-41d4-a716-446655440003",
      "StudentId": "550e8400-e29b-41d4-a716-446655440001",
      "CourseId": "550e8400-e29b-41d4-a716-446655440002",
      "EnrollmentDate": "2024-09-01",
      "Status": 1,
      "CreditHours": 3,
      "GradeId": null
    }
  ]
}
```

## Ràng Buộc & Indexes Database

### Primary Keys
- Tất cả entities sử dụng GUID primary keys cho tính duy nhất toàn cục
- Clustered indexes trên primary keys cho hiệu suất

### Unique Constraints
- `Students.Email` - Ràng buộc unique
- `Courses.Code` - Ràng buộc unique

### Foreign Key Constraints
- `Enrollments.StudentId → Students.Id`
- `Enrollments.CourseId → Courses.Id`
- `Enrollments.GradeId → Grades.Id` (nullable)

### Check Constraints
- Validation số tín chỉ (1-10)
- Validation grade points (0.0-4.0)
- Validation tuổi (13-120 năm)
- Validation enrollment status (1-3)

### Performance Indexes
- `IX_Enrollments_StudentId` - Truy vấn enrollment của sinh viên
- `IX_Enrollments_CourseId` - Truy vấn enrollment của khóa học
- `IX_Students_Email` - Tối ưu lookup email
- `IX_Courses_Code` - Tìm kiếm mã khóa học
- `IX_Courses_Department` - Filtering theo khoa

## Migration History

Database schema được quản lý thông qua Entity Framework Core migrations:

- **Initial Migration**: Cấu trúc entity cơ bản
- **Course Prerequisites**: JSON serialization cho prerequisites
- **Grade System**: Triển khai hệ thống chấm điểm toàn diện
- **Performance Indexes**: Tối ưu hiệu suất truy vấn
- **Validation Constraints**: Thực thi quy tắc domain

## Thống Kê Database Hiện Tại

### Entities & Value Objects
- **4 Core Entities**: Student, Course, Enrollment, Grade
- **4 Value Objects**: StudentId, Email, CourseCode, GPA
- **5 Repository Interfaces**: IStudentRepository, ICourseRepository, IEnrollmentRepository, IGradeRepository, IUnitOfWork

### Test Coverage
- **41 Unit Tests**: Bao phủ tất cả entity behaviors
- **100% Pass Rate**: Tất cả tests pass thành công
- **Comprehensive Coverage**: Student (14), Course (11), Enrollment (9), Grade (7)

---

**Tạo ngày**: 2025-12-02
**Hệ Thống Quản Lý Sinh Viên v1.0**
**Clean Architecture với Domain-Driven Design**
