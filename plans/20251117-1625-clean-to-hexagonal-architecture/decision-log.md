# Migration Decision Log

**Date**: 2025-11-17
**Phase**: 01 - Research & Preparation
**Status**: ✅ Complete

## Decision Record Format

Each decision follows this format:
- **ID**: Unique identifier
- **Date**: When decision was made
- **Context**: Why this decision is needed
- **Decision**: What was decided
- **Rationale**: Why this approach
- **Alternatives Considered**: Other options evaluated
- **Consequences**: Impact of this decision
- **Status**: Proposed, Accepted, Rejected, Superseded

---

## D001: Adopt Hexagonal Architecture Alongside Clean Architecture

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Current system uses Clean Architecture. Need to improve testability, flexibility, and make architectural boundaries more explicit.

**Decision**: Migrate to Hexagonal Architecture (Ports and Adapters) while preserving Clean Architecture principles.

**Rationale**:
- Clean and Hexagonal are compatible, not competing
- Hexagonal makes implicit ports/adapters explicit
- Improves testability through port mocking
- Enables adapter swapping (e.g., SQLite → PostgreSQL, REST → gRPC)
- Industry best practice for domain-centric systems

**Alternatives Considered**:
1. Keep pure Clean Architecture → Less explicit about boundaries
2. Onion Architecture → Similar to Clean, less industry adoption
3. Vertical Slice Architecture → Too radical a change

**Consequences**:
- ✅ Better testability
- ✅ More explicit boundaries
- ✅ Easier to swap adapters
- ⚠️ More interfaces to maintain
- ⚠️ Learning curve for team

---

## D002: Rename Projects to Adapters.* Pattern

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Current projects named Infrastructure and WebApi don't reflect Hexagonal terminology.

**Decision**: Rename projects:
- `Infrastructure` → `Adapters.Persistence`
- `WebApi` → `Adapters.WebApi`
- Keep `Domain` and `Application` unchanged

**Rationale**:
- Makes Hexagonal Architecture explicit
- "Adapters" clearly indicates swappable implementations
- Persistence and WebApi are specific adapter types
- Domain and Application are core, not adapters

**Alternatives Considered**:
1. Keep Infrastructure/WebApi names → Less clear Hexagonal intent
2. Use "External" prefix → Not standard Hexagonal terminology
3. Rename all projects → Unnecessary for Domain/Application

**Consequences**:
- ✅ Clear Hexagonal terminology
- ✅ Intent is obvious to new developers
- ⚠️ Namespace changes in all files
- ⚠️ Update documentation references

---

## D003: Create Port Interfaces in Domain and Application Layers

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Need to define explicit contracts between core and adapters.

**Decision**:
- **Secondary Ports** (outbound): Domain layer at `Domain/Ports/IPersistence/`
- **Primary Ports** (inbound): Application layer at `Application/Ports/`

**Rationale**:
- Secondary ports define what domain needs → belongs in Domain
- Primary ports define what application offers → belongs in Application
- Follows Hexagonal Architecture standard placement
- Maintains dependency inversion (ports owned by core)

**Alternatives Considered**:
1. All ports in Domain → Primary ports not domain concern
2. All ports in Application → Secondary ports too close to use cases
3. Ports in separate project → Over-engineering for this size

**Consequences**:
- ✅ Clear ownership of contracts
- ✅ Dependency inversion preserved
- ✅ Standard Hexagonal pattern
- ⚠️ New directory structure to learn

---

## D004: Use Functional Grouping for Primary Ports

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Need to decide granularity of primary port interfaces.

**Decision**: Create functional port interfaces:
- `IStudentManagementPort` (all student operations)
- `ICourseManagementPort` (all course operations)
- `IEnrollmentManagementPort` (all enrollment operations)

**Rationale**:
- Cohesive grouping of related operations
- Easier to mock for testing (one interface per context)
- Aligns with bounded contexts in DDD
- Reduces interface proliferation

**Alternatives Considered**:
1. One interface per operation (ICreateStudent, IGetStudent) → Too many interfaces
2. Single IApplicationPort → Too broad, violates ISP
3. Per-entity interfaces (IStudentPort) → Less clear intent

**Consequences**:
- ✅ Cohesive interfaces
- ✅ Easier testing
- ✅ Aligns with DDD
- ⚠️ Larger interfaces (potential ISP violation if grows too large)

