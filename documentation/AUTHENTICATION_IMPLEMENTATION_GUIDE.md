# Hướng Dẫn Implement Authentication Flow - Dành Cho Fresher

## 📋 Tổng Quan

Đây là hướng dẫn chi tiết từng bước để implement User entity và JWT authentication flow trong dự án Student Management System. Hướng dẫn này được thiết kế cho người mới bắt đầu với .NET và Clean Architecture.

### Mục Tiêu
- Tạo User entity trong Domain layer
- Implement authentication commands/queries trong Application layer
- Tạo JWT service trong Infrastructure layer
- Tạo AuthController trong WebApi layer
- Bảo vệ các endpoints với [Authorize] attribute

### Thời Gian Ước Tính
- **Domain Layer**: 2-3 giờ
- **Application Layer**: 3-4 giờ
- **Infrastructure Layer**: 2-3 giờ
- **WebApi Layer**: 2-3 giờ
- **Testing**: 1-2 giờ
- **Tổng**: 10-15 giờ

---

## 📚 Kiến Thức Cần Có

### 1. Concepts Cần Hiểu
- **Entity**: Object có identity (ID), có lifecycle
- **Value Object**: Object không có identity, immutable
- **Aggregate Root**: Entity chính quản lý các entities khác
- **Command**: Request để thay đổi state (Create, Update, Delete)
- **Query**: Request để đọc dữ liệu (Get, List)
- **JWT (JSON Web Token)**: Token để authenticate user
- **Password Hashing**: Mã hóa password để bảo mật

### 2. NuGet Packages Cần Cài
```bash
# Vào thư mục Infrastructure
cd src/StudentManagement.Infrastructure

# Cài JWT và BCrypt packages
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.4
dotnet add package BCrypt.Net-Next --version 4.0.3

# Vào thư mục WebApi
cd ../StudentManagement.WebApi

# Đảm bảo có JWT package
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.4
```

---

## 🎯 PHASE 1: DOMAIN LAYER (2-3 giờ)

### Mục Tiêu
Tạo User entity, Role enum, và các value objects liên quan.

---

### **STEP 1.1: Tạo Role Enum**

📁 **File**: `src/StudentManagement.Domain/Common/Enums/UserRole.cs`

```csharp
namespace StudentManagement.Domain.Common.Enums;

/// <summary>
/// Các vai trò của user trong hệ thống
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Quản trị viên - có toàn quyền
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Giáo viên - quản lý khóa học và điểm
    /// </summary>
    Teacher = 2,

    /// <summary>
    /// Sinh viên - xem thông tin của mình
    /// </summary>
    Student = 3,

    /// <summary>
    /// Nhân viên - chức năng quản trị
    /// </summary>
    Staff = 4
}
```

**❓ Giải thích**:
- `enum`: Kiểu dữ liệu định nghĩa tập hợp các hằng số
- Mỗi role có giá trị số (1, 2, 3, 4)
- XML comments (`///`) giúp hiển thị documentation

---

### **STEP 1.2: Tạo Username Value Object**

📁 **File**: `src/StudentManagement.Domain/ValueObjects/Username.cs`

```csharp
using System.Text.RegularExpressions;

namespace StudentManagement.Domain.ValueObjects;

/// <summary>
/// Value object cho Username
/// Username phải từ 3-50 ký tự, chỉ chứa chữ, số, underscore, dấu chấm
/// </summary>
public class Username : IEquatable<Username>
{
    public string Value { get; }

    // Constructor private để bắt buộc dùng factory method
    private Username(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Factory method để tạo Username với validation
    /// </summary>
    public static Username Create(string value)
    {
        // Validation 1: Null hoặc empty
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Username không được để trống");
        }

        // Validation 2: Độ dài
        if (value.Length < 3 || value.Length > 50)
        {
            throw new ArgumentException("Username phải từ 3-50 ký tự");
        }

        // Validation 3: Format (chỉ chữ, số, underscore, dấu chấm)
        var regex = new Regex(@"^[a-zA-Z0-9_.]+$");
        if (!regex.IsMatch(value))
        {
            throw new ArgumentException("Username chỉ được chứa chữ, số, underscore và dấu chấm");
        }

        return new Username(value.ToLower()); // Normalize về lowercase
    }

    // Implement IEquatable để so sánh
    public bool Equals(Username? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Username);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    // Operator overloading để so sánh
    public static bool operator ==(Username? left, Username? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Username? left, Username? right)
    {
        return !(left == right);
    }
}
```

**❓ Giải thích**:
- **Value Object**: Object không có ID, được định nghĩa bởi giá trị của nó
- **Factory Method** (`Create`): Pattern để tạo object với validation
- **IEquatable**: Interface để so sánh 2 objects
- **Regex**: Regular expression để validate format
- **Operator overloading**: Cho phép dùng `==` và `!=`

---

### **STEP 1.3: Tạo PasswordHash Value Object**

📁 **File**: `src/StudentManagement.Domain/ValueObjects/PasswordHash.cs`

```csharp
namespace StudentManagement.Domain.ValueObjects;

/// <summary>
/// Value object cho password đã hash
/// Không bao giờ lưu plain password!
/// </summary>
public class PasswordHash : IEquatable<PasswordHash>
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Tạo từ password đã hash (từ database)
    /// </summary>
    public static PasswordHash FromHash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            throw new ArgumentException("Password hash không được để trống");
        }

        return new PasswordHash(hashedPassword);
    }

    public bool Equals(PasswordHash? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PasswordHash);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(PasswordHash? left, PasswordHash? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(PasswordHash? left, PasswordHash? right)
    {
        return !(left == right);
    }
}
```

**❓ Giải thích**:
- Lưu password đã hash, KHÔNG BAO GIỜ lưu plain password
- Chỉ có 1 factory method `FromHash` để tạo từ password đã hash
- Password hashing sẽ được xử lý ở Infrastructure layer

