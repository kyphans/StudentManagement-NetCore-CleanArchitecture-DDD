# Suggested Development Commands

## Build & Run Commands

### Build the Solution
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/StudentManagement.WebApi
dotnet build src/StudentManagement.Domain
dotnet build src/StudentManagement.Application
dotnet build src/StudentManagement.Infrastructure
```

### Run the Application
```bash
# Run the Web API (from root directory)
dotnet run --project src/StudentManagement.WebApi

# Run with file watching for development
dotnet watch --project src/StudentManagement.WebApi
```

### Restore Dependencies
```bash
# Restore NuGet packages for entire solution
dotnet restore

# Restore for specific project
dotnet restore src/StudentManagement.WebApi
```

## Database Commands (Entity Framework Core)

### Migrations
```bash
# Add new migration
dotnet ef migrations add <MigrationName> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Apply migrations to database
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Remove last migration
dotnet ef migrations remove -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# List migrations
dotnet ef migrations list -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

### Database Management
```bash
# Drop database (SQLite file will be deleted)
dotnet ef database drop -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Generate SQL script from migrations
dotnet ef migrations script -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

## Testing Commands
```bash
# Run all tests (when test projects are created)
dotnet test

# Run tests for specific project
dotnet test src/StudentManagement.Domain.Tests/
dotnet test src/StudentManagement.Application.Tests/

# Run single test method
dotnet test --filter "TestMethodName"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Package Management
```bash
# Add package to specific project
dotnet add src/StudentManagement.Application package <PackageName>

# Remove package
dotnet remove src/StudentManagement.Application package <PackageName>

# List packages
dotnet list package

# Update packages
dotnet list package --outdated
```

## Solution Management
```bash
# Add project to solution
dotnet sln add src/StudentManagement.NewProject/StudentManagement.NewProject.csproj

# Remove project from solution
dotnet sln remove src/StudentManagement.OldProject/StudentManagement.OldProject.csproj

# List projects in solution
dotnet sln list
```

## macOS-specific Utilities
```bash
# File operations (macOS/Darwin)
ls -la              # List files with details
find . -name "*.cs" # Find C# files
grep -r "pattern"   # Search for patterns
open .              # Open current directory in Finder

# Process management
ps aux | grep dotnet    # Find running dotnet processes
kill -9 <PID>          # Force kill process
```

## Git Commands (Repository Management)
```bash
git status
git add .
git commit -m "message"
git push origin main
git pull origin main
git branch -a
git checkout -b feature/branch-name
```