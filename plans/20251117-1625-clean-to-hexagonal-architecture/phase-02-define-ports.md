# Phase 02: Define Port Interfaces

## Context Links
**Parent**: [plan.md](plan.md) | **Prev**: [Phase 01](phase-01-research-preparation.md) | **Next**: [Phase 03](phase-03-restructure-projects.md)

## Overview
**Duration**: 3-5 days (Actual: 1 day) | **Priority**: P0 | **Status**: ✅ Complete
**Completed**: 2025-11-17

Create explicit port interfaces for all external communications (Primary/Driving ports for inbound, Secondary/Driven ports for outbound).

## Key Insights
- Ports are stable contracts, adapters are swappable implementations
- Primary ports define WHAT operations domain offers
- Secondary ports define WHAT domain needs from external world
- Use functional grouping (not per-entity) for port granularity

## Requirements

### Primary Ports (Inbound - Application Layer)
Create interfaces for external actors to interact with system:
- `IStudentManagementPort`: Create, Get, Update, Delete students
- `ICourseManagementPort`: Course CRUD operations
- `IEnrollmentManagementPort`: Enrollment and grade operations

### Secondary Ports (Outbound - Domain Layer)
Create interfaces for system to interact with external dependencies:
- `IStudentPersistencePort`: Student data access
- `ICoursePersistencePort`: Course data access
- `IEnrollmentPersistencePort`: Enrollment data access
- `IUnitOfWorkPort`: Transaction management

## Architecture

### Primary Port Pattern
```csharp
// Location: Application/Ports/IStudentManagementPort.cs
public interface IStudentManagementPort
{
    Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request);
    Task<StudentResponse> GetStudentByIdAsync(Guid id);
    Task<PagedResult<StudentSummary>> GetStudentsAsync(StudentFilter filter);
    Task<StudentResponse> UpdateStudentAsync(Guid id, UpdateStudentRequest request);
    Task DeleteStudentAsync(Guid id);
}
```

### Secondary Port Pattern
```csharp
// Location: Domain/Ports/IPersistence/IStudentPersistencePort.cs
public interface IStudentPersistencePort
{
    Task<Student> SaveAsync(Student student);
    Task<Student?> GetByIdAsync(StudentId id);
    Task<Student?> GetByEmailAsync(Email email);
    Task<IEnumerable<Student>> FindAsync(StudentFilter filter);
    Task<bool> ExistsAsync(StudentId id);
    Task DeleteAsync(Student student);
}
```

## Related Code Files

### Files to Create
**Primary Ports**:
- `Application/Ports/IStudentManagementPort.cs`
- `Application/Ports/ICourseManagementPort.cs`
- `Application/Ports/IEnrollmentManagementPort.cs`

**Secondary Ports**:
- `Domain/Ports/IPersistence/IStudentPersistencePort.cs`
- `Domain/Ports/IPersistence/ICoursePersistencePort.cs`
- `Domain/Ports/IPersistence/IEnrollmentPersistencePort.cs`
- `Domain/Ports/IPersistence/IUnitOfWorkPort.cs`

**Request/Response DTOs** (if not existing):
- `Application/DTOs/Requests/CreateStudentRequest.cs`
- `Application/DTOs/Responses/StudentResponse.cs`
- (Similar for Course, Enrollment)

## Implementation Steps

### Step 1: Create Port Directory Structure (30 min)
```bash
mkdir -p src/StudentManagement.Domain/Ports/IPersistence
mkdir -p src/StudentManagement.Application/Ports
```

### Step 2: Define Secondary Ports (4-6 hours)
For each aggregate (Student, Course, Enrollment):
1. Analyze existing `I*Repository` interface
2. Create corresponding `I*PersistencePort`
3. Map methods, adjust signatures if needed
4. Remove EF Core specific types (e.g., IQueryable)
5. Use domain entities, not DTOs

### Step 3: Define Primary Ports (4-6 hours)
For each bounded context:
1. Identify entry points from controllers
2. Group related operations
3. Create port interface with operation methods
4. Use Request/Response DTOs, not domain entities
5. Technology-agnostic signatures (no HTTP types)

### Step 4: Create Request/Response DTOs (2-3 hours)
1. Extract existing DTOs or create new
2. Separate input (Request) from output (Response)
3. Validation attributes if needed
4. Map to/from domain entities

### Step 5: Update Project References (1 hour)
- Ensure Domain project has no external dependencies
- Application project references Domain
- No circular dependencies

### Step 6: Code Review (2 hours)
- Team review of port interfaces
- Verify naming conventions
- Check method signatures
- Validate grouping decisions

## Todo List
- [ ] Create port directories
- [ ] Define IStudentPersistencePort
- [ ] Define ICoursePersistencePort
- [ ] Define IEnrollmentPersistencePort
- [ ] Define IUnitOfWorkPort
- [ ] Define IStudentManagementPort
- [ ] Define ICourseManagementPort
- [ ] Define IEnrollmentManagementPort
- [ ] Create Request DTOs
- [ ] Create Response DTOs
- [ ] Update project references
- [ ] Run compile check
- [ ] Conduct code review
- [ ] Get team approval

## Success Criteria
1. ✅ All port interfaces compile successfully
2. ✅ No breaking changes to existing code (interfaces added only)
3. ✅ Clear separation: Primary ports in Application, Secondary in Domain
4. ✅ Technology-agnostic signatures
5. ✅ Consistent naming conventions
6. ✅ Team approval obtained

## Risk Assessment
**Low Risk** - No existing code modified, only new interfaces added

**Mitigation**: Interfaces can be deleted if not used in Phase 03

## Next Steps
Proceed to [Phase 03: Restructure Project Layout](phase-03-restructure-projects.md)