---

### **STEP 1.4: Tạo RefreshToken Entity**

📁 **File**: `src/StudentManagement.Domain/Entities/RefreshToken.cs`

```csharp
namespace StudentManagement.Domain.Entities;

/// <summary>
/// Entity để quản lý refresh tokens
/// Refresh token dùng để lấy access token mới khi token cũ hết hạn
/// </summary>
public class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string CreatedByIp { get; private set; }

    // Computed property
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Constructor private để bắt buộc dùng factory method
    private RefreshToken() { }

    /// <summary>
    /// Factory method để tạo refresh token mới
    /// </summary>
    public static RefreshToken Create(
        Guid userId,
        string token,
        int expiryDays,
        string createdByIp)
    {
        // Validations
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID không hợp lệ");

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token không được để trống");

        if (expiryDays <= 0)
            throw new ArgumentException("Expiry days phải > 0");

        if (string.IsNullOrWhiteSpace(createdByIp))
            throw new ArgumentException("Created by IP không được để trống");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = createdByIp
        };
    }

    /// <summary>
    /// Revoke (thu hồi) refresh token
    /// </summary>
    public void Revoke(string revokedByIp, string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new InvalidOperationException("Token đã bị revoke rồi");

        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}
```

**❓ Giải thích**:
- **RefreshToken**: Token dùng để lấy access token mới
- **Computed properties**: Properties tính toán (IsExpired, IsRevoked, IsActive)
- **Revoke**: Thu hồi token khi không còn dùng
- **IP tracking**: Lưu IP address để security audit

---

### **STEP 1.5: Tạo User Entity (Aggregate Root)**

📁 **File**: `src/StudentManagement.Domain/Entities/User.cs`

```csharp
using StudentManagement.Domain.Common.Enums;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Entities;

/// <summary>
/// User entity - Aggregate Root
/// Quản lý authentication và authorization
/// </summary>
public class User : Entity<Guid>
{
    // Properties
    public Username Username { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Navigation property
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    // Computed property
    public string FullName => $"{FirstName} {LastName}";

    // Constructor private
    private User() { }

    /// <summary>
    /// Factory method để tạo User mới
    /// </summary>
    public static User Create(
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        UserRole role)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name không được để trống");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name không được để trống");

        if (firstName.Length > 50)
            throw new ArgumentException("First name không được quá 50 ký tự");

        if (lastName.Length > 50)
            throw new ArgumentException("Last name không được quá 50 ký tự");

        // Tạo user mới
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = Username.Create(username),
            Email = Email.Create(email),
            PasswordHash = PasswordHash.FromHash(passwordHash),
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return user;
    }

    /// <summary>
    /// Cập nhật thông tin user
    /// </summary>
    public void UpdateInfo(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name không được để trống");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name không được để trống");

        FirstName = firstName;
        LastName = lastName;
        Email = Email.Create(email);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đổi password
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash không được để trống");

        PasswordHash = PasswordHash.FromHash(newPasswordHash);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đổi role
    /// </summary>
    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activate user
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("User đã active rồi");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivate user
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("User đã bị deactivate rồi");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật last login time
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Thêm refresh token
    /// </summary>
    public void AddRefreshToken(RefreshToken token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        _refreshTokens.Add(token);
    }

    /// <summary>
    /// Revoke tất cả refresh tokens
    /// </summary>
    public void RevokeAllRefreshTokens(string revokedByIp)
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke(revokedByIp);
        }
    }

    /// <summary>
    /// Xóa các refresh tokens đã expired
    /// </summary>
    public void RemoveExpiredRefreshTokens()
    {
        _refreshTokens.RemoveAll(t => t.IsExpired);
    }
}
```

**❓ Giải thích**:
- **Aggregate Root**: Entity chính quản lý các entities khác (RefreshToken)
- **Encapsulation**: Tất cả properties là `private set`, chỉ có thể thay đổi qua methods
- **Business methods**: `UpdateInfo`, `ChangePassword`, `Activate`, `Deactivate`, etc.
- **Validation**: Mỗi method đều có validation
- **DateTime.UtcNow**: Dùng UTC để tránh vấn đề timezone

---

### **STEP 1.6: Tạo IUserRepository Interface**

📁 **File**: `src/StudentManagement.Domain/Repositories/IUserRepository.cs`

```csharp
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Repositories;

/// <summary>
/// Repository interface cho User entity
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// Lấy user theo username
    /// </summary>
    Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy user theo email
    /// </summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra username đã tồn tại chưa
    /// </summary>
    Task<bool> IsUsernameUniqueAsync(Username username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra email đã tồn tại chưa
    /// </summary>
    Task<bool> IsEmailUniqueAsync(Email email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy user với refresh tokens
    /// </summary>
    Task<User?> GetWithRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy user theo refresh token
    /// </summary>
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
```

**❓ Giải thích**:
- **Interface**: Contract định nghĩa các methods mà repository phải implement
- `Task<T>`: Async method trả về type T
- `?`: Nullable type (có thể null)
- `CancellationToken`: Để cancel async operation

---

### **STEP 1.7: Update IUnitOfWork**

📁 **File**: `src/StudentManagement.Domain/Repositories/IUnitOfWork.cs`

Thêm property `Users`:

