# Dependency Graph Analysis

**Date**: 2025-11-17
**Phase**: 01 - Research & Preparation
**Status**: ✅ Complete

## Layer Dependency Flow (Clean Architecture)

```
┌──────────────────────────────────────────────────────────────┐
│                         WebApi Layer                         │
│  ┌────────────┐  ┌────────────┐  ┌───────────────────────┐  │
│  │ Controllers│  │ Middleware │  │ DependencyInjection   │  │
│  └────────────┘  └────────────┘  └───────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
         │ depends on                            │ depends on
         ▼                                       ▼
┌──────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                       │
│  ┌────────────┐  ┌────────────┐  ┌───────────────────────┐  │
│  │ DbContext  │  │Repositories│  │ EF Configurations     │  │
│  └────────────┘  └────────────┘  └───────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
         │ depends on                            │ depends on
         ▼                                       ▼
┌──────────────────────────────────────────────────────────────┐
│                    Application Layer                          │
│  ┌────────────┐  ┌────────────┐  ┌───────────────────────┐  │
│  │  Commands  │  │  Queries   │  │  DTOs / Validators    │  │
│  │  Handlers  │  │  Handlers  │  │  Mappings / Behaviors │  │
│  └────────────┘  └────────────┘  └───────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
         │ depends on
         ▼
┌──────────────────────────────────────────────────────────────┐
│                        Domain Layer                           │
│  ┌────────────┐  ┌────────────┐  ┌───────────────────────┐  │
│  │  Entities  │  │Value Objs  │  │ Repository Interfaces │  │
│  │  Events    │  │            │  │                       │  │
│  └────────────┘  └────────────┘  └───────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

## Hexagonal Architecture Target State

```
┌────────────────────────────────────────────────────────────────────┐
│                        PRIMARY ADAPTERS                            │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │                    Adapters.WebApi                           │ │
│  │  ┌────────────┐  ┌────────────┐  ┌──────────────────────┐  │ │
│  │  │Controllers │  │ App Service│  │ Middleware / Program │  │ │
│  │  └────────────┘  └────────────┘  └──────────────────────┘  │ │
│  └──────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────────┘
                         │ implements                ▲
                         ▼                           │ calls
         ┌──────────────────────────┐    ┌──────────────────────┐
         │   PRIMARY PORTS          │    │   APPLICATION        │
         │  IStudentManagementPort  │◄───│   UseCases           │
         │  ICourseManagementPort   │    │   Commands/Queries   │
         │  IEnrollmentMgmtPort     │    │   Handlers           │
         └──────────────────────────┘    └──────────────────────┘
                                                     │ uses
                                                     ▼
         ┌──────────────────────────┐    ┌──────────────────────┐
         │   SECONDARY PORTS        │◄───│   DOMAIN             │
         │  IStudentPersistencePort │    │   Entities           │
         │  ICoursePersistencePort  │    │   Value Objects      │
         │  IEnrollmentPersistPort  │    │   Domain Events      │
         │  IUnitOfWorkPort         │    │                      │
         └──────────────────────────┘    └──────────────────────┘
                         ▲
                         │ implements
                         │
┌────────────────────────────────────────────────────────────────────┐
│                       SECONDARY ADAPTERS                           │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │                 Adapters.Persistence                         │ │
│  │  ┌────────────┐  ┌────────────┐  ┌──────────────────────┐  │ │
│  │  │ DbContext  │  │Persistence │  │ EF Configurations    │  │ │
│  │  │            │  │ Adapters   │  │ Migrations           │  │ │
│  │  └────────────┘  └────────────┘  └──────────────────────┘  │ │
│  └──────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────────┘
                         │
                         ▼
                  External Database
                    (SQLite)
```

## NuGet Package Dependencies

### Domain Layer (ZERO external dependencies ✅)
```
StudentManagement.Domain.csproj
  TargetFramework: net8.0
  ImplicitUsings: enable
  Nullable: enable

  PackageReferences: NONE
  ProjectReferences: NONE
```

### Application Layer
```
StudentManagement.Application.csproj
  TargetFramework: net8.0

  PackageReferences:
    - MediatR 13.0.0
    - AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
    - FluentValidation 12.0.0
    - FluentValidation.DependencyInjectionExtensions 12.0.0
    - Microsoft.Extensions.DependencyInjection.Abstractions 9.0.9

  ProjectReferences:
    - StudentManagement.Domain
