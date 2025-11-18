# Migration Plan: Clean Architecture to Hexagonal Architecture

**Created**: 2025-11-17 16:25
**Status**: In Progress (Phase 03 Complete)
**Overall Progress**: 33% (3 of 9 phases)
**Estimated Duration**: 4-6 weeks
**Complexity**: Medium

## Overview

Migration from Clean Architecture to Hexagonal Architecture (Ports & Adapters) for Student Management System. This is NOT replacement but REFINEMENT - making implicit ports/adapters EXPLICIT and restructuring to emphasize inbound/outbound communication patterns.

## Executive Summary

Current system uses Clean Architecture with 4 layers (Domain, Application, Infrastructure, WebApi). Hexagonal Architecture provides explicit mechanism for implementing Clean's dependency inversion through ports and adapters.

**Key Insight**: Clean Architecture already follows Hexagonal principles implicitly. Migration makes boundaries EXPLICIT.

### What Changes
- Repository interfaces → Secondary ports (driven)
- Controllers → Primary adapters (driving)
- Application handlers → Application core (use cases)
- Infrastructure implementations → Secondary adapters (driven)
- Project structure → Ports/Adapters emphasis

### What Stays Same
- Domain entities, value objects (untouched)
- Business logic (untouched)
- CQRS with MediatR (fully compatible)
- DDD patterns (fully compatible)
- Vietnamese documentation (update after migration)

## Migration Approach

**Strategy**: Incremental refactoring (Strangler Fig pattern)
**Risk Level**: Low-Medium (Clean Architecture provides solid foundation)
**Rollback**: Each phase can be reverted independently

## Research Completed

Three comprehensive research reports generated:

1. **[Hexagonal Architecture Fundamentals](../research/20251117-hexagonal-architecture-fundamentals.md)** (150 lines)
   - Core principles, ports vs adapters
   - Comparison with Clean Architecture
   - Benefits, trade-offs, best practices
   - Authoritative sources (Alistair Cockburn)

2. **[Migration Strategy](../research/20251117-clean-to-hexagonal-migration-strategy.md)** (150 lines)
   - Step-by-step migration approach
   - Component mapping Clean → Hexagonal
   - CQRS/MediatR integration patterns
   - Risk mitigation, rollback strategies

3. **[.NET 8 Implementation Patterns](../research/20251117-hexagonal-dotnet-implementation.md)** (150 lines)
   - Project structure conventions
   - Port/adapter code examples
   - Dependency injection patterns
   - Testing strategies

## Phase Breakdown

| Phase | Description | Duration | Status | Progress |
|-------|-------------|----------|--------|----------|
| [Phase 01](phase-01-research-preparation.md) | Research & Preparation | 2-3 days | ✅ Complete | 100% |
| [Phase 02](phase-02-define-ports.md) | Define Port Interfaces | 3-5 days | ✅ Complete | 100% |
| [Phase 03](phase-03-restructure-projects.md) | Restructure Project Layout | 2-3 days | ✅ Complete | 100% |
| [Phase 04](phase-04-persistence-adapters.md) | Create Persistence Adapters | 5-7 days | ⏳ Pending | 0% |
| [Phase 05](phase-05-api-adapters.md) | Create API Adapters | 3-5 days | ⏳ Pending | 0% |
| [Phase 06](phase-06-migrate-domain-logic.md) | Migrate Domain Logic | 2-3 days | ⏳ Pending | 0% |
| [Phase 07](phase-07-dependency-injection.md) | Update Dependency Injection | 2-3 days | ⏳ Pending | 0% |
| [Phase 08](phase-08-testing-validation.md) | Testing & Validation | 5-7 days | ⏳ Pending | 0% |
| [Phase 09](phase-09-documentation.md) | Documentation Updates | 2-3 days | ⏳ Pending | 0% |

**Total Estimated Duration**: 26-39 days (4-6 weeks)

## Current Architecture Analysis

### Existing Clean Architecture Structure

```
/src
├── StudentManagement.Domain (✅ Core - no dependencies)
│   ├── Entities (Student, Course, Enrollment, Grade)
│   ├── ValueObjects (Email, GPA, CourseCode, IDs)
│   ├── Events (IDomainEvent, StudentEnrolledEvent, etc.)
│   └── Repositories (IRepository, IStudentRepository, etc.)
├── StudentManagement.Application (✅ Use cases)
│   ├── Commands (CreateStudent, UpdateCourse, etc.)
│   ├── Queries (GetStudents, GetCourseById, etc.)
│   ├── DTOs (StudentDto, CourseDto, CommonDto)
│   ├── Validators (FluentValidation)
│   ├── Mappings (AutoMapper profiles)
│   └── Behaviors (ValidationBehavior)
├── StudentManagement.Infrastructure (✅ Data access)
│   ├── Data (DbContext, Configurations)
│   ├── Repositories (Repository<T>, StudentRepository, etc.)
│   └── Migrations (EF Core)
└── StudentManagement.WebApi (✅ Presentation)
    ├── Controllers (StudentsController, CoursesController, etc.)
    ├── Middleware (GlobalExceptionMiddleware)
    └── Program.cs
```