---

## D005: Preserve MediatR CQRS Pattern

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Current system uses MediatR for CQRS. Need to decide if this fits Hexagonal.

**Decision**: Keep MediatR CQRS implementation unchanged. Handlers are use cases in application core.

**Rationale**:
- MediatR is perfectly compatible with Hexagonal
- Handlers are use cases in the application layer
- CQRS provides clear separation of concerns
- No benefit to removing working pattern

**Alternatives Considered**:
1. Remove MediatR, use direct service calls → Loses CQRS benefits
2. Replace with custom mediator → Reinventing wheel
3. Use ports instead of MediatR → Conflates two patterns

**Consequences**:
- ✅ Preserve working CQRS pattern
- ✅ No relearning needed
- ✅ Handlers = Use Cases (conceptual alignment)
- ✅ Zero migration cost for this component

---

## D006: Rename Repository Interfaces to PersistencePort

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Current repository interfaces (IStudentRepository, etc.) need to align with Hexagonal terminology.

**Decision**: Rename repository interfaces to *PersistencePort:
- `IStudentRepository` → `IStudentPersistencePort`
- `ICourseRepository` → `ICoursePersistencePort`
- `IEnrollmentRepository` → `IEnrollmentPersistencePort`
- `IUnitOfWork` → `IUnitOfWorkPort`

**Rationale**:
- "Port" suffix makes Hexagonal intent explicit
- "Persistence" is more accurate than "Repository"
- Aligns with industry Hexagonal terminology
- Maintains same contract semantics

**Alternatives Considered**:
1. Keep IRepository names → Doesn't signal Hexagonal migration
2. Use IRepositoryPort → Redundant terminology
3. Use IStudentPort → Ambiguous (could be primary port)

**Consequences**:
- ✅ Clear Hexagonal terminology
- ✅ Accurate naming (persistence, not just repository)
- ⚠️ Search/replace across codebase
- ⚠️ Update all handler dependencies

---

## D007: Rename Repository Implementations to PersistenceAdapter

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Implementation classes need to signal they are adapters.

**Decision**: Rename implementations to *PersistenceAdapter:
- `StudentRepository` → `StudentPersistenceAdapter`
- `CourseRepository` → `CoursePersistenceAdapter`
- `EnrollmentRepository` → `EnrollmentPersistenceAdapter`
- `UnitOfWork` → `UnitOfWorkAdapter`

**Rationale**:
- "Adapter" clearly indicates Hexagonal pattern
- Pairs with *PersistencePort naming
- Makes architecture explicit in code

**Alternatives Considered**:
1. Keep Repository names → Less clear about pattern
2. Use Impl suffix → Generic, not Hexagonal-specific
3. Use EfCoreRepository → Too technology-specific

**Consequences**:
- ✅ Explicit Hexagonal terminology
- ✅ Pairs nicely with port naming
- ⚠️ DI registration updates needed
- ⚠️ Namespace changes

---

## D008: Introduce Application Services for Primary Ports

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Controllers currently call MediatR directly. Need a layer to implement primary ports.

**Decision**: Create Application Service classes in Adapters.WebApi that implement primary port interfaces and delegate to MediatR handlers.

Example:
```csharp
public class StudentManagementService : IStudentManagementPort
{
    private readonly IMediator _mediator;

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        return await _mediator.Send(new CreateStudentCommand(request));
    }
}
```

**Rationale**:
- Implements primary ports (application interface)
- Decouples controllers from MediatR dependency
- Allows testing controllers against port interfaces
- Enables swapping MediatR for alternative implementations

**Alternatives Considered**:
1. Controllers implement ports directly → Violates SRP
2. Skip primary ports → Not true Hexagonal
3. Handlers implement ports → Wrong layer

**Consequences**:
- ✅ True Hexagonal pattern with primary ports
- ✅ Controllers depend on abstractions
- ✅ Testable without MediatR
- ⚠️ Thin wrapper layer (some might see as over-engineering)
- ⚠️ Additional classes to maintain

---

## D009: Keep DTOs in Application Layer

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: DTOs currently in Application layer. Need to decide if they move or stay.

**Decision**: Keep DTOs in Application layer. Optionally split into Request/Response subdirectories.

