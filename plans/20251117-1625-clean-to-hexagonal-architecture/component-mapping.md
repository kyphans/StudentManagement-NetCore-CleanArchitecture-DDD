# Component Mapping: Clean Architecture → Hexagonal Architecture

**Date**: 2025-11-17
**Phase**: 01 - Research & Preparation
**Status**: ✅ Complete

## Code Inventory Summary

| Layer | Files Count | Description |
|-------|------------|-------------|
| Domain | 21 | Core entities, value objects, events, repository interfaces |
| Application | 47 | Commands, queries, handlers, DTOs, validators, mappings |
| Infrastructure | 14 | DbContext, configurations, repository implementations |
| WebApi | 8 | Controllers, middleware, DI, program entry |
| **Total** | **90** | Complete codebase inventory |

## Domain Layer Inventory (21 files)

### Entities (5 files)
- `BaseEntity.cs` → **Domain Core** (unchanged)
- `Student.cs` → **Domain Core** (unchanged)
- `Course.cs` → **Domain Core** (unchanged)
- `Enrollment.cs` → **Domain Core** (unchanged)
- `Grade.cs` → **Domain Core** (unchanged)

### Value Objects (7 files)
- `Email.cs` → **Domain Core** (unchanged)
- `GPA.cs` → **Domain Core** (unchanged)
- `CourseCode.cs` → **Domain Core** (unchanged)
- `StudentId.cs` → **Domain Core** (unchanged)
- `CourseId.cs` → **Domain Core** (unchanged)
- `EnrollmentId.cs` → **Domain Core** (unchanged)
- `GradeId.cs` → **Domain Core** (unchanged)

### Domain Events (4 files)
- `IDomainEvent.cs` → **Domain Events** (unchanged)
- `StudentEnrolledEvent.cs` → **Domain Events** (unchanged)
- `GradeAssignedEvent.cs` → **Domain Events** (unchanged)
- `CourseCompletedEvent.cs` → **Domain Events** (unchanged)

### Repository Interfaces (5 files) → **BECOMES SECONDARY PORTS**
- `IRepository.cs` → **`Domain/Ports/IPersistence/IPersistencePort.cs`**
- `IStudentRepository.cs` → **`Domain/Ports/IPersistence/IStudentPersistencePort.cs`**
- `ICourseRepository.cs` → **`Domain/Ports/IPersistence/ICoursePersistencePort.cs`**
- `IEnrollmentRepository.cs` → **`Domain/Ports/IPersistence/IEnrollmentPersistencePort.cs`**
- `IUnitOfWork.cs` → **`Domain/Ports/IPersistence/IUnitOfWorkPort.cs`**

## Application Layer Inventory (47 files)

### Commands (16 files) → **Use Cases (Commands)**
**Students** (6 files):
- `CreateStudentCommand.cs` → **`Application/UseCases/Commands/Students/CreateStudentCommand.cs`**
- `CreateStudentCommandHandler.cs` → **`Application/UseCases/Commands/Students/CreateStudentCommandHandler.cs`**
- `UpdateStudentCommand.cs` → **`Application/UseCases/Commands/Students/UpdateStudentCommand.cs`**
- `UpdateStudentCommandHandler.cs` → **`Application/UseCases/Commands/Students/UpdateStudentCommandHandler.cs`**
- `DeleteStudentCommand.cs` → **`Application/UseCases/Commands/Students/DeleteStudentCommand.cs`**
- `DeleteStudentCommandHandler.cs` → **`Application/UseCases/Commands/Students/DeleteStudentCommandHandler.cs`**

**Courses** (6 files):
- `CreateCourseCommand.cs` → **`Application/UseCases/Commands/Courses/CreateCourseCommand.cs`**
- `CreateCourseCommandHandler.cs` → **`Application/UseCases/Commands/Courses/CreateCourseCommandHandler.cs`**
- `UpdateCourseCommand.cs` → **`Application/UseCases/Commands/Courses/UpdateCourseCommand.cs`**
- `UpdateCourseCommandHandler.cs` → **`Application/UseCases/Commands/Courses/UpdateCourseCommandHandler.cs`**
- `DeleteCourseCommand.cs` → **`Application/UseCases/Commands/Courses/DeleteCourseCommand.cs`**
- `DeleteCourseCommandHandler.cs` → **`Application/UseCases/Commands/Courses/DeleteCourseCommandHandler.cs`**