```csharp
public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    ICourseRepository Courses { get; }
    IEnrollmentRepository Enrollments { get; }
    IUserRepository Users { get; } // ✅ THÊM DÒNG NÀY

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

---

### ✅ **CHECKPOINT 1: Domain Layer Complete**

**Kiểm tra:**
- [ ] Đã tạo `UserRole` enum
- [ ] Đã tạo `Username` value object
- [ ] Đã tạo `PasswordHash` value object
- [ ] Đã tạo `RefreshToken` entity
- [ ] Đã tạo `User` entity
- [ ] Đã tạo `IUserRepository` interface
- [ ] Đã update `IUnitOfWork`

**Build để kiểm tra lỗi:**
```bash
dotnet build src/StudentManagement.Domain
```

Nếu không có lỗi, chuyển sang Phase 2!

---

## 🎯 PHASE 2: APPLICATION LAYER (3-4 giờ)

### Mục Tiêu
Tạo DTOs, Commands, Queries, Validators và Handlers cho authentication.

---

### **STEP 2.1: Tạo Authentication DTOs**

📁 **File**: `src/StudentManagement.Application/DTOs/AuthenticationDtos.cs`

```csharp
namespace StudentManagement.Application.DTOs;

// ============ REQUEST DTOs ============

/// <summary>
/// DTO cho request đăng ký user mới
/// </summary>
public record RegisterRequestDto
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = "Student"; // Default role
}

/// <summary>
/// DTO cho request đăng nhập
/// </summary>
public record LoginRequestDto
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// DTO cho request refresh token
/// </summary>
public record RefreshTokenRequestDto
{
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// DTO cho request đổi password
/// </summary>
public record ChangePasswordRequestDto
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}

// ============ RESPONSE DTOs ============

/// <summary>
/// DTO cho response sau khi đăng nhập thành công
/// </summary>
public record AuthenticationResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}

/// <summary>
/// DTO cho User
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}
```

**❓ Giải thích**:
- **record**: C# record type, immutable by default
- **init**: Property chỉ có thể set khi khởi tạo
- **DTO**: Data Transfer Object, object để truyền dữ liệu giữa layers

---

### **STEP 2.2: Tạo IPasswordHasher Interface**

📁 **File**: `src/StudentManagement.Application/Interfaces/IPasswordHasher.cs`

```csharp
namespace StudentManagement.Application.Interfaces;

/// <summary>
/// Interface cho password hashing service
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify password với hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
```

---

### **STEP 2.3: Tạo IJwtTokenService Interface**

📁 **File**: `src/StudentManagement.Application/Interfaces/IJwtTokenService.cs`

```csharp
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Interfaces;

/// <summary>
/// Interface cho JWT token service
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate access token (JWT)
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate access token
    /// </summary>
    bool ValidateToken(string token);

    /// <summary>
    /// Get user ID từ token
    /// </summary>
    Guid? GetUserIdFromToken(string token);
}
```

---

### **STEP 2.4: Tạo RegisterCommand**

📁 **File**: `src/StudentManagement.Application/Commands/Authentication/RegisterCommand.cs`

```csharp
using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
/// Command để đăng ký user mới
/// </summary>
public record RegisterCommand : IRequest<ApiResponseDto<UserDto>>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = "Student";

    public static RegisterCommand FromDto(RegisterRequestDto dto)
    {
        return new RegisterCommand
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            ConfirmPassword = dto.ConfirmPassword,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = dto.Role
        };
    }
}
```

**❓ Giải thích**:
- **IRequest<T>**: MediatR interface, định nghĩa command trả về type T
- **record**: Immutable data structure
- **FromDto**: Factory method để tạo command từ DTO

---

### **STEP 2.5: Tạo RegisterCommandValidator**

📁 **File**: `src/StudentManagement.Application/Validators/Authentication/RegisterCommandValidator.cs`

```csharp
using FluentValidation;
using StudentManagement.Application.Commands.Authentication;

namespace StudentManagement.Application.Validators.Authentication;