**Rationale**:
- DTOs are application concerns, not domain
- Port interfaces use DTOs (Request/Response objects)
- Application layer owns data contracts
- Follows Clean Architecture placement

**Alternatives Considered**:
1. Move DTOs to Adapters.WebApi → Couples to specific adapter
2. Move DTOs to Domain → Violates domain purity
3. Create separate Contracts project → Over-engineering

**Consequences**:
- ✅ DTOs in correct layer
- ✅ Reusable across multiple adapters
- ✅ Maintains Clean Architecture principles
- ⚠️ May split into Request/Response folders for clarity

---

## D010: Preserve AutoMapper Usage

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: AutoMapper currently maps entities to DTOs. Decide if this continues.

**Decision**: Keep AutoMapper in Application layer for entity-to-DTO mapping.

**Rationale**:
- AutoMapper is working well
- Reduces boilerplate mapping code
- Mapping profiles clearly defined
- Compatible with Hexagonal Architecture

**Alternatives Considered**:
1. Manual mapping → More boilerplate, error-prone
2. Mapster → Requires relearning, migration cost
3. Remove mapping, expose entities → Violates encapsulation

**Consequences**:
- ✅ Existing mapping code preserved
- ✅ No relearning needed
- ✅ Clean separation of entities and DTOs
- ✅ Zero migration cost

---

## D011: Preserve FluentValidation Pipeline

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: FluentValidation currently integrated via MediatR behavior. Decide if this pattern continues.

**Decision**: Keep FluentValidation with ValidationBehavior in Application layer.

**Rationale**:
- Validation is application concern
- Pipeline behavior is elegant solution
- All validation logic centralized
- Compatible with Hexagonal Architecture

**Alternatives Considered**:
1. Move validation to port implementations → Scattered logic
2. Remove pipeline, validate in handlers → Duplicated code
3. Data annotations instead → Less expressive, less testable

**Consequences**:
- ✅ Centralized validation
- ✅ Pipeline pattern preserved
- ✅ Clean separation of concerns
- ✅ Zero migration cost

---

## D012: Gradual Migration Strategy (Strangler Fig Pattern)

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Need to decide migration approach: big bang vs gradual.

**Decision**: Use Strangler Fig pattern - gradual migration over 9 phases:
1. Research & Preparation
2. Define Port Interfaces
3. Restructure Projects
4. Create Persistence Adapters
5. Create API Adapters
6. Migrate Domain Logic
7. Update Dependency Injection
8. Testing & Validation
9. Documentation Updates

**Rationale**:
- Reduces risk of breaking changes
- Allows testing after each phase
- Team can continue working on features
- Rollback is easier if issues arise
- Follows industry best practice for migrations

**Alternatives Considered**:
1. Big bang migration → High risk, long downtime
2. Dual-run both architectures → Too complex
3. Greenfield rewrite → Throws away working code

**Consequences**:
- ✅ Low risk approach
- ✅ Testable at each phase
- ✅ Can pause/resume migration
- ⚠️ Longer total duration (4-6 weeks)
- ⚠️ Temporary inconsistency during migration

---

## D013: Maintain Zero Breaking Changes to API

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Public API is being used. Need to decide if breaking changes are acceptable.

**Decision**: Maintain 100% API compatibility throughout migration. All endpoints, request/response formats unchanged.

**Rationale**:
- Internal refactoring, not API redesign
- Avoids client impact
- Can migrate without coordination
- Proves migration is safe

**Alternatives Considered**:
1. Allow breaking changes → Requires client updates
2. Version API (v1 vs v2) → Unnecessary for internal refactoring
3. Deprecate old endpoints → Confusing during migration

**Consequences**:
- ✅ No client impact
- ✅ Safe, reversible migration
- ✅ Can migrate in production
- ⚠️ Cannot improve API during migration (separate effort)

---

## D014: No Performance Regression Tolerance

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Need to define acceptable performance impact.

**Decision**: Zero tolerance for performance regression. If any endpoint slows down, investigate and fix before proceeding.

**Rationale**:
- Migration should not degrade user experience
- Performance is a feature
- Additional layers (Application Services) are thin wrappers
- EF Core queries unchanged

**Alternatives Considered**:
1. Allow up to 10% slowdown → Unacceptable for users
2. Don't measure performance → Risky
3. Optimize after migration → Delays fixing issues