**Enrollments** (4 files):
- `CreateEnrollmentCommand.cs` → **`Application/UseCases/Commands/Enrollments/CreateEnrollmentCommand.cs`**
- `CreateEnrollmentCommandHandler.cs` → **`Application/UseCases/Commands/Enrollments/CreateEnrollmentCommandHandler.cs`**
- `AssignGradeCommand.cs` → **`Application/UseCases/Commands/Enrollments/AssignGradeCommand.cs`**
- `AssignGradeCommandHandler.cs` → **`Application/UseCases/Commands/Enrollments/AssignGradeCommandHandler.cs`**

### Queries (12 files) → **Use Cases (Queries)**
**Students** (4 files):
- `GetStudentByIdQuery.cs` → **`Application/UseCases/Queries/Students/GetStudentByIdQuery.cs`**
- `GetStudentByIdQueryHandler.cs` → **`Application/UseCases/Queries/Students/GetStudentByIdQueryHandler.cs`**
- `GetStudentsQuery.cs` → **`Application/UseCases/Queries/Students/GetStudentsQuery.cs`**
- `GetStudentsQueryHandler.cs` → **`Application/UseCases/Queries/Students/GetStudentsQueryHandler.cs`**

**Courses** (4 files):
- `GetCourseByIdQuery.cs` → **`Application/UseCases/Queries/Courses/GetCourseByIdQuery.cs`**
- `GetCourseByIdQueryHandler.cs` → **`Application/UseCases/Queries/Courses/GetCourseByIdQueryHandler.cs`**
- `GetCoursesQuery.cs` → **`Application/UseCases/Queries/Courses/GetCoursesQuery.cs`**
- `GetCoursesQueryHandler.cs` → **`Application/UseCases/Queries/Courses/GetCoursesQueryHandler.cs`**

**Enrollments** (4 files):
- `GetEnrollmentByIdQuery.cs` → **`Application/UseCases/Queries/Enrollments/GetEnrollmentByIdQuery.cs`**
- `GetEnrollmentByIdQueryHandler.cs` → **`Application/UseCases/Queries/Enrollments/GetEnrollmentByIdQueryHandler.cs`**
- `GetEnrollmentsQuery.cs` → **`Application/UseCases/Queries/Enrollments/GetEnrollmentsQuery.cs`**
- `GetEnrollmentsQueryHandler.cs` → **`Application/UseCases/Queries/Enrollments/GetEnrollmentsQueryHandler.cs`**

### DTOs (5 files) → **Requests/Responses**
- `CommonDtos.cs` → **`Application/DTOs/Common/`** (ApiResponse, PagedResult, etc.)
- `StudentDtos.cs` → **`Application/DTOs/Requests/StudentRequest.cs` + `Responses/StudentResponse.cs`**
- `CourseDtos.cs` → **`Application/DTOs/Requests/CourseRequest.cs` + `Responses/CourseResponse.cs`**
- `EnrollmentDtos.cs` → **`Application/DTOs/Requests/EnrollmentRequest.cs` + `Responses/EnrollmentResponse.cs`**
- `GradeDtos.cs` → **`Application/DTOs/Responses/GradeResponse.cs`**

### Validators (8 files) → **Validation** (unchanged location)
- `CreateStudentCommandValidator.cs` → **`Application/Validators/Students/`** (unchanged)
- `UpdateStudentCommandValidator.cs` → **`Application/Validators/Students/`** (unchanged)
- `DeleteStudentCommandValidator.cs` → **`Application/Validators/Students/`** (unchanged)
- `CreateCourseCommandValidator.cs` → **`Application/Validators/Courses/`** (unchanged)
- `UpdateCourseCommandValidator.cs` → **`Application/Validators/Courses/`** (unchanged)
- `DeleteCourseCommandValidator.cs` → **`Application/Validators/Courses/`** (unchanged)
- `CreateEnrollmentCommandValidator.cs` → **`Application/Validators/Enrollments/`** (unchanged)
- `AssignGradeCommandValidator.cs` → **`Application/Validators/Enrollments/`** (unchanged)