### Component Mapping: Clean → Hexagonal

| Clean Architecture Component | Hexagonal Architecture | Type | Location |
|------------------------------|------------------------|------|----------|
| Domain/Entities | Domain Core | Core | Domain/ |
| Domain/ValueObjects | Domain Core | Core | Domain/ |
| Domain/Events | Domain Events | Core | Domain/ |
| Domain/Repositories (interfaces) | **Secondary Ports** | Port | Domain/Ports/ |
| Application/Commands | Use Cases | Core | Application/UseCases/ |
| Application/Queries | Use Cases | Core | Application/UseCases/ |
| Application/DTOs | DTOs | Core | Application/DTOs/ |
| (NEW) Application Services | **Primary Ports (impl)** | Adapter | Application/Ports/ |
| Infrastructure/Repositories | **Secondary Adapters** | Adapter | Adapters.Persistence/ |
| Infrastructure/DbContext | Database Adapter | Adapter | Adapters.Persistence/ |
| WebApi/Controllers | **Primary Adapters** | Adapter | Adapters.WebApi/ |
| WebApi/Middleware | Infrastructure | Adapter | Adapters.WebApi/ |

## Target Hexagonal Architecture Structure

```
/src
├── StudentManagement.Domain (Core)
│   ├── Entities
│   ├── ValueObjects
│   ├── DomainEvents
│   └── Ports (NEW - Secondary port interfaces)
│       ├── IPersistence
│       │   ├── IStudentPersistencePort.cs
│       │   ├── ICoursePersistencePort.cs
│       │   └── IEnrollmentPersistencePort.cs
│       └── IExternal (for future external services)
│
├── StudentManagement.Application (Core + Primary Ports)
│   ├── Ports (NEW - Primary port interfaces)
│   │   ├── IStudentManagementPort.cs
│   │   ├── ICourseManagementPort.cs
│   │   └── IEnrollmentManagementPort.cs
│   ├── UseCases (renamed from Commands/Queries)
│   │   ├── Students/
│   │   ├── Courses/
│   │   └── Enrollments/
│   ├── DTOs
│   ├── Validators
│   └── Mappings
│
├── StudentManagement.Adapters.Persistence (renamed from Infrastructure)
│   ├── Database/
│   │   ├── DbContext
│   │   ├── Configurations/
│   │   └── Migrations/
│   └── Repositories/ (implement persistence ports)
│       ├── EfCoreStudentAdapter.cs
│       ├── EfCoreCourseAdapter.cs
│       └── EfCoreEnrollmentAdapter.cs
│
└── StudentManagement.Adapters.WebApi (renamed from WebApi)
    ├── Controllers/ (primary adapters)
    ├── ApplicationServices/ (NEW - implement primary ports)
    │   ├── StudentApplicationService.cs
    │   ├── CourseApplicationService.cs
    │   └── EnrollmentApplicationService.cs
    └── Middleware/
```

## Key Migration Decisions

### Decision 1: Project Naming Convention
**Options**:
- A: Keep existing names, add Ports folders
- B: Rename to Adapters.* pattern (RECOMMENDED)
- C: Hybrid approach

**Recommendation**: Option B - Rename Infrastructure → Adapters.Persistence, WebApi → Adapters.WebApi
**Rationale**: Makes Hexagonal Architecture explicit, clearer intent

### Decision 2: Port Granularity
**Options**:
- A: One port per entity (fine-grained)
- B: One port per aggregate (coarse-grained)
- C: Functional grouping (RECOMMENDED)

**Recommendation**: Option C - Functional grouping (IStudentManagementPort, ICoursePersistencePort)
**Rationale**: Balance between flexibility and simplicity

### Decision 3: CQRS Integration
**Decision**: Keep MediatR handlers, wrap in Application Services
**Rationale**: MediatR fully compatible, provides clean separation

### Decision 4: Backward Compatibility
**Decision**: Maintain existing API contracts during migration
**Rationale**: Zero downtime, gradual rollout