**Consequences**:
- ✅ User experience protected
- ✅ Performance monitoring required
- ⚠️ May need to optimize wrapper layers
- ⚠️ Requires baseline measurements (Phase 01)

---

## D015: Update Vietnamese Documentation in Phase 09

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Comprehensive Vietnamese docs exist in `docs/` directory. Need to decide when to update.

**Decision**: Update all Vietnamese documentation in Phase 09 after migration is complete and validated.

**Rationale**:
- Documentation should reflect final state
- Updating mid-migration creates confusion
- Team can reference current docs during work
- Single documentation update effort at end

**Alternatives Considered**:
1. Update docs continuously → Requires constant edits
2. Update docs per phase → Too much churn
3. Never update docs → Docs become outdated

**Consequences**:
- ✅ Single documentation effort
- ✅ Docs reflect final state accurately
- ⚠️ Docs temporarily outdated during migration
- ⚠️ Need to track all doc changes needed

---

## D016: Technology Stack Remains Unchanged

**Date**: 2025-11-17
**Status**: ✅ Accepted

**Context**: Hexagonal enables swapping adapters. Decide if we change technology now.

**Decision**: Preserve all current technologies:
- .NET 8.0
- SQLite + EF Core
- MediatR
- AutoMapper
- FluentValidation
- Serilog
- Swagger

**Rationale**:
- Migration is architecture change, not technology change
- Reduces scope and risk
- Technology swap can happen later (benefit of Hexagonal)
- Team familiar with current stack

**Alternatives Considered**:
1. Swap SQLite → PostgreSQL → Separate effort
2. Replace AutoMapper → No benefit, adds risk
3. Remove MediatR → Breaks working pattern

**Consequences**:
- ✅ Focused migration scope
- ✅ Lower risk
- ✅ Can swap technologies later
- ✅ Team remains productive

---

## Summary of Key Decisions

| ID | Decision | Impact | Status |
|----|----------|--------|--------|
| D001 | Adopt Hexagonal Architecture | High | ✅ Accepted |
| D002 | Rename projects to Adapters.* | Medium | ✅ Accepted |
| D003 | Create port interfaces | High | ✅ Accepted |
| D004 | Functional grouping for ports | Medium | ✅ Accepted |
| D005 | Preserve MediatR CQRS | Low | ✅ Accepted |
| D006 | Rename to *PersistencePort | Medium | ✅ Accepted |
| D007 | Rename to *PersistenceAdapter | Medium | ✅ Accepted |
| D008 | Introduce Application Services | Medium | ✅ Accepted |
| D009 | Keep DTOs in Application | Low | ✅ Accepted |
| D010 | Preserve AutoMapper | Low | ✅ Accepted |
| D011 | Preserve FluentValidation | Low | ✅ Accepted |
| D012 | Gradual migration (9 phases) | High | ✅ Accepted |
| D013 | Zero breaking API changes | High | ✅ Accepted |
| D014 | No performance regression | Medium | ✅ Accepted |
| D015 | Update docs in Phase 09 | Low | ✅ Accepted |
| D016 | Technology stack unchanged | Medium | ✅ Accepted |

## Decision-Making Principles Applied

1. **Minimize Risk**: Gradual migration, preserve working code
2. **Preserve Compatibility**: Zero API breaking changes
3. **Follow Standards**: Industry-standard Hexagonal patterns
4. **Practical Pragmatism**: Don't over-engineer, keep it simple
5. **Team Productivity**: Minimal learning curve, familiar tools

## Open Questions for Future Decisions

1. **Q1**: Should we add integration tests during migration or after?
   - **Lean toward**: During (Phase 08)

2. **Q2**: Should we introduce database indexing during migration?
   - **Lean toward**: After (separate performance optimization effort)

3. **Q3**: Should we implement authentication during or after migration?
   - **Lean toward**: After (separate security feature)

4. **Q4**: Should we containerize (Docker) during migration?
   - **Lean toward**: After (Phase 10+, separate effort)

5. **Q5**: Should we switch to PostgreSQL for production?
   - **Lean toward**: After migration complete (demonstrates Hexagonal benefit)

## Next Steps

- ✅ Phase 01 Complete: Research & Preparation
- ▶️ **Proceed to Phase 02**: Define Port Interfaces
- Create secondary ports in Domain layer
- Create primary ports in Application layer
- Verify compilation (no implementations yet)