### Mappings (4 files) → **Unchanged**
- `StudentMappingProfile.cs` → **`Application/Mappings/`** (unchanged)
- `CourseMappingProfile.cs` → **`Application/Mappings/`** (unchanged)
- `EnrollmentMappingProfile.cs` → **`Application/Mappings/`** (unchanged)
- `GradeMappingProfile.cs` → **`Application/Mappings/`** (unchanged)

### Behaviors (1 file) → **Unchanged**
- `ValidationBehavior.cs` → **`Application/Common/Behaviors/`** (unchanged)

### Dependency Injection (1 file) → **Unchanged**
- `DependencyInjection.cs` → **`Application/DependencyInjection.cs`** (unchanged)

## Infrastructure Layer Inventory (14 files)

### DbContext (1 file) → **BECOMES PERSISTENCE ADAPTER**
- `StudentManagementDbContext.cs` → **`Adapters.Persistence/Database/StudentManagementDbContext.cs`**

### Entity Configurations (4 files) → **PERSISTENCE ADAPTER**
- `StudentConfiguration.cs` → **`Adapters.Persistence/Database/Configurations/StudentConfiguration.cs`**
- `CourseConfiguration.cs` → **`Adapters.Persistence/Database/Configurations/CourseConfiguration.cs`**
- `EnrollmentConfiguration.cs` → **`Adapters.Persistence/Database/Configurations/EnrollmentConfiguration.cs`**
- `GradeConfiguration.cs` → **`Adapters.Persistence/Database/Configurations/GradeConfiguration.cs`**

### Repository Implementations (5 files) → **SECONDARY ADAPTERS**
- `Repository.cs` → **`Adapters.Persistence/Repositories/PersistenceAdapter.cs`**
- `StudentRepository.cs` → **`Adapters.Persistence/Repositories/StudentPersistenceAdapter.cs`**
- `CourseRepository.cs` → **`Adapters.Persistence/Repositories/CoursePersistenceAdapter.cs`**
- `EnrollmentRepository.cs` → **`Adapters.Persistence/Repositories/EnrollmentPersistenceAdapter.cs`**
- `UnitOfWork.cs` → **`Adapters.Persistence/Repositories/UnitOfWorkAdapter.cs`**

### Migrations (3 files) → **PERSISTENCE ADAPTER**
- Migration files → **`Adapters.Persistence/Migrations/`** (unchanged location)

### Dependency Injection (1 file) → **ADAPTER DI**
- `DependencyInjection.cs` → **`Adapters.Persistence/DependencyInjection.cs`**

## WebApi Layer Inventory (8 files)

### Controllers (5 files) → **PRIMARY ADAPTERS**
- `BaseApiController.cs` → **`Adapters.WebApi/Controllers/BaseApiController.cs`**
- `StudentsController.cs` → **`Adapters.WebApi/Controllers/StudentsController.cs`**
- `CoursesController.cs` → **`Adapters.WebApi/Controllers/CoursesController.cs`**
- `EnrollmentsController.cs` → **`Adapters.WebApi/Controllers/EnrollmentsController.cs`**
- `HealthController.cs` → **`Adapters.WebApi/Controllers/HealthController.cs`**

### Middleware (1 file) → **PRIMARY ADAPTER**
- `GlobalExceptionMiddleware.cs` → **`Adapters.WebApi/Middleware/GlobalExceptionMiddleware.cs`**

### Dependency Injection (1 file) → **ADAPTER DI**
- `DependencyInjection.cs` → **`Adapters.WebApi/DependencyInjection.cs`**

### Entry Point (1 file) → **PRIMARY ADAPTER**
- `Program.cs` → **`Adapters.WebApi/Program.cs`**

## New Components to Create (Phase 02)

### Primary Ports (Application Layer) - NEW
- **`Application/Ports/IStudentManagementPort.cs`** (NEW)
- **`Application/Ports/ICourseManagementPort.cs`** (NEW)
- **`Application/Ports/IEnrollmentManagementPort.cs`** (NEW)

### Secondary Ports (Domain Layer) - NEW
- **`Domain/Ports/IPersistence/IPersistencePort.cs`** (base interface)
- **`Domain/Ports/IPersistence/IStudentPersistencePort.cs`** (replaces IStudentRepository)
- **`Domain/Ports/IPersistence/ICoursePersistencePort.cs`** (replaces ICourseRepository)
- **`Domain/Ports/IPersistence/IEnrollmentPersistencePort.cs`** (replaces IEnrollmentRepository)
- **`Domain/Ports/IPersistence/IUnitOfWorkPort.cs`** (replaces IUnitOfWork)