```

### Infrastructure Layer
```
StudentManagement.Infrastructure.csproj
  TargetFramework: net8.0

  PackageReferences:
    - Microsoft.EntityFrameworkCore.Design 8.0.4
    - Microsoft.EntityFrameworkCore.Sqlite 8.0.4

  ProjectReferences:
    - StudentManagement.Domain
    - StudentManagement.Application
```

### WebApi Layer
```
StudentManagement.WebApi.csproj
  TargetFramework: net8.0
  SDK: Microsoft.NET.Sdk.Web

  PackageReferences:
    - MediatR 13.0.0
    - AutoMapper 12.0.1
    - AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
    - Microsoft.AspNetCore.OpenApi 8.0.4
    - Microsoft.EntityFrameworkCore.Design 8.0.4
    - Serilog.AspNetCore 9.0.0
    - Swashbuckle.AspNetCore 6.4.0
    - Swashbuckle.AspNetCore.Annotations 9.0.4

  ProjectReferences:
    - StudentManagement.Application
    - StudentManagement.Infrastructure
```

## Project Reference Graph

```
┌─────────────────────┐
│   Domain            │◄──────────────────────────────────┐
│  (0 dependencies)   │                                   │
└─────────────────────┘                                   │
         ▲                                                │
         │                                                │
         │ references                                     │
         │                                                │
┌─────────────────────┐                                   │
│   Application       │                                   │
│  (1 project ref)    │                                   │
└─────────────────────┘                                   │
         ▲                  ▲                             │
         │                  │                             │
         │ references       │ references                  │
         │                  │                             │
┌─────────────────────┐     │                             │
│   Infrastructure    │─────┘                             │
│  (2 project refs)   │───────────────────────────────────┘
└─────────────────────┘    references
         ▲
         │
         │ references (both)
         │
┌─────────────────────┐
│   WebApi            │
│  (2 project refs)   │
└─────────────────────┘
```

## Dependency Injection Flow

### Current State (Clean Architecture)

```
Program.cs (WebApi)
  │
  ├─► services.AddApplication()
  │    └─► Registers:
  │         - MediatR handlers
  │         - AutoMapper profiles
  │         - FluentValidation validators
  │         - ValidationBehavior pipeline
  │
  ├─► services.AddInfrastructure(config)
  │    └─► Registers:
  │         - DbContext (scoped)
  │         - Repository implementations
  │         - UnitOfWork
  │
  └─► services.AddWebApi()
       └─► Registers:
            - Controllers
            - Swagger
            - CORS
            - Response compression
            - Health checks
            - GlobalExceptionMiddleware
```

### Target State (Hexagonal Architecture)

```
Program.cs (Adapters.WebApi)
  │
  ├─► services.AddDomain()  [NEW - minimal, if any]
  │    └─► Registers domain services if needed
  │
  ├─► services.AddApplication()
  │    └─► Registers:
  │         - MediatR handlers (use cases)
  │         - AutoMapper profiles
  │         - FluentValidation validators
  │         - ValidationBehavior pipeline
  │
  ├─► services.AddPersistenceAdapter(config)  [RENAMED]
  │    └─► Registers:
  │         - DbContext (scoped)
  │         - Secondary adapters (implements persistence ports)
  │         - UnitOfWorkAdapter
  │
  └─► services.AddWebApiAdapter()  [RENAMED]
       └─► Registers:
            - Controllers
            - Application Services (implements primary ports) [NEW]
            - Swagger
            - CORS
            - Response compression
            - Health checks
            - GlobalExceptionMiddleware
