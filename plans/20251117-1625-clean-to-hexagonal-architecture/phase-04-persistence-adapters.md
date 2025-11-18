# Phase 04: Create Persistence Adapters

## Context Links
**Parent**: [plan.md](plan.md) | **Prev**: [Phase 03](phase-03-restructure-projects.md) | **Next**: [Phase 05](phase-05-api-adapters.md)

## Overview
**Duration**: 5-7 days | **Priority**: P0 | **Status**: ⏳ Pending

Convert repository implementations to explicit secondary adapters implementing persistence ports.

## Key Insights
- Repositories already implement interfaces - minimal code changes
- Rename implementations: `StudentRepository` → `EfCoreStudentAdapter`
- Keep EF Core DbContext (infrastructure detail)
- Adapters implement ports from Domain layer

## Requirements

Convert each repository:
- `StudentRepository` → `EfCoreStudentAdapter : IStudentPersistencePort`
- `CourseRepository` → `EfCoreCourseAdapter : ICoursePersistencePort`
- `EnrollmentRepository` → `EfCoreEnrollmentAdapter : IEnrollmentPersistencePort`
- `UnitOfWork` → `EfCoreUnitOfWorkAdapter : IUnitOfWorkPort`

## Architecture

### Before (Clean Architecture)
```csharp
public class StudentRepository : IStudentRepository
{
    private readonly StudentManagementDbContext _context;
    // Implementation...
}
```

### After (Hexagonal Architecture)
```csharp
public class EfCoreStudentAdapter : IStudentPersistencePort
{
    private readonly StudentManagementDbContext _context;
    // Same implementation, different name/interface
}
```

## Related Code Files

### Files to Modify
- `Adapters.Persistence/Repositories/Repository.cs` → Delete or rename base
- `Adapters.Persistence/Repositories/StudentRepository.cs` → `EfCoreStudentAdapter.cs`
- `Adapters.Persistence/Repositories/CourseRepository.cs` → `EfCoreCourseAdapter.cs`
- `Adapters.Persistence/Repositories/EnrollmentRepository.cs` → `EfCoreEnrollmentAdapter.cs`
- `Adapters.Persistence/Repositories/UnitOfWork.cs` → `EfCoreUnitOfWorkAdapter.cs`

### Files to Keep
- DbContext (infrastructure detail)
- Entity Configurations (EF Core specific)
- Migrations (database versioning)

## Implementation Steps

### Step 1: Rename Repository Files (1 hour)
- StudentRepository.cs → EfCoreStudentAdapter.cs
- Update class name inside
- Change interface: `IStudentRepository` → `IStudentPersistencePort`

### Step 2: Update Implementations (2-3 days)
For each adapter:
1. Implement `I*PersistencePort` from Domain
2. Keep EF Core logic (no changes needed)
3. Remove methods not in port interface (if any)
4. Add missing methods from port (if any)
5. Test compilation

### Step 3: Update DI Registration (1 hour)
In `Adapters.Persistence/DependencyInjection.cs`:
```csharp
// Before
services.AddScoped<IStudentRepository, StudentRepository>();

// After
services.AddScoped<IStudentPersistencePort, EfCoreStudentAdapter>();
```

### Step 4: Update Handler Dependencies (1 day)
In Application layer handlers:
```csharp
// Before
private readonly IStudentRepository _repository;

// After
private readonly IStudentPersistencePort _persistencePort;
```

### Step 5: Test (1-2 days)
- Unit tests for adapters
- Integration tests with real database
- Performance testing

## Todo List
- [ ] Rename StudentRepository to EfCoreStudentAdapter
- [ ] Update class to implement IStudentPersistencePort
- [ ] Repeat for CourseRepository
- [ ] Repeat for EnrollmentRepository
- [ ] Update UnitOfWork
- [ ] Update DI registrations
- [ ] Update handler dependencies (Students)
- [ ] Update handler dependencies (Courses)
- [ ] Update handler dependencies (Enrollments)
- [ ] Rebuild solution
- [ ] Fix compilation errors
- [ ] Write/update adapter tests
- [ ] Run integration tests
- [ ] Performance testing

## Success Criteria
1. ✅ All adapters implement persistence ports
2. ✅ No references to old `I*Repository` interfaces
3. ✅ All tests pass
4. ✅ No performance degradation
5. ✅ EF Core migrations still work

## Risk Assessment
**Medium Risk** - Many files to update, potential for missed references

**Mitigation**: Compiler errors will catch missing updates, comprehensive testing

## Next Steps
[Phase 05: Create API Adapters](phase-05-api-adapters.md)
