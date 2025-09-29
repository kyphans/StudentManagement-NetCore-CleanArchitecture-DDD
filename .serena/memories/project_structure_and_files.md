# Project Structure and Key Files

## Solution Structure
```
StudentManagement/
├── StudentManagement.sln                    # Solution file
├── CLAUDE.md                               # Claude Code instructions
├── ddd-dotnet.md                          # Implementation plan document
├── src/
│   ├── StudentManagement.Domain/           # Domain layer (pure business logic)
│   │   ├── Entities/                      # Domain entities (planned)
│   │   ├── ValueObjects/                  # Value objects (planned)
│   │   ├── Events/                        # Domain events (planned)
│   │   ├── Services/                      # Domain services (planned)
│   │   ├── Repositories/                  # Repository interfaces (planned)
│   │   ├── Class1.cs                      # Placeholder file
│   │   └── StudentManagement.Domain.csproj
│   ├── StudentManagement.Application/       # Application layer (use cases)
│   │   ├── Commands/                      # CQRS commands (planned)
│   │   ├── Queries/                       # CQRS queries (planned)
│   │   ├── DTOs/                         # Data transfer objects (planned)
│   │   ├── Services/                      # Application services (planned)
│   │   ├── Auth/                         # Auth interfaces (planned)
│   │   ├── Common/                       # Common logic (planned)
│   │   ├── Class1.cs                     # Placeholder file
│   │   └── StudentManagement.Application.csproj
│   ├── StudentManagement.Infrastructure/    # Infrastructure layer
│   │   ├── Data/                         # EF Core DbContext (planned)
│   │   ├── Auth/                         # JWT & Identity services (planned)
│   │   ├── Repositories/                 # Repository implementations (planned)
│   │   ├── Services/                     # External services (planned)
│   │   ├── Migrations/                   # EF Core migrations (planned)
│   │   ├── Class1.cs                     # Placeholder file
│   │   └── StudentManagement.Infrastructure.csproj
│   └── StudentManagement.WebApi/          # Presentation layer
│       ├── Controllers/                   # API controllers (planned)
│       ├── Middleware/                    # Custom middleware (planned)
│       ├── Models/                        # Request/response models (planned)
│       ├── Configuration/                 # DI configuration (planned)
│       ├── Properties/
│       │   └── launchSettings.json        # Launch configuration
│       ├── Program.cs                     # Application entry point
│       ├── appsettings.json              # Main configuration
│       ├── appsettings.Development.json  # Development settings
│       ├── StudentManagement.WebApi.http # HTTP test file
│       └── StudentManagement.WebApi.csproj
└── .serena/                              # Serena MCP tool data
    └── .claude/                          # Claude Code data
```

## Key Configuration Files

### appsettings.json
- SQLite connection string: `Data Source=studentmanagement.db`
- JWT settings with secret key, issuer, audience, expiry times
- Logging configuration
- CORS settings

### Program.cs (Current State)
- Basic ASP.NET Core setup with Swagger
- Contains demo WeatherForecast endpoint (to be replaced)
- Missing: DI configuration, authentication setup, MediatR registration

### Project Files (.csproj)
- **Domain**: Pure .NET 8.0, no external dependencies
- **Application**: MediatR, FluentValidation, AutoMapper, DI abstractions
- **Infrastructure**: EF Core SQLite, Identity, JWT tokens
- **WebApi**: Authentication, Swagger, Serilog, AutoMapper extensions

## Database
- **File Location**: `studentmanagement.db` (SQLite file in WebApi output directory)
- **Provider**: Entity Framework Core with SQLite
- **Identity**: ASP.NET Core Identity integration planned
- **Migrations**: Not yet created (pending Phase 2 implementation)

## Development Files
- **Solution**: Multi-project structure with proper layer dependencies
- **IDE**: JetBrains Rider configuration files present
- **Build Artifacts**: bin/ and obj/ directories (ignored in git)
- **Runtime**: .NET 8.0 assemblies and dependencies in bin/Debug/net8.0/

## Implementation Status
- ✅ **Solution structure** created with 4 projects
- ✅ **NuGet packages** installed per layer requirements  
- ✅ **Basic configuration** setup in appsettings.json
- ❌ **Domain entities** not implemented (placeholder Class1.cs files exist)
- ❌ **Application services** not implemented
- ❌ **Infrastructure repositories** not implemented
- ❌ **API controllers** not implemented (only demo WeatherForecast)
- ❌ **Database migrations** not created
- ❌ **Authentication setup** not configured in Program.cs