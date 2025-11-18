# Phase 06: Migrate Domain Logic

## Context Links
**Parent**: [plan.md](plan.md) | **Prev**: [Phase 05](phase-05-api-adapters.md) | **Next**: [Phase 07](phase-07-dependency-injection.md)

## Overview
**Duration**: 2-3 days | **Priority**: P1 | **Status**: ⏳ Pending

Ensure domain layer uses ports instead of concrete dependencies, verify business logic intact.

## Key Insights
- Domain layer already has NO external dependencies (Clean Architecture)
- Minimal changes needed
- Verify use cases (handlers) depend on ports, not concrete implementations
- Business logic should be untouched

## Requirements

1. Domain entities have no external dependencies ✅ (already true)
2. Use case handlers depend on port interfaces
3. No references to `I*Repository` - use `I*PersistencePort`
4. Business logic unchanged

## Architecture

### Use Case Handler Pattern
```csharp
// Application/UseCases/Commands/CreateStudentCommandHandler.cs
public class CreateStudentCommandHandler
    : IRequestHandler<CreateStudentCommand, ApiResponseDto<StudentDto>>
{
    private readonly IStudentPersistencePort _persistencePort; // Not IStudentRepository
    private readonly IUnitOfWorkPort _unitOfWork;
    private readonly IMapper _mapper;

    public async Task<ApiResponseDto<StudentDto>> Handle(...)
    {
        var email = new Email(request.Email);
        var existing = await _persistencePort.GetByEmailAsync(email);
        // ...domain logic...
        await _persistencePort.SaveAsync(student);
        await _unitOfWork.CommitAsync();
        // ...
    }
}
```

## Related Code Files

### Files to Modify
All command/query handlers in `Application/Commands/` and `Application/Queries/`:
- CreateStudentCommandHandler.cs
- UpdateStudentCommandHandler.cs
- DeleteStudentCommandHandler.cs
- GetStudentsQueryHandler.cs
- GetStudentByIdQueryHandler.cs
- (Similar for Course, Enrollment)

## Implementation Steps

### Step 1: Audit Domain Layer (2 hours)
1. Verify no external dependencies in Domain/Entities
2. Verify no external dependencies in Domain/ValueObjects
3. Verify no leakage of infrastructure concerns
4. Document findings

### Step 2: Update Handler Dependencies (1 day)
For each handler:
1. Replace `I*Repository` with `I*PersistencePort`
2. Update variable names (_repository → _persistencePort)
3. Verify method calls match port interface
4. Test compilation

### Step 3: Verify Business Logic (1 day)
1. Review entity methods (Student.Create, Enrollment.AssignGrade, etc.)
2. Ensure domain invariants still enforced
3. Verify value object validation intact
4. Run domain unit tests

### Step 4: Integration Testing (1 day)
- Test full workflows (create student, enroll, assign grade)
- Verify business rules enforced
- Check domain events raised correctly

## Todo List
- [ ] Audit Domain/Entities for external dependencies
- [ ] Audit Domain/ValueObjects
- [ ] Update CreateStudentCommandHandler
- [ ] Update UpdateStudentCommandHandler
- [ ] Update DeleteStudentCommandHandler
- [ ] Update GetStudentsQueryHandler
- [ ] Update GetStudentByIdQueryHandler
- [ ] Repeat for Course handlers
- [ ] Repeat for Enrollment handlers
- [ ] Rebuild solution
- [ ] Run domain unit tests
- [ ] Run integration tests
- [ ] Verify business rules enforced
- [ ] Verify domain events work

## Success Criteria
1. ✅ Domain layer has zero external dependencies
2. ✅ All handlers use port interfaces
3. ✅ Business logic unchanged
4. ✅ All domain tests pass
5. ✅ Integration tests pass

## Risk Assessment
**Low Risk** - Clean Architecture already isolates domain

**Mitigation**: Comprehensive testing of business rules

## Next Steps
[Phase 07: Update Dependency Injection](phase-07-dependency-injection.md)