/// <summary>
/// Validator cho RegisterCommand
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        // Username validation
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username không được để trống")
            .Length(3, 50).WithMessage("Username phải từ 3-50 ký tự")
            .Matches(@"^[a-zA-Z0-9_.]+$").WithMessage("Username chỉ được chứa chữ, số, underscore và dấu chấm");

        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ")
            .MaximumLength(255).WithMessage("Email không được quá 255 ký tự");

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password không được để trống")
            .MinimumLength(8).WithMessage("Password phải ít nhất 8 ký tự")
            .Matches(@"[A-Z]").WithMessage("Password phải có ít nhất 1 chữ hoa")
            .Matches(@"[a-z]").WithMessage("Password phải có ít nhất 1 chữ thường")
            .Matches(@"[0-9]").WithMessage("Password phải có ít nhất 1 số")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password phải có ít nhất 1 ký tự đặc biệt");

        // Confirm password validation
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password không được để trống")
            .Equal(x => x.Password).WithMessage("Confirm password không khớp với password");

        // First name validation
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name không được để trống")
            .MaximumLength(50).WithMessage("First name không được quá 50 ký tự");

        // Last name validation
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name không được để trống")
            .MaximumLength(50).WithMessage("Last name không được quá 50 ký tự");

        // Role validation
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role không được để trống")
            .Must(BeValidRole).WithMessage("Role không hợp lệ");
    }

    private bool BeValidRole(string role)
    {
        var validRoles = new[] { "Admin", "Teacher", "Student", "Staff" };
        return validRoles.Contains(role);
    }
}
```

**❓ Giải thích**:
- **AbstractValidator<T>**: Base class của FluentValidation
- **RuleFor**: Định nghĩa rule cho property
- **WithMessage**: Custom error message
- **Matches**: Validate với regex
- **Must**: Custom validation logic

---

### **STEP 2.6: Tạo RegisterCommandHandler**

📁 **File**: `src/StudentManagement.Application/Commands/Authentication/RegisterCommandHandler.cs`

```csharp
using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Common.Enums;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
/// Handler cho RegisterCommand
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponseDto<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<ApiResponseDto<UserDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Kiểm tra username đã tồn tại chưa
            var username = Username.Create(request.Username);
            var isUsernameUnique = await _unitOfWork.Users.IsUsernameUniqueAsync(username, cancellationToken: cancellationToken);
            if (!isUsernameUnique)
            {
                return ApiResponseDto<UserDto>.ErrorResult(
                    new[] { "Username đã tồn tại" });
            }

            // 2. Kiểm tra email đã tồn tại chưa
            var email = Email.Create(request.Email);
            var isEmailUnique = await _unitOfWork.Users.IsEmailUniqueAsync(email, cancellationToken: cancellationToken);
            if (!isEmailUnique)
            {
                return ApiResponseDto<UserDto>.ErrorResult(
                    new[] { "Email đã tồn tại" });
            }

            // 3. Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 4. Parse role
            if (!Enum.TryParse<UserRole>(request.Role, out var userRole))
            {
                return ApiResponseDto<UserDto>.ErrorResult(
                    new[] { "Role không hợp lệ" });
            }

            // 5. Tạo user entity
            var user = User.Create(
                username: request.Username,
                email: request.Email,
                passwordHash: passwordHash,
                firstName: request.FirstName,
                lastName: request.LastName,
                role: userRole
            );

            // 6. Thêm vào database
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Map sang DTO và return
            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(
                userDto,
                "Đăng ký thành công");
        }
        catch (ArgumentException ex)
        {
            return ApiResponseDto<UserDto>.ErrorResult(
                new[] { ex.Message });
        }
        catch (Exception ex)
        {
            return ApiResponseDto<UserDto>.ErrorResult(
                new[] { "Có lỗi xảy ra khi đăng ký user", ex.Message });
        }
    }
}
```

**❓ Giải thích**:
- **IRequestHandler<TRequest, TResponse>**: Handler xử lý command/query
- **Dependency Injection**: Constructor nhận dependencies qua DI
- **try-catch**: Bắt và xử lý exceptions
- **Business logic flow**: Validate → Hash password → Create entity → Save → Return

---

### **STEP 2.7: Tạo LoginCommand**

📁 **File**: `src/StudentManagement.Application/Commands/Authentication/LoginCommand.cs`

```csharp
using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
/// Command để đăng nhập
/// </summary>
public record LoginCommand : IRequest<ApiResponseDto<AuthenticationResponseDto>>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; } // IP address của client

    public static LoginCommand FromDto(LoginRequestDto dto, string? ipAddress = null)
    {
        return new LoginCommand
        {
            Username = dto.Username,
            Password = dto.Password,
            IpAddress = ipAddress
        };
    }
}
```

---

### **STEP 2.8: Tạo LoginCommandValidator**

📁 **File**: `src/StudentManagement.Application/Validators/Authentication/LoginCommandValidator.cs`

```csharp
using FluentValidation;
using StudentManagement.Application.Commands.Authentication;

namespace StudentManagement.Application.Validators.Authentication;

/// <summary>
/// Validator cho LoginCommand
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username không được để trống");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password không được để trống");
    }
}
```

---

### **STEP 2.9: Tạo LoginCommandHandler**

📁 **File**: `src/StudentManagement.Application/Commands/Authentication/LoginCommandHandler.cs`

```csharp
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
/// Handler cho LoginCommand
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponseDto<AuthenticationResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<ApiResponseDto<AuthenticationResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Tìm user theo username
            var username = Username.Create(request.Username);
            var user = await _unitOfWork.Users.GetByUsernameAsync(username, cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                    new[] { "Username hoặc password không đúng" });
            }

            // 2. Kiểm tra user có active không
            if (!user.IsActive)
            {
                return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                    new[] { "Tài khoản đã bị khóa" });
            }

            // 3. Verify password
            var isPasswordValid = _passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash.Value);

            if (!isPasswordValid)
            {
                return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                    new[] { "Username hoặc password không đúng" });
            }

            // 4. Generate access token
            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            // 5. Generate refresh token
            var refreshTokenString = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiryDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays");

            var refreshToken = RefreshToken.Create(
                userId: user.Id,
                token: refreshTokenString,
                expiryDays: refreshTokenExpiryDays,
                createdByIp: request.IpAddress ?? "unknown"
            );

            // 6. Thêm refresh token vào user
            user.AddRefreshToken(refreshToken);
            user.UpdateLastLogin();

            // 7. Xóa các refresh tokens đã expired
            user.RemoveExpiredRefreshTokens();

            // 8. Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Map sang DTO
            var userDto = _mapper.Map<UserDto>(user);
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

            var response = new AuthenticationResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                User = userDto
            };

            return ApiResponseDto<AuthenticationResponseDto>.SuccessResult(
                response,
                "Đăng nhập thành công");
        }
        catch (ArgumentException ex)
        {
            return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                new[] { ex.Message });
        }
        catch (Exception ex)
        {
            return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                new[] { "Có lỗi xảy ra khi đăng nhập", ex.Message });
        }
    }
}
```

**❓ Giải thích**:
- **Authentication flow**: Find user → Verify password → Generate tokens → Save → Return
- **Security**: Không reveal user existence (message chung "username hoặc password không đúng")
- **IConfiguration**: Đọc config từ appsettings.json

---

### **STEP 2.10: Tạo User Mapping Profile**

📁 **File**: `src/StudentManagement.Application/Mappings/UserMappingProfile.cs`

```csharp
using AutoMapper;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile cho User
/// </summary>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}
```

**❓ Giải thích**:
- **Profile**: AutoMapper profile định nghĩa các mappings
- **ForMember**: Custom mapping cho property
- **MapFrom**: Lấy giá trị từ source property

---

### ✅ **CHECKPOINT 2: Application Layer Complete (Part 1)**

**Kiểm tra:**
- [ ] Đã tạo Authentication DTOs
- [ ] Đã tạo `IPasswordHasher` interface
- [ ] Đã tạo `IJwtTokenService` interface
- [ ] Đã tạo `RegisterCommand` và handler
- [ ] Đã tạo `RegisterCommandValidator`
- [ ] Đã tạo `LoginCommand` và handler
- [ ] Đã tạo `LoginCommandValidator`
- [ ] Đã tạo `UserMappingProfile`

**Note**: Còn RefreshToken command và các queries khác sẽ implement sau. Giờ chuyển sang Phase 3!

---

## 🎯 PHASE 3: INFRASTRUCTURE LAYER (2-3 giờ)

### Mục Tiêu
Implement PasswordHasher, JwtTokenService, UserRepository và EF Core configurations.

---

### **STEP 3.1: Implement PasswordHasher Service**

📁 **File**: `src/StudentManagement.Infrastructure/Services/PasswordHasher.cs`

```csharp
using StudentManagement.Application.Interfaces;
using BCrypt.Net;