### Application Services (NEW) - Implements Primary Ports
- **`Adapters.WebApi/ApplicationServices/StudentManagementService.cs`** (implements IStudentManagementPort)
- **`Adapters.WebApi/ApplicationServices/CourseManagementService.cs`** (implements ICourseManagementPort)
- **`Adapters.WebApi/ApplicationServices/EnrollmentManagementService.cs`** (implements IEnrollmentManagementPort)

## Summary: Clean → Hexagonal Mapping

| Clean Architecture Component | Count | → | Hexagonal Component | New Location |
|------------------------------|-------|---|---------------------|--------------|
| **Domain/Entities** | 5 | → | Domain Core | `Domain/Entities/` (unchanged) |
| **Domain/ValueObjects** | 7 | → | Domain Core | `Domain/ValueObjects/` (unchanged) |
| **Domain/Events** | 4 | → | Domain Events | `Domain/Events/` (unchanged) |
| **Domain/Repositories** (interfaces) | 5 | → | **Secondary Ports** | `Domain/Ports/IPersistence/` |
| **Application/Commands** | 8 | → | Use Cases (Commands) | `Application/UseCases/Commands/` |
| **Application/Queries** | 6 | → | Use Cases (Queries) | `Application/UseCases/Queries/` |
| **Application/Handlers** | 14 | → | Use Case Handlers | `Application/UseCases/` |
| **Application/DTOs** | 5 | → | Request/Response DTOs | `Application/DTOs/` |
| **Infrastructure/Repositories** (impl) | 5 | → | **Secondary Adapters** | `Adapters.Persistence/Repositories/` |
| **Infrastructure/DbContext** | 1 | → | Persistence Adapter | `Adapters.Persistence/Database/` |
| **WebApi/Controllers** | 5 | → | **Primary Adapters** | `Adapters.WebApi/Controllers/` |
| **(NEW) Primary Ports** | 0 | → | **Primary Ports** | `Application/Ports/` (NEW) |
| **(NEW) Application Services** | 0 | → | Port Implementations | `Adapters.WebApi/ApplicationServices/` (NEW) |

## Key Transformations

### 1. Repository Interfaces → Secondary Ports
```
Domain/Repositories/IStudentRepository.cs
  → Domain/Ports/IPersistence/IStudentPersistencePort.cs
```

### 2. Repository Implementations → Secondary Adapters
```
Infrastructure/Repositories/StudentRepository.cs
  → Adapters.Persistence/Repositories/StudentPersistenceAdapter.cs
```

### 3. Controllers → Primary Adapters
```
WebApi/Controllers/StudentsController.cs
  → Adapters.WebApi/Controllers/StudentsController.cs
```

### 4. NEW: Primary Ports (Application Layer)
```
(NEW) Application/Ports/IStudentManagementPort.cs
```

### 5. NEW: Application Services (Adapter Layer)
```
(NEW) Adapters.WebApi/ApplicationServices/StudentManagementService.cs
  implements IStudentManagementPort
```

## Migration Impact Analysis

### No Changes Required (61 files - 68%)
- All entities, value objects, domain events
- All validators, mappings, behaviors
- Migration files
- Most DTOs (may split Request/Response)

### Rename/Move Only (24 files - 27%)
- Commands, Queries, Handlers (move to UseCases/)
- Repository implementations (rename to *Adapter)
- DbContext and configurations (move to Adapters.Persistence)
- Controllers (move to Adapters.WebApi)

### New Files Required (10 files - 11%)
- 3 Primary Port interfaces
- 5 Secondary Port interfaces (replace repository interfaces)
- 3 Application Service implementations (NEW pattern)

### Total Migration Effort
- **Low Risk**: 68% of files unchanged
- **Medium Risk**: 27% of files rename/move only
- **New Development**: 11% new port interfaces and services

## Next Steps (Phase 02)
1. Create port directory structure
2. Define Secondary Ports (replace repository interfaces)
3. Define Primary Ports (new application interfaces)
4. Create Request/Response DTOs if needed
5. Verify compilation (no implementations yet)
