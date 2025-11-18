# Phase 05: Create API Adapters

## Context Links
**Parent**: [plan.md](plan.md) | **Prev**: [Phase 04](phase-04-persistence-adapters.md) | **Next**: [Phase 06](phase-06-migrate-domain-logic.md)

## Overview
**Duration**: 3-5 days | **Priority**: P0 | **Status**: ⏳ Pending

Convert controllers to primary adapters, create Application Services implementing primary ports.

## Key Insights
- Controllers remain thin, delegate to Application Services
- Application Services implement primary ports
- Application Services use MediatR internally (preserve CQRS)
- No breaking changes to API contracts

## Requirements

Create Application Services:
- `StudentApplicationService : IStudentManagementPort`
- `CourseApplicationService : ICourseManagementPort`
- `EnrollmentApplicationService : IEnrollmentManagementPort`

Controllers call Application Services instead of MediatR directly.

## Architecture

### Application Service Pattern
```csharp
// Adapters.WebApi/ApplicationServices/StudentApplicationService.cs
public class StudentApplicationService : IStudentManagementPort
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
    {
        var command = _mapper.Map<CreateStudentCommand>(request);
        var result = await _mediator.Send(command);

        if (!result.Success)
            throw new BusinessException(result.Message, result.Errors);

        return _mapper.Map<StudentResponse>(result.Data);
    }
}
```

### Controller Pattern
```csharp
// Adapters.WebApi/Controllers/StudentsController.cs
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentManagementPort _studentPort;

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var result = await _studentPort.CreateStudentAsync(request);
        return CreatedAtAction(nameof(GetStudent), new { id = result.Id }, result);
    }
}
```

## Related Code Files

### Files to Create
- `Adapters.WebApi/ApplicationServices/StudentApplicationService.cs`
- `Adapters.WebApi/ApplicationServices/CourseApplicationService.cs`
- `Adapters.WebApi/ApplicationServices/EnrollmentApplicationService.cs`

### Files to Modify
- `Adapters.WebApi/Controllers/StudentsController.cs`
- `Adapters.WebApi/Controllers/CoursesController.cs`
- `Adapters.WebApi/Controllers/EnrollmentsController.cs`
- `Adapters.WebApi/DependencyInjection.cs`

## Implementation Steps

### Step 1: Create ApplicationServices Directory (15 min)
```bash
mkdir src/StudentManagement.Adapters.WebApi/ApplicationServices
```

### Step 2: Implement StudentApplicationService (1 day)
1. Create class implementing `IStudentManagementPort`
2. Inject IMediator, IMapper
3. For each port method, map Request → Command, send via MediatR
4. Map result → Response
5. Handle errors appropriately

### Step 3: Repeat for Course, Enrollment (1 day)
Similar implementation for other aggregates

### Step 4: Update Controllers (1 day)
For each controller:
1. Replace `IMediator` dependency with `I*ManagementPort`
2. Update methods to call port instead of mediator
3. Keep HTTP decorators, validation
4. Maintain API contracts (no breaking changes)

### Step 5: Update DI (1 hour)
In `Adapters.WebApi/DependencyInjection.cs`:
```csharp
services.AddScoped<IStudentManagementPort, StudentApplicationService>();
services.AddScoped<ICourseManagementPort, CourseApplicationService>();
services.AddScoped<IEnrollmentManagementPort, EnrollmentApplicationService>();
```

### Step 6: Test (1-2 days)
- API integration tests
- Swagger UI manual testing
- Postman/curl testing
- Performance testing

## Todo List
- [ ] Create ApplicationServices directory
- [ ] Implement StudentApplicationService
- [ ] Implement CourseApplicationService
- [ ] Implement EnrollmentApplicationService
- [ ] Update StudentsController
- [ ] Update CoursesController
- [ ] Update EnrollmentsController
- [ ] Update DI registrations
- [ ] Rebuild solution
- [ ] Run API tests
- [ ] Manual testing via Swagger
- [ ] Performance testing
- [ ] Verify no API contract changes

## Success Criteria
1. ✅ All Application Services implement primary ports
2. ✅ Controllers delegate to ports
3. ✅ No breaking changes to API contracts
4. ✅ All API tests pass
5. ✅ Swagger documentation works
6. ✅ No performance degradation

## Risk Assessment
**Low-Medium Risk** - Controllers are thin, minimal logic to change

**Mitigation**: Comprehensive API testing, maintain existing contracts

## Next Steps
[Phase 06: Migrate Domain Logic](phase-06-migrate-domain-logic.md)