namespace StudentManagement.Infrastructure.Services;

/// <summary>
/// Service để hash và verify password sử dụng BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hash password với BCrypt
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password không được để trống");

        // BCrypt.HashPassword tự động generate salt và hash
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// Verify password với hash
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }
}
```

**❓ Giải thích**:
- **BCrypt**: Algorithm để hash password securely
- **workFactor (12)**: Cost factor, càng cao càng secure nhưng càng chậm
- **Salt**: BCrypt tự động generate salt unique cho mỗi password
- **Verify**: So sánh password với hash một cách an toàn

---

### **STEP 3.2: Implement JWT Token Service**

📁 **File**: `src/StudentManagement.Infrastructure/Services/JwtTokenService.cs`

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Services;

/// <summary>
/// Service để generate và validate JWT tokens
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secret = _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        _issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        _audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
        _expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");
    }

    /// <summary>
    /// Generate JWT access token
    /// </summary>
    public string GenerateAccessToken(User user)
    {
        // 1. Tạo claims (thông tin user trong token)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (User ID)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (unique)
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), // Issued At
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username.Value),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("fullName", user.FullName)
        };

        // 2. Tạo signing key từ secret
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Tạo JWT token
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: credentials
        );

        // 4. Serialize token thành string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generate refresh token (random string)
    /// </summary>
    public string GenerateRefreshToken()
    {
        // Tạo random bytes
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        // Convert sang base64 string
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Không có grace period
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get User ID từ token
    /// </summary>
    public Guid? GetUserIdFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
```

**❓ Giải thích**:
- **JWT**: JSON Web Token, token để authenticate user
- **Claims**: Thông tin user trong token (ID, username, email, role)
- **Signing**: Token được sign với secret key để chống tamper
- **HmacSha256**: Algorithm để sign token
- **Expiry**: Token tự động expire sau X phút
- **Refresh Token**: Random string để lấy access token mới

---

### **STEP 3.3: Tạo User Entity Configuration**

📁 **File**: `src/StudentManagement.Infrastructure/Data/Configurations/UserConfiguration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Common.Enums;

namespace StudentManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration cho User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Username (Value Object)
        builder.Property(u => u.Username)
            .HasConversion(
                username => username.Value,
                value => Domain.ValueObjects.Username.Create(value))
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        // Email (Value Object)
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Domain.ValueObjects.Email.Create(value))
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        // PasswordHash (Value Object)
        builder.Property(u => u.PasswordHash)
            .HasConversion(
                hash => hash.Value,
                value => Domain.ValueObjects.PasswordHash.FromHash(value))
            .IsRequired()
            .HasMaxLength(500);

        // FirstName
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        // LastName
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        // Role (Enum)
        builder.Property(u => u.Role)
            .HasConversion<string>() // Store as string
            .IsRequired()
            .HasMaxLength(20);

        // IsActive
        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Timestamps
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        builder.Property(u => u.LastLoginAt)
            .IsRequired(false);

        // Ignore computed properties
        builder.Ignore(u => u.FullName);

        // Relationships
        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**❓ Giải thích**:
- **IEntityTypeConfiguration**: Interface để config entity
- **HasConversion**: Convert value object ↔ database value
- **HasIndex + IsUnique**: Tạo unique index
- **HasConversion<string>**: Convert enum sang string trong DB
- **HasDefaultValueSql**: Set default value trong DB
- **Ignore**: Không map property vào DB (computed properties)

---

### **STEP 3.4: Tạo RefreshToken Entity Configuration**

📁 **File**: `src/StudentManagement.Infrastructure/Data/Configurations/RefreshTokenConfiguration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration cho RefreshToken entity
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Table name
        builder.ToTable("RefreshTokens");

        // Primary key
        builder.HasKey(rt => rt.Id);

        // UserId
        builder.Property(rt => rt.UserId)
            .IsRequired();

        // Token
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        // Dates
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.CreatedAt)
            .IsRequired();

        builder.Property(rt => rt.RevokedAt)
            .IsRequired(false);

        // IPs and metadata
        builder.Property(rt => rt.CreatedByIp)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(50);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        // Ignore computed properties
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsRevoked);
        builder.Ignore(rt => rt.IsActive);

        // Indexes for query performance
        builder.HasIndex(rt => rt.UserId);
        builder.HasIndex(rt => rt.ExpiresAt);
    }
}
```

---

### **STEP 3.5: Update DbContext**

📁 **File**: `src/StudentManagement.Infrastructure/Data/StudentManagementDbContext.cs`

Thêm DbSets:

```csharp
public DbSet<Student> Students { get; set; } = null!;
public DbSet<Course> Courses { get; set; } = null!;
public DbSet<Enrollment> Enrollments { get; set; } = null!;
public DbSet<Grade> Grades { get; set; } = null!;
public DbSet<User> Users { get; set; } = null!; // ✅ THÊM DÒNG NÀY
public DbSet<RefreshToken> RefreshTokens { get; set; } = null!; // ✅ THÊM DÒNG NÀY
```

Trong `OnModelCreating`, thêm configurations:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Existing configurations
    modelBuilder.ApplyConfiguration(new StudentConfiguration());
    modelBuilder.ApplyConfiguration(new CourseConfiguration());
    modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
    modelBuilder.ApplyConfiguration(new GradeConfiguration());

    // ✅ THÊM 2 DÒNG NÀY
    modelBuilder.ApplyConfiguration(new UserConfiguration());
    modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

    // Existing value object conversions...
}
```