```

## Key Coupling Points

### 1. Repository Interface Dependencies
**Current**:
- `IStudentRepository` → used by command/query handlers
- `ICourseRepository` → used by command/query handlers
- `IEnrollmentRepository` → used by command/query handlers
- `IUnitOfWork` → used by command handlers for transactions

**Target**:
- Port interfaces in Domain layer
- Adapters implement ports in Infrastructure layer
- Handlers depend on port abstractions (same pattern, renamed)

### 2. MediatR Dependencies
**Current**:
- Controllers → send commands/queries to MediatR
- Handlers → process requests
- ValidationBehavior → intercepts pipeline

**Target**: UNCHANGED
- Hexagonal is compatible with MediatR
- Handlers are use cases in application core
- Controllers still send commands via MediatR

### 3. AutoMapper Dependencies
**Current**:
- Handlers → map entities to DTOs
- Mapping profiles registered in Application layer

**Target**: UNCHANGED
- Mapping remains in Application layer
- DTOs separate requests/responses clearly

### 4. EF Core Dependencies
**Current**:
- DbContext in Infrastructure
- Entity configurations in Infrastructure
- Migrations in Infrastructure

**Target**:
- Move to Adapters.Persistence project
- Same functionality, different namespace
- Decouples from "Infrastructure" term

## External System Dependencies

### Current External Dependencies
1. **Database**: SQLite (file-based, local)
   - Connection: `studentmanagement.db`
   - Provider: EF Core SQLite

2. **Logging**: Serilog
   - Sinks: Console, File (if configured)

3. **API Documentation**: Swagger/OpenAPI
   - Endpoint: `/swagger`

4. **Health Checks**: ASP.NET Core Health Checks
   - Endpoint: `/health`

### Future External Dependencies (Potential)
1. **Authentication**: JWT tokens (configured but not implemented)
2. **Caching**: Redis or in-memory
3. **Message Bus**: RabbitMQ or Azure Service Bus
4. **External APIs**: Student information systems, payment gateways
5. **File Storage**: Azure Blob, AWS S3, or local file system
6. **Email Service**: SendGrid, SMTP

### Hexagonal Adapter Strategy for Future Dependencies

```
Future Adapter Examples:

Adapters.Messaging/
  ├── RabbitMqAdapter.cs (implements IMessageBusPort)
  └── AzureServiceBusAdapter.cs (implements IMessageBusPort)

Adapters.ExternalApi/
  ├── PaymentGatewayAdapter.cs (implements IPaymentPort)
  └── StudentInfoSystemAdapter.cs (implements IStudentDataPort)

Adapters.Storage/
  ├── AzureBlobStorageAdapter.cs (implements IFileStoragePort)
  └── LocalFileStorageAdapter.cs (implements IFileStoragePort)
```

## Dependency Rules Validation

### ✅ Current State Compliance
1. **Domain has ZERO dependencies**: ✅ Compliant
2. **Application depends on Domain only**: ✅ Compliant
3. **Infrastructure depends on Domain + Application**: ✅ Compliant
4. **WebApi depends on Application + Infrastructure**: ✅ Compliant

### ✅ No Circular Dependencies
- Verified: No circular references
- All dependencies flow inward toward Domain

### ✅ Dependency Inversion
- Interfaces in Domain layer
- Implementations in Infrastructure layer
- Controllers and handlers depend on abstractions

## Migration Impact on Dependencies

### No Changes to NuGet Packages
- All package references remain the same
- Package versions unchanged

### Project Reference Changes
**Before**:
```
Domain ← Application ← Infrastructure ← WebApi
```

**After**:
```
Domain ← Application ← Adapters.Persistence ← Adapters.WebApi
```

### New Port Dependencies (Phase 02)
**Secondary Ports** (Domain layer):
- `IStudentPersistencePort`
- `ICoursePersistencePort`
- `IEnrollmentPersistencePort`
- `IUnitOfWorkPort`

**Primary Ports** (Application layer):
- `IStudentManagementPort`
- `ICourseManagementPort`
- `IEnrollmentManagementPort`

## Risks and Mitigations

### Risk 1: Breaking Existing DI Registrations
**Probability**: Low
**Impact**: Medium
**Mitigation**:
- Keep DI structure identical
- Only rename extension methods
- Test DI resolution after changes

### Risk 2: Lost EF Core Tooling
**Probability**: Low
**Impact**: Low
**Mitigation**:
- Update migration commands to point to new project name
- Test `dotnet ef` commands early in migration

### Risk 3: Incomplete Port Coverage
**Probability**: Low
**Impact**: Medium
**Mitigation**:
- Comprehensive component mapping (completed ✅)
- Review all repository interfaces before creating ports

## Next Steps

1. ✅ **Completed**: Component inventory and mapping
2. ✅ **Completed**: Dependency graph analysis
3. **Next**: Create port interfaces (Phase 02)
4. **Next**: Restructure projects (Phase 03)
5. **Next**: Update DI registrations (Phase 07)

## Conclusion

The current Clean Architecture implementation is **highly compatible** with Hexagonal Architecture principles:

- ✅ Clean dependency flow (inward toward domain)
- ✅ No external dependencies in Domain layer
- ✅ Interface-based abstractions (repositories)
- ✅ Dependency inversion properly applied
- ✅ No circular dependencies

**Migration complexity**: LOW to MEDIUM
**Primary work**: Renaming and restructuring, not re-architecting
**Risk level**: LOW (68% of files unchanged)
