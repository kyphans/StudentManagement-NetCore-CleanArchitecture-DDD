# Task Completion Checklist

## When a Task is Completed

### Build and Compilation
- [ ] **Build the solution**: Run `dotnet build` to ensure no compilation errors
- [ ] **Check for warnings**: Address any compiler warnings that appear
- [ ] **Verify project references**: Ensure all project dependencies are correctly referenced

### Code Quality
- [ ] **Follow naming conventions**: Verify PascalCase for classes, camelCase for parameters
- [ ] **Check nullable reference types**: Ensure proper null handling with nullable enabled
- [ ] **Verify architectural boundaries**: Confirm dependencies flow in correct direction (Domain ← Application ← Infrastructure ← WebApi)
- [ ] **Apply SOLID principles**: Review code for single responsibility and dependency inversion

### Database Operations (if applicable)
- [ ] **Create migration**: Run `dotnet ef migrations add <MigrationName>` if database changes
- [ ] **Update database**: Run `dotnet ef database update` to apply schema changes
- [ ] **Test database operations**: Verify CRUD operations work as expected

### Testing (when test projects exist)
- [ ] **Run unit tests**: Execute `dotnet test` to ensure existing tests pass
- [ ] **Add new tests**: Write tests for new functionality, especially domain logic
- [ ] **Integration tests**: Test API endpoints and database interactions

### API Development
- [ ] **Test endpoints**: Verify API endpoints work correctly with proper HTTP status codes
- [ ] **Update Swagger docs**: Ensure API documentation is current
- [ ] **Validate request/response models**: Check DTOs and validation rules
- [ ] **Test authentication/authorization**: Verify role-based access controls work

### Configuration
- [ ] **Check appsettings.json**: Verify configuration values are correct
- [ ] **Environment-specific config**: Ensure Development/Production settings are appropriate
- [ ] **Connection strings**: Verify database connections work properly

### Logging and Monitoring
- [ ] **Add appropriate logging**: Use Serilog for structured logging where needed
- [ ] **Log levels**: Ensure proper log levels (Information, Warning, Error)
- [ ] **Error handling**: Implement proper exception handling and logging

### Documentation
- [ ] **Update CLAUDE.md**: Document any significant architectural changes
- [ ] **Code comments**: Add XML documentation for public APIs (only when necessary)
- [ ] **API documentation**: Ensure Swagger/OpenAPI docs are complete

### Security Considerations
- [ ] **Authentication flows**: Verify JWT token generation and validation
- [ ] **Authorization policies**: Test role-based and policy-based authorization
- [ ] **Input validation**: Ensure FluentValidation rules are applied
- [ ] **Sensitive data**: Verify no secrets are hardcoded or logged

### Final Checks
- [ ] **Clean solution**: Run `dotnet clean` then `dotnet build` for fresh build
- [ ] **Run application**: Start with `dotnet run --project src/StudentManagement.WebApi`
- [ ] **Manual testing**: Test the implemented functionality through API or UI
- [ ] **Performance check**: Basic verification that operations perform reasonably

### Before Committing (if using version control)
- [ ] **Review changes**: Examine all modified files
- [ ] **Check .gitignore**: Ensure build artifacts and sensitive files are ignored
- [ ] **Commit messages**: Write clear, descriptive commit messages
- [ ] **Small commits**: Make atomic commits that represent logical changes