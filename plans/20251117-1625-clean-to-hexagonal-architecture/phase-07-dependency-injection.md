# Phase 07: Update Dependency Injection

## Context Links
**Parent**: [plan.md](plan.md) | **Prev**: [Phase 06](phase-06-migrate-domain-logic.md) | **Next**: [Phase 08](phase-08-testing-validation.md)

## Overview
**Duration**: 2-3 days | **Priority**: P0 | **Status**: ⏳ Pending

Wire ports to adapters via DI container, organize DI per adapter package.

## Key Insights
- Clean separation: each adapter project has own DI extension method
- Register ports → adapter implementations
- Maintain existing DI patterns (scoped, singleton, transient)
- Application, WebApi startup remains similar

## Requirements

Update DI configuration in:
1. `Domain` - None needed (no dependencies)
2. `Application` - Register MediatR, validators, AutoMapper
3. `Adapters.Persistence` - Register persistence ports → EF Core adapters
4. `Adapters.WebApi` - Register primary ports → Application Services

## Architecture

### DI Registration Pattern

**Application Layer**:
```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    return services;
}
```

**Adapters.Persistence**:
```csharp
public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<StudentManagementDbContext>(opt =>
        opt.UseSqlite(config.GetConnectionString("DefaultConnection")));

    // Secondary Ports → Adapters
    services.AddScoped<IStudentPersistencePort, EfCoreStudentAdapter>();
    services.AddScoped<ICoursePersistencePort, EfCoreCourseAdapter>();
    services.AddScoped<IEnrollmentPersistencePort, EfCoreEnrollmentAdapter>();
    services.AddScoped<IUnitOfWorkPort, EfCoreUnitOfWorkAdapter>();

    return services;
}
```

**Adapters.WebApi**:
```csharp
public static IServiceCollection AddWebApi(this IServiceCollection services)
{
    services.AddControllers();
    services.AddSwaggerGen();

    // Primary Ports → Application Services
    services.AddScoped<IStudentManagementPort, StudentApplicationService>();
    services.AddScoped<ICourseManagementPort, CourseApplicationService>();
    services.AddScoped<IEnrollmentManagementPort, EnrollmentApplicationService>();

    return services;
}
```

**Program.cs**:
```csharp
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddWebApi();
```

## Related Code Files

### Files to Modify
- `Application/DependencyInjection.cs` (minor updates)
- `Adapters.Persistence/DependencyInjection.cs` (update registrations)
- `Adapters.WebApi/DependencyInjection.cs` (update registrations)
- `Adapters.WebApi/Program.cs` (verify startup)

## Implementation Steps

### Step 1: Update Adapters.Persistence DI (2 hours)
1. Open `DependencyInjection.cs`
2. Replace `I*Repository` → `I*PersistencePort` registrations
3. Update implementation types (e.g., `EfCoreStudentAdapter`)
4. Test compilation

### Step 2: Update Adapters.WebApi DI (2 hours)
1. Add primary port registrations
2. Register Application Services
3. Verify controller, middleware registrations
4. Test compilation

### Step 3: Update Program.cs (1 hour)
1. Verify DI extension method calls
2. Rename if needed: `AddInfrastructure` → `AddPersistence`
3. Verify middleware pipeline
4. Test startup

### Step 4: Test DI Resolution (1 day)
1. Run application
2. Test dependency resolution at runtime
3. Use DI container diagnostics if available
4. Verify no circular dependencies

### Step 5: Integration Testing (1 day)
- Test full request flow (HTTP → Controller → App Service → Handler → Adapter)
- Verify all dependencies injected correctly
- Test different scopes (scoped, transient, singleton)

## Todo List
- [ ] Update Adapters.Persistence DI registrations
- [ ] Update Adapters.WebApi DI registrations
- [ ] Update Program.cs extension methods
- [ ] Rebuild solution
- [ ] Run application
- [ ] Test DI resolution
- [ ] Verify no circular dependencies
- [ ] Run integration tests
- [ ] Test all API endpoints
- [ ] Verify dependency scopes correct

## Success Criteria
1. ✅ All ports resolve to correct adapters
2. ✅ Application starts successfully
3. ✅ No DI resolution errors
4. ✅ All API endpoints work
5. ✅ Integration tests pass

## Risk Assessment
**Low Risk** - DI updates straightforward, compiler enforces correctness

**Mitigation**: Test DI resolution at startup, integration tests

## Next Steps
[Phase 08: Testing & Validation](phase-08-testing-validation.md)