## Critical Dependencies

- .NET 8.0 SDK
- EF Core 8.0
- MediatR 13.0.0
- AutoMapper 12.0.1
- FluentValidation 12.0.0
- SQLite (database)

**No new dependencies required** - migration uses existing stack

## Success Criteria

1. ✅ All existing tests pass
2. ✅ No breaking changes to API contracts
3. ✅ Explicit ports/adapters visible in project structure
4. ✅ CQRS with MediatR continues working
5. ✅ DDD patterns intact
6. ✅ Performance maintained or improved
7. ✅ Documentation updated (English + Vietnamese)
8. ✅ Team understands new structure

## Risk Assessment

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Breaking API changes | Low | High | Maintain existing contracts, versioned endpoints |
| Performance degradation | Low | Medium | Benchmark critical paths, optimize adapters |
| Team confusion | Medium | Medium | Training, documentation, pair programming |
| Incomplete migration | Medium | High | Clear phases, code reviews, automated linting |
| Database migration issues | Low | High | Reversible migrations, backup strategy |

## Rollback Strategy

Each phase is independently reversible:
- Phase 1-2: No code changes, only research/design
- Phase 3: Project rename reversible via Git
- Phase 4-7: Feature flags control new implementations
- Phase 8-9: Documentation only

**Critical Rollback Point**: End of Phase 7 (before production deployment)

## Skills Assessment

### Required Skills
- ✅ .NET 8 / C# (team has)
- ✅ Clean Architecture (current implementation)
- ✅ CQRS with MediatR (current implementation)
- ✅ DDD patterns (current implementation)
- ⚠️ Hexagonal Architecture principles (needs training)
- ⚠️ Ports & Adapters pattern (needs training)

### Recommended Training
1. Team workshop on Hexagonal Architecture (4 hours)
2. Code walkthrough of research documents (2 hours)
3. Pair programming sessions during Phase 2-3 (ongoing)

## Timeline Estimate

### Conservative Estimate (6 weeks)
- Week 1-2: Research, design, ports definition
- Week 3: Project restructure, persistence adapters
- Week 4: API adapters, domain migration
- Week 5: DI updates, testing
- Week 6: Documentation, final validation

### Aggressive Estimate (4 weeks)
- Week 1: Research, design, ports
- Week 2: Restructure, adapters
- Week 3: Migration, testing
- Week 4: Documentation

**Recommended**: Conservative estimate with buffer

## Next Steps

1. **Review this plan** with team lead/architect
2. **Approve migration** or request changes
3. **Schedule kickoff meeting** (1 hour)
4. **Assign phase owners** (who implements each phase)
5. **Begin Phase 01** (Research & Preparation)

## Links to Detailed Phase Plans

- [Phase 01: Research & Preparation](phase-01-research-preparation.md)
- [Phase 02: Define Port Interfaces](phase-02-define-ports.md)
- [Phase 03: Restructure Project Layout](phase-03-restructure-projects.md)
- [Phase 04: Create Persistence Adapters](phase-04-persistence-adapters.md)
- [Phase 05: Create API Adapters](phase-05-api-adapters.md)
- [Phase 06: Migrate Domain Logic](phase-06-migrate-domain-logic.md)
- [Phase 07: Update Dependency Injection](phase-07-dependency-injection.md)
- [Phase 08: Testing & Validation](phase-08-testing-validation.md)
- [Phase 09: Documentation Updates](phase-09-documentation.md)

## Unresolved Questions

1. **Database Schema Changes**: Migration requires schema changes? (Likely NO)
2. **API Versioning**: Introduce v2 endpoints or keep v1? (Recommend keep v1)
3. **External Service Integration**: Future email/storage adapters location? (Adapters.External)
4. **Performance Benchmarks**: Establish baseline before migration? (Yes, recommended)
5. **Vietnamese Docs Update**: During or after migration? (Recommend after Phase 09)

## References

- [Hexagonal Architecture Fundamentals Research](../research/20251117-hexagonal-architecture-fundamentals.md)
- [Migration Strategy Research](../research/20251117-clean-to-hexagonal-migration-strategy.md)
- [.NET 8 Implementation Research](../research/20251117-hexagonal-dotnet-implementation.md)
- [Current Codebase Summary](../../docs/codebase-summary.md)
- [Current Code Standards](../../docs/code-standards.md)
- [Current System Architecture](../../docs/system-architecture.md)

---

**Status Legend**: ⏳ Pending | 🔄 In Progress | ✅ Complete | ⚠️ Blocked | ❌ Cancelled
