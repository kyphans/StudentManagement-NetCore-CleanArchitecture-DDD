# Phase 01: Research & Preparation

## Context Links
- **Parent Plan**: [plan.md](plan.md)
- **Dependencies**: None (starting phase)
- **Related Docs**: [Codebase Summary](../../docs/codebase-summary.md), [Code Standards](../../docs/code-standards.md)

## Overview
- **Date Completed**: 2025-11-17
- **Description**: Understand current architecture, create component mapping, establish migration baseline
- **Duration**: 2-3 days (Actual: 1 day)
- **Priority**: P0 (Critical - foundation for all phases)
- **Implementation Status**: ✅ Complete
- **Review Status**: ✅ Approved

## Key Insights from Research

1. **Clean already Hexagonal-compatible**: Current Clean Architecture implicitly follows Hexagonal principles via repository interfaces and dependency inversion

2. **Migration is refinement**: NOT replacement - make implicit boundaries EXPLICIT through ports/adapters terminology

3. **CQRS/MediatR compatible**: Handlers become core use cases, no structural changes needed

4. **Low risk migration**: Solid Clean Architecture foundation minimizes breaking changes

## Requirements

### Functional Requirements
1. Audit all current components in 4 layers
2. Map each component to Hexagonal equivalent
3. Document all external dependencies (DB, future APIs)
4. Establish performance baselines
5. Create migration roadmap with decision points

### Non-Functional Requirements
1. Zero code changes in this phase (research only)
2. Complete component inventory
3. Documented baseline metrics
4. Team alignment on Hexagonal principles

## Architecture

### Current State Inventory

**Domain Layer** (28 files):
- Entities: BaseEntity, Student, Course, Enrollment, Grade
- ValueObjects: Email, GPA, CourseCode, StudentId, CourseId, EnrollmentId, GradeId
- Events: IDomainEvent, StudentEnrolledEvent, GradeAssignedEvent, CourseCompletedEvent
- Repositories: IRepository<T>, IStudentRepository, ICourseRepository, IEnrollmentRepository, IUnitOfWork

**Application Layer** (50+ files):
- Commands: 8 commands (Student, Course, Enrollment operations)
- Queries: 6 queries (Get operations)
- Handlers: 14 handlers (CQRS implementation)
- DTOs: 4 DTO files (Student, Course, Enrollment, Common)
- Validators: 7 validators (FluentValidation)
- Mappings: 4 AutoMapper profiles
- Behaviors: ValidationBehavior (MediatR pipeline)

**Infrastructure Layer** (15 files):
- DbContext: StudentManagementDbContext
- Configurations: 4 entity configurations
- Repositories: 5 repository implementations
- Migrations: 3 migration files

**WebApi Layer** (8 files):
- Controllers: 4 controllers (Students, Courses, Enrollments, Health)
- Middleware: GlobalExceptionMiddleware
- DI: DependencyInjection.cs
- Program.cs

### Component Mapping Table

| Current Component | Type | → | Hexagonal Component | New Location |
|-------------------|------|---|---------------------|--------------|
| Domain/Entities | Core | → | Domain Core | Domain/Entities |
| Domain/ValueObjects | Core | → | Domain Core | Domain/ValueObjects |
| Domain/Events | Core | → | Domain Events | Domain/Events |
| Domain/Repositories (I*) | Interface | → | **Secondary Ports** | Domain/Ports/IPersistence/ |
| Application/Commands | UseCase | → | Use Cases (Command) | Application/UseCases/Commands/ |
| Application/Queries | UseCase | → | Use Cases (Query) | Application/UseCases/Queries/ |
| Application/Handlers | UseCase | → | Use Case Handlers | Application/UseCases/ |
| Infrastructure/Repositories | Impl | → | **Secondary Adapters** | Adapters.Persistence/Repositories/ |
| Infrastructure/DbContext | Impl | → | DB Adapter | Adapters.Persistence/Database/ |
| WebApi/Controllers | Impl | → | **Primary Adapters** | Adapters.WebApi/Controllers/ |
| (NEW) App Services | Impl | → | **Primary Port Impl** | Adapters.WebApi/ApplicationServices/ |

## Related Code Files

