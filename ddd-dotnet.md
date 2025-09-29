# DDD & Clean Architecture Implementation Plan
## Student Management System with SQLite & Authentication

### **Solution Architecture Overview**

The solution follows Clean Architecture principles with Domain-Driven Design (DDD) tactical patterns:

```
StudentManagement/
├── src/
│   ├── StudentManagement.Domain/           # Core business logic (innermost layer)
│   │   ├── Entities/                      # Student, Course, User, Role
│   │   ├── ValueObjects/                  # StudentId, Email, etc.
│   │   ├── Events/                        # Domain events
│   │   ├── Services/                      # Domain services
│   │   └── Repositories/                  # Repository interfaces
│   ├── StudentManagement.Application/      # Use cases and application logic
│   │   ├── Commands/                      # CQRS commands
│   │   ├── Queries/                       # CQRS queries
│   │   ├── DTOs/                         # Data transfer objects
│   │   ├── Services/                     # Application services
│   │   ├── Auth/                         # Auth interfaces & models
│   │   └── Common/                       # Common application logic
│   ├── StudentManagement.Infrastructure/   # External concerns
│   │   ├── Data/                         # EF Core, SQLite setup
│   │   ├── Auth/                         # JWT, Identity implementation
│   │   ├── Repositories/                 # Repository implementations
│   │   ├── Services/                     # External service implementations
│   │   └── Migrations/                   # Database migrations
│   └── StudentManagement.WebApi/          # Presentation layer
│       ├── Controllers/                  # API controllers
│       ├── Middleware/                   # Auth, error handling
│       ├── Models/                       # Request/response models
│       └── Configuration/                # DI, startup config
├── tests/
│   ├── StudentManagement.Domain.Tests/
│   ├── StudentManagement.Application.Tests/
│   ├── StudentManagement.Infrastructure.Tests/
│   └── StudentManagement.WebApi.Tests/
└── docs/
```

### **Layer Responsibilities**

1. **Domain Layer** - Pure business logic, entities, value objects, domain services
2. **Application Layer** - Use cases, commands/queries, interfaces for infrastructure
3. **Infrastructure Layer** - Data persistence, external APIs, cross-cutting concerns
4. **WebApi Layer** - REST API endpoints, request/response models, dependency injection

### **Authentication & Authorization Strategy**

**Authentication Approach**: JWT Bearer tokens with ASP.NET Core Identity
**Database**: SQLite with Entity Framework Core
**Authorization**: Role-based with custom policies

**User Roles**:
- **Admin**: Full system access
- **Teacher**: Manage courses and grades
- **Student**: View own academic records
- **Staff**: Administrative functions

**Auth Flow**:
1. User registration/login
2. JWT token generation with claims
3. Token validation on protected endpoints
4. Role/policy-based authorization

**Security Features**:
- Password hashing with Identity
- JWT token expiration and refresh
- Rate limiting on auth endpoints
- CORS configuration

### **NuGet Packages by Layer**

**Domain Layer**: 
- None (pure .NET)

**Application Layer**:
- MediatR
- FluentValidation
- Microsoft.Extensions.DependencyInjection.Abstractions
- AutoMapper

**Infrastructure Layer**:
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- System.IdentityModel.Tokens.Jwt

**WebApi Layer**:
- Microsoft.AspNetCore.OpenApi
- Swashbuckle.AspNetCore
- MediatR (direct reference for v13+ compatibility)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Serilog.AspNetCore
- AutoMapper.Extensions.Microsoft.DependencyInjection

### **Implementation Phases**

## **Phase 1: Solution Restructuring & Database Setup**
1. Create 4 class library projects with proper dependencies
2. Set up SQLite with Entity Framework Core
3. Configure ASP.NET Core Identity
4. Create initial database context and migrations
5. Move existing code to WebApi project

**Tasks:**
- [x] Create StudentManagement.Domain project
- [x] Create StudentManagement.Application project
- [x] Create StudentManagement.Infrastructure project
- [x] Create StudentManagement.WebApi project
- [x] Set up project references and dependencies
- [x] Install required NuGet packages
- [x] Configure SQLite connection string

## **Phase 2: Domain Layer with Auth Entities**
1. Create User aggregate (extends IdentityUser)
2. Create Student, Course, Enrollment entities
3. Add Role entity (Admin, Teacher, Student, Staff)
4. Implement value objects (StudentId, Email, etc.)
5. Define domain events and repository interfaces

**Domain Entities:**
- **Student** (Aggregate Root)
- **Course** 
- **Enrollment**
- **Grade**
- **User** (extends IdentityUser)

**Value Objects:**
- StudentId, CourseCode, Email
- Address, PhoneNumber
- AcademicYear, Semester

**Domain Events:**
- StudentEnrolled, CourseCompleted
- GradeAssigned, StudentGraduated
- UserRegistered, UserRoleChanged

**Tasks:**
- [ ] Create base entity and aggregate root classes
- [ ] Implement Student aggregate with business rules
- [ ] Create Course and Enrollment entities
- [ ] Implement value objects
- [ ] Define repository interfaces
- [ ] Create domain events
- [ ] Add domain services