---

### **STEP 3.6: Implement UserRepository**

📁 **File**: `src/StudentManagement.Infrastructure/Repositories/UserRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;
using StudentManagement.Infrastructure.Data;

namespace StudentManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation cho User entity
/// </summary>
public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(StudentManagementDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(
        Username username,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(
        Username username,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(u => u.Username == username);

        if (excludeUserId != null)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(
        Email email,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(u => u.Email == email);

        if (excludeUserId != null)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<User?> GetWithRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken),
                cancellationToken);
    }
}
```

**❓ Giải thích**:
- **Include**: Eager loading, load related entities (RefreshTokens)
- **FirstOrDefaultAsync**: Lấy 1 record hoặc null
- **AnyAsync**: Kiểm tra có record nào match condition không
- **Where**: Filter records

---

### **STEP 3.7: Update UnitOfWork**

📁 **File**: `src/StudentManagement.Infrastructure/Repositories/UnitOfWork.cs`

Thêm Users repository:

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly StudentManagementDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(StudentManagementDbContext context)
    {
        _context = context;
        Students = new StudentRepository(_context);
        Courses = new CourseRepository(_context);
        Enrollments = new EnrollmentRepository(_context);
        Users = new UserRepository(_context); // ✅ THÊM DÒNG NÀY
    }

    public IStudentRepository Students { get; }
    public ICourseRepository Courses { get; }
    public IEnrollmentRepository Enrollments { get; }
    public IUserRepository Users { get; } // ✅ THÊM DÒNG NÀY

    // Rest of the code...
}
```

---

### **STEP 3.8: Update Infrastructure DependencyInjection**

📁 **File**: `src/StudentManagement.Infrastructure/DependencyInjection.cs`

Thêm registrations:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Application.Interfaces; // ✅ THÊM USING NÀY
using StudentManagement.Domain.Repositories;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Infrastructure.Services; // ✅ THÊM USING NÀY

namespace StudentManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<StudentManagementDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>(); // ✅ THÊM DÒNG NÀY

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ✅ THÊM AUTHENTICATION SERVICES
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
```

---

### **STEP 3.9: Tạo Migration**

Mở terminal và chạy:

```bash
# Đảm bảo đang ở root directory của solution
cd D:\My-Project\StudentManagement-NetCore-CleanArchitecture-DDD

# Tạo migration
dotnet ef migrations add AddUserAuthentication -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Apply migration
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

**❓ Giải thích**:
- **migrations add**: Tạo migration mới
- **-p**: Project chứa DbContext
- **-s**: Startup project
- **database update**: Apply migration vào database

---

### ✅ **CHECKPOINT 3: Infrastructure Layer Complete**

**Kiểm tra:**
- [ ] Đã implement `PasswordHasher`
- [ ] Đã implement `JwtTokenService`
- [ ] Đã tạo `UserConfiguration`
- [ ] Đã tạo `RefreshTokenConfiguration`
- [ ] Đã update `DbContext`
- [ ] Đã implement `UserRepository`
- [ ] Đã update `UnitOfWork`
- [ ] Đã update `DependencyInjection`
- [ ] Đã tạo và apply migration

**Build để kiểm tra:**
```bash
dotnet build src/StudentManagement.Infrastructure
```

---

## 🎯 PHASE 4: WEBAPI LAYER (2-3 giờ)

### Mục Tiêu
Tạo AuthController, configure authentication middleware, và protect endpoints.

---

### **STEP 4.1: Configure JWT Authentication**

📁 **File**: `src/StudentManagement.WebApi/DependencyInjection.cs`

Update để thêm Authentication:

```csharp
using System.IO.Compression;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace StudentManagement.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(
        this IServiceCollection services,
        IConfiguration configuration) // ✅ THÊM PARAMETER NÀY
    {
        // Controllers with enhanced configuration
        services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });

        // API Explorer for Swagger
        services.AddEndpointsApiExplorer();

        // ✅ THÊM JWT AUTHENTICATION
        var jwtSecret = configuration["JwtSettings:Secret"];
        var jwtIssuer = configuration["JwtSettings:Issuer"];
        var jwtAudience = configuration["JwtSettings:Audience"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // ✅ THÊM AUTHORIZATION
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
            options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
            options.AddPolicy("TeacherOrAdmin", policy => policy.RequireRole("Teacher", "Admin"));
        });

        // Response compression (existing code)
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = new[]
            {
                "application/json",
                "application/xml",
                "text/plain",
                "text/json",
                "text/xml"
            };
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        // Memory caching
        services.AddMemoryCache();

        // Health checks
        services.AddHealthChecks();

        // ✅ UPDATE SWAGGER CONFIGURATION
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Student Management API",
                Version = "v1",
                Description = @"A comprehensive Student Management System built with Clean Architecture and Domain-Driven Design principles."
            });

            // ✅ THÊM JWT BEARER AUTHENTICATION CHO SWAGGER
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML comments for better documentation
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.EnableAnnotations();
        });

        // CORS (if needed for frontend integration)
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }
}
```

**❓ Giải thích**:
- **AddAuthentication**: Configure authentication scheme
- **JwtBearerDefaults**: Default scheme cho JWT
- **TokenValidationParameters**: Parameters để validate JWT token
- **AddAuthorization**: Configure authorization policies
- **RequireRole**: Policy yêu cầu role cụ thể
- **Swagger Security**: Thêm button "Authorize" trong Swagger UI

---

### **STEP 4.2: Update Program.cs**

📁 **File**: `src/StudentManagement.WebApi/Program.cs`

Update để thêm authentication middleware:

```csharp
using StudentManagement.Application;
using StudentManagement.Infrastructure;
using StudentManagement.WebApi;
using StudentManagement.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container by layer
services.AddApplication();
services.AddInfrastructure(config);
services.AddWebApi(config); // ✅ THÊM PARAMETER config

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Response compression
app.UseResponseCompression();