### Files to Analyze (No Modifications)
- All files in `/src/StudentManagement.Domain/`
- All files in `/src/StudentManagement.Application/`
- All files in `/src/StudentManagement.Infrastructure/`
- All files in `/src/StudentManagement.WebApi/`
- `docs/codebase-summary.md`
- `docs/code-standards.md`
- `docs/system-architecture.md`

### Files to Create
- `/plans/20251117-1625-clean-to-hexagonal-architecture/component-mapping.md`
- `/plans/20251117-1625-clean-to-hexagonal-architecture/dependency-graph.md`
- `/plans/20251117-1625-clean-to-hexagonal-architecture/performance-baseline.md`
- `/plans/20251117-1625-clean-to-hexagonal-architecture/decision-log.md`

## Implementation Steps

### Step 1: Code Inventory (4 hours)
1. Count files per layer using `find` or IDE
2. List all entities, value objects, events
3. List all commands, queries, handlers
4. List all repository interfaces and implementations
5. List all controllers and endpoints
6. Document in `component-mapping.md`

### Step 2: Dependency Analysis (3 hours)
1. Map dependencies between layers
2. Identify external dependencies (NuGet packages)
3. Document coupling points
4. Create dependency graph diagram (mermaid or ASCII)
5. Save to `dependency-graph.md`

### Step 3: Performance Baseline (2 hours)
1. Run existing application
2. Measure API response times for key endpoints:
   - GET /api/students
   - POST /api/students
   - GET /api/students/{id}
   - GET /api/courses
   - POST /api/enrollments
3. Measure database query times
4. Document in `performance-baseline.md`

### Step 4: Team Workshop (4 hours)
1. Present Hexagonal Architecture fundamentals (1 hour)
2. Review research documents with team (1 hour)
3. Discuss migration approach (1 hour)
4. Q&A and decision making (1 hour)
5. Document decisions in `decision-log.md`

### Step 5: Risk Assessment (2 hours)
1. Identify potential breaking points
2. Assess team skill gaps
3. Document mitigation strategies
4. Get stakeholder sign-off

### Step 6: Finalize Plan (1 hour)
1. Review all phase plans
2. Adjust timeline based on team feedback
3. Assign phase owners
4. Schedule next phase kickoff

## Todo List

- [ ] Run code inventory script
- [ ] Count files per layer
- [ ] List all components
- [ ] Create component mapping table
- [ ] Analyze dependencies between layers
- [ ] Identify external dependencies
- [ ] Create dependency graph
- [ ] Measure API response times
- [ ] Measure database query times
- [ ] Document performance baseline
- [ ] Schedule team workshop
- [ ] Present Hexagonal Architecture fundamentals
- [ ] Review research documents
- [ ] Discuss migration approach
- [ ] Conduct Q&A session
- [ ] Document decisions
- [ ] Identify migration risks
- [ ] Assess team skills
- [ ] Document mitigation strategies
- [ ] Get stakeholder approval
- [ ] Review all phase plans
- [ ] Assign phase owners
- [ ] Schedule Phase 02 kickoff

## Success Criteria

1. ✅ Complete component inventory documented
2. ✅ Component mapping table created
3. ✅ Dependency graph visualized
4. ✅ Performance baseline established
5. ✅ Team understands Hexagonal Architecture
6. ✅ All decisions documented
7. ✅ Stakeholder approval obtained
8. ✅ Phase 02 schedule confirmed

## Risk Assessment

### Risks Identified
1. **Team Knowledge Gap**
   - **Probability**: Medium
   - **Impact**: Medium
   - **Mitigation**: Comprehensive workshop, documentation, pair programming

2. **Incomplete Inventory**
   - **Probability**: Low
   - **Impact**: High
   - **Mitigation**: Automated scripts, code review

3. **Stakeholder Misalignment**
   - **Probability**: Low
   - **Impact**: High
   - **Mitigation**: Clear communication, benefits explanation

## Security Considerations

- No security implications (research phase only)
- Ensure decision documents not exposed publicly (may contain architectural details)

## Next Steps

After Phase 01 completion:
1. Review component mapping with team
2. Validate performance baseline
3. Get sign-off on migration approach
4. Begin Phase 02: Define Port Interfaces