## **Phase 3: Application Layer with Auth Services**
1. Implement CQRS with MediatR
2. Create auth commands (Register, Login, RefreshToken)
3. Add student management commands/queries
4. Implement JWT token service
5. Add FluentValidation for all operations

**Use Cases:**
- **Authentication**: Register, Login, RefreshToken, Logout
- **Student Management**: CreateStudent, UpdateStudent, GetStudentById
- **Course Management**: CreateCourse, AssignTeacher, GetCourseList
- **Enrollment**: EnrollStudent, WithdrawStudent, GetEnrollments
- **Grades**: AssignGrade, UpdateGrade, GetStudentGrades

**Tasks:**
- [ ] Set up MediatR configuration
- [ ] Create command/query base classes
- [ ] Implement authentication commands and queries
- [ ] Create student management use cases
- [ ] Add FluentValidation rules
- [ ] Define DTOs for data transfer
- [ ] Create application service interfaces

## **Phase 4: Infrastructure Layer Implementation**
1. Implement Entity Framework repositories
2. Configure SQLite database context
3. Set up Identity services and JWT authentication
4. Create database migrations
5. Implement logging and caching

**Tasks:**
- [ ] Configure StudentManagementDbContext with Identity
- [ ] Implement repository pattern
- [ ] Set up SQLite configuration
- [ ] Create JWT token service
- [ ] Configure Identity services
- [ ] Add database migrations
- [ ] Implement logging with Serilog
- [ ] Set up AutoMapper profiles

## **Phase 5: WebApi with Authentication**
1. Create AuthController (login, register, refresh)
2. Create StudentController with role-based auth
3. Configure JWT middleware and policies
4. Add Swagger with JWT support
5. Implement error handling and validation

**Controllers:**
- **AuthController**: Register, Login, RefreshToken, Logout
- **StudentsController**: CRUD operations with role-based auth
- **CoursesController**: Course management for teachers/admins
- **EnrollmentsController**: Enrollment management

**Tasks:**
- [ ] Create base API controller
- [ ] Implement AuthController with JWT
- [ ] Create student management controllers
- [ ] Configure authentication middleware
- [ ] Set up authorization policies
- [ ] Add Swagger JWT configuration
- [ ] Implement global error handling
- [ ] Add request/response models

## **Phase 6: Security & Testing**
1. Add rate limiting and CORS
2. Implement refresh token rotation
3. Create unit and integration tests
4. Add authentication integration tests
5. Security audit and documentation

**Tasks:**
- [ ] Configure rate limiting
- [ ] Set up CORS policies
- [ ] Implement refresh token rotation
- [ ] Create unit tests for domain logic
- [ ] Add application layer tests
- [ ] Create integration tests for API
- [ ] Add authentication flow tests
- [ ] Security configuration review
- [ ] Update documentation

### **Database Schema Overview**

**Core Tables:**
- **Users** (AspNetUsers + custom fields)
- **Students** (links to Users)
- **Courses**
- **Enrollments** (Student-Course relationships)
- **Grades**
- **Roles** (AspNetRoles)

**Identity Tables** (managed by ASP.NET Core Identity):
- AspNetUsers
- AspNetRoles
- AspNetUserRoles
- AspNetUserClaims
- AspNetRoleClaims
- AspNetUserLogins
- AspNetUserTokens

### **API Endpoints Structure**

```
/api/auth/
  POST /register
  POST /login
  POST /refresh-token
  POST /logout

/api/students/
  GET / (Admin/Teacher only)
  GET /{id} (Admin/Teacher or own record)
  POST / (Admin only)
  PUT /{id} (Admin or own record)
  DELETE /{id} (Admin only)

/api/courses/
  GET /
  GET /{id}
  POST / (Admin/Teacher only)
  PUT /{id} (Admin/Teacher only)
  DELETE /{id} (Admin only)

/api/enrollments/
  GET /student/{studentId} (Admin/Teacher or own records)
  POST / (Admin/Teacher only)
  DELETE /{id} (Admin/Teacher only)

/api/grades/
  GET /student/{studentId} (Admin/Teacher or own records)
  POST / (Teacher/Admin only)
  PUT /{id} (Teacher/Admin only)
```

### **Development Commands**

```bash
# Build solution
dotnet build

# Run migrations
dotnet ef migrations add InitialCreate -p StudentManagement.Infrastructure -s StudentManagement.WebApi
dotnet ef database update -p StudentManagement.Infrastructure -s StudentManagement.WebApi

# Run application
dotnet run --project StudentManagement.WebApi

# Run tests
dotnet test

# Watch for changes
dotnet watch --project StudentManagement.WebApi
```

### **Configuration Settings**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=studentmanagement.db"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret",
    "Issuer": "StudentManagement",
    "Audience": "StudentManagementUsers",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

### **Next Steps**

1. **Review and approve this plan**
2. **Start with Phase 1**: Create the solution structure
3. **Set up the database**: Configure SQLite and Identity
4. **Implement authentication**: JWT tokens and user management
5. **Build domain logic**: Student management business rules
6. **Create API endpoints**: RESTful services with proper authorization
7. **Add comprehensive testing**: Unit, integration, and security tests

This plan provides a solid foundation for a production-ready student management system following best practices in .NET development, security, and architecture.