// Global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable CORS (if configured)
app.UseCors("AllowAll");

// ✅ THÊM AUTHENTICATION & AUTHORIZATION MIDDLEWARE
app.UseAuthentication(); // Phải đặt TRƯỚC UseAuthorization
app.UseAuthorization();

// Health checks
app.MapHealthChecks("/health");

// Map controllers
app.MapControllers();

app.Run();
```

**❓ Giải thích**:
- **UseAuthentication**: Middleware để authenticate requests
- **UseAuthorization**: Middleware để authorize requests
- **Thứ tự quan trọng**: UseAuthentication phải đặt TRƯỚC UseAuthorization

---

### **STEP 4.3: Tạo AuthController**

📁 **File**: `src/StudentManagement.WebApi/Controllers/AuthController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Commands.Authentication;
using StudentManagement.Application.DTOs;

namespace StudentManagement.WebApi.Controllers;

/// <summary>
/// Controller cho authentication và authorization
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Đăng ký user mới
    /// </summary>
    /// <param name="dto">Thông tin đăng ký</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Thông tin user đã tạo</returns>
    [HttpPost("register")]
    [AllowAnonymous] // Cho phép anonymous (chưa đăng nhập)
    public async Task<ActionResult<ApiResponseDto<UserDto>>> Register(
        [FromBody] RegisterRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = RegisterCommand.FromDto(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    /// <param name="dto">Username và password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token và refresh token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponseDto<AuthenticationResponseDto>>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Lấy IP address của client
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = LoginCommand.FromDto(dto, ipAddress);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin user hiện tại (yêu cầu authenticated)
    /// </summary>
    /// <returns>Thông tin user</returns>
    [HttpGet("me")]
    [Authorize] // Yêu cầu authenticated
    public ActionResult<object> GetCurrentUser()
    {
        // Lấy claims từ token
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var fullName = User.FindFirst("fullName")?.Value;

        return Ok(new
        {
            userId,
            username,
            email,
            role,
            fullName
        });
    }

    /// <summary>
    /// Test endpoint cho Admin only
    /// </summary>
    [HttpGet("admin-test")]
    [Authorize(Policy = "AdminOnly")]
    public ActionResult<object> AdminTest()
    {
        return Ok(new { message = "Bạn là Admin!" });
    }

    /// <summary>
    /// Test endpoint cho Teacher only
    /// </summary>
    [HttpGet("teacher-test")]
    [Authorize(Policy = "TeacherOnly")]
    public ActionResult<object> TeacherTest()
    {
        return Ok(new { message = "Bạn là Teacher!" });
    }
}
```

**❓ Giải thích**:
- **[AllowAnonymous]**: Endpoint không cần authentication
- **[Authorize]**: Endpoint yêu cầu authentication
- **[Authorize(Policy = "...")]**: Endpoint yêu cầu policy cụ thể
- **User.FindFirst**: Lấy claim từ authenticated user
- **HttpContext.Connection.RemoteIpAddress**: Lấy IP của client

---

### **STEP 4.4: Protect Existing Controllers (Optional)**

Bạn có thể protect các endpoints hiện có bằng cách thêm `[Authorize]`:

📁 **Example**: `src/StudentManagement.WebApi/Controllers/StudentsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // ✅ THÊM AUTHORIZE Ở CLASS LEVEL (tất cả endpoints cần auth)
public class StudentsController : ControllerBase
{
    // Existing code...

    [HttpGet]
    [Authorize(Roles = "Admin,Teacher,Staff")] // Override: chỉ Admin, Teacher, Staff mới xem list
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<StudentSummaryDto>>>> GetStudents(
        [FromQuery] StudentFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        // Existing code...
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Staff")] // Override: chỉ Admin, Staff mới tạo student
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> CreateStudent(
        [FromBody] CreateStudentDto dto,
        CancellationToken cancellationToken = default)
    {
        // Existing code...
    }

    // Rest of the code...
}
```

**❓ Giải thích**:
- **[Authorize]** ở class level: Tất cả endpoints cần authentication
- **[Authorize(Roles = "...")]**: Override cho specific endpoint, yêu cầu roles cụ thể

---

### ✅ **CHECKPOINT 4: WebApi Layer Complete**

**Kiểm tra:**
- [ ] Đã configure JWT authentication trong `DependencyInjection.cs`
- [ ] Đã update `Program.cs` với authentication middleware
- [ ] Đã tạo `AuthController` với register/login endpoints
- [ ] (Optional) Đã protect existing controllers

**Build solution:**
```bash
dotnet build
```

---

## 🧪 PHASE 5: TESTING (1-2 giờ)

### **STEP 5.1: Run Application**

```bash
cd D:\My-Project\StudentManagement-NetCore-CleanArchitecture-DDD
dotnet run --project src/StudentManagement.WebApi
```

Application sẽ chạy tại: `http://localhost:5282`

---

### **STEP 5.2: Test với Swagger UI**

1. Mở browser: `http://localhost:5282/swagger`
2. Bạn sẽ thấy button **"Authorize"** ở góc trên bên phải

#### **Test Register**

1. Expand `POST /api/Auth/register`
2. Click "Try it out"
3. Nhập data:
```json
{
  "username": "john.doe",
  "email": "john.doe@email.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "role": "Student"
}
```
4. Click "Execute"
5. Kiểm tra response: `200 OK` với user data

#### **Test Login**

1. Expand `POST /api/Auth/login`
2. Click "Try it out"
3. Nhập data:
```json
{
  "username": "john.doe",
  "password": "Password123!"
}
```
4. Click "Execute"
5. Response sẽ có `accessToken` và `refreshToken`
6. **Copy** `accessToken` value

#### **Test Protected Endpoint**

1. Click button **"Authorize"** (góc trên)
2. Nhập: `Bearer <paste_your_token_here>`
   - Ví dụ: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Click "Authorize", rồi "Close"
4. Expand `GET /api/Auth/me`
5. Click "Try it out", rồi "Execute"
6. Response sẽ trả về thông tin user từ token

#### **Test Role-based Authorization**

1. Try `GET /api/Auth/student-test` → Should work (nếu role là Student)
2. Try `GET /api/Auth/admin-test` → Should return 403 Forbidden (nếu không phải Admin)

---

### **STEP 5.3: Test với Postman/curl**

#### Register:
```bash
curl -X POST "http://localhost:5282/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "jane.smith",
    "email": "jane.smith@email.com",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!",
    "firstName": "Jane",
    "lastName": "Smith",
    "role": "Teacher"
  }'
```

#### Login:
```bash
curl -X POST "http://localhost:5282/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "jane.smith",
    "password": "SecurePass123!"
  }'
```

#### Get Current User (với token):
```bash
curl -X GET "http://localhost:5282/api/auth/me" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 📋 IMPLEMENTATION CHECKLIST

### Domain Layer ✅
- [ ] UserRole enum
- [ ] Username value object
- [ ] PasswordHash value object
- [ ] RefreshToken entity
- [ ] User entity (aggregate root)
- [ ] IUserRepository interface
- [ ] Update IUnitOfWork

### Application Layer ✅
- [ ] Authentication DTOs
- [ ] IPasswordHasher interface
- [ ] IJwtTokenService interface
- [ ] RegisterCommand + Validator + Handler
- [ ] LoginCommand + Validator + Handler
- [ ] UserMappingProfile

### Infrastructure Layer ✅
- [ ] PasswordHasher service
- [ ] JwtTokenService
- [ ] UserConfiguration
- [ ] RefreshTokenConfiguration
- [ ] Update DbContext
- [ ] UserRepository
- [ ] Update UnitOfWork
- [ ] Update DependencyInjection
- [ ] Create and apply migration

### WebApi Layer ✅
- [ ] Configure JWT authentication (DependencyInjection.cs)
- [ ] Update Program.cs (middleware)
- [ ] AuthController
- [ ] Test all endpoints

---

## 🚨 Common Issues & Solutions

### Issue 1: Build Error - "Cannot find type or namespace"
**Solution**: Đảm bảo đã add using statements đầy đủ:
```csharp
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.ValueObjects;
using StudentManagement.Domain.Repositories;
// etc...
```

### Issue 2: Migration Error - "No DbContext found"
**Solution**: Đảm bảo đang ở root directory và dùng đúng parameters:
```bash
dotnet ef migrations add AddUserAuthentication -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

### Issue 3: 401 Unauthorized khi call protected endpoint
**Solution**:
- Kiểm tra token có được thêm vào header không
- Format: `Authorization: Bearer <token>`
- Token có expired chưa (check thời gian)

### Issue 4: JWT Secret key too short
**Solution**: Secret key phải ít nhất 256 bits (32 characters). Update trong `appsettings.json`:
```json
"Secret": "Your-Secret-Key-Must-Be-At-Least-32-Characters-Long!"
```

### Issue 5: "Sequence contains no elements" khi login
**Solution**: Username hoặc password không đúng. Debug bằng cách:
1. Check user có tồn tại trong DB không
2. Check password đã hash đúng không
3. Check validation trong Verify

---

## 🎓 Học Thêm

### Concepts Nâng Cao
- **Refresh Token Rotation**: Implement refresh token endpoint
- **Email Confirmation**: Send email khi register
- **Password Reset**: Forgot password flow
- **Two-Factor Authentication (2FA)**: OTP via email/SMS
- **OAuth/Social Login**: Login với Google, Facebook
- **Rate Limiting**: Giới hạn số requests để chống brute force

### Best Practices
- **Không log passwords**: Không bao giờ log password (dù đã hash)
- **HTTPS only**: Production phải dùng HTTPS
- **Secure secret key**: Lưu JWT secret trong environment variables, không commit vào Git
- **Token expiry**: Access token ngắn (5-15 phút), refresh token dài (7-30 ngày)
- **Revoke tokens**: Implement token blacklist khi cần

---

## 🎯 Next Steps

Sau khi hoàn thành guide này, bạn có thể:

1. **Implement RefreshToken endpoint**
   - Tạo `RefreshTokenCommand`
   - Validate refresh token
   - Generate new access token

2. **Implement ChangePassword endpoint**
   - Verify old password
   - Hash new password
   - Update user

3. **Add more authorization policies**
   - Resource-based authorization
   - Claims-based authorization

4. **Write unit tests**
   - Test validators
   - Test handlers
   - Test services

5. **Write integration tests**
   - Test entire auth flow
   - Test JWT generation/validation

---

## 📞 Hỗ Trợ

Nếu gặp vấn đề trong quá trình implement:

1. **Check logs**: Xem console output để debug
2. **Debug breakpoints**: Đặt breakpoints trong handlers
3. **Check database**: Dùng DB browser để xem data
4. **Google error messages**: Search exact error message
5. **Ask for help**: Đừng ngại hỏi senior developers!

---

**Good luck với implementation! 🚀**

**Remember**: Coding là kỹ năng practice makes perfect. Đừng nản nếu gặp lỗi, đó là part of learning process!
