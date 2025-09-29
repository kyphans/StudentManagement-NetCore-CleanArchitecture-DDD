# Student Management System

A comprehensive student management system built with Clean Architecture, Domain-Driven Design (DDD), and CQRS patterns using .NET 8.0 and SQLite.

## 🏗️ Architecture

This project implements **Clean Architecture** with the following layers:
- **Domain**: Core business logic and entities
- **Application**: Use cases and CQRS handlers
- **Infrastructure**: Data access and external services
- **WebApi**: REST API controllers and presentation

### Key Patterns
- **Clean Architecture** with proper dependency inversion
- **Domain-Driven Design (DDD)** with rich domain models
- **CQRS** pattern using MediatR
- **Repository Pattern** with Unit of Work
- **AutoMapper** for object-to-object mapping
- **FluentValidation** pipeline for input validation

## 🚀 Features
- **Student Management**: Create, read, update students with validation
- **Course Management**: Manage courses with prerequisites and enrollment limits
- **Enrollment System**: Handle student course enrollments and grading
- **Global Exception Handling**: Centralized error handling middleware
- **Response Compression**: Gzip compression for better performance
- **API Documentation**: Enhanced Swagger/OpenAPI documentation
- **Validation Pipeline**: FluentValidation integrated with MediatR
- **AutoMapper Integration**: Automatic entity-to-DTO mapping
- **Performance Optimization**: Database-level filtering and pagination
- **Caching Layer**: Redis or in-memory caching
- **Advanced Features**: Bulk operations, reporting, file handling
- **Production Readiness**: Docker, health checks, monitoring

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/) (included with .NET)
- IDE: [Visual Studio](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

## 🛠️ Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd StudentManagement
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Solution
```bash
dotnet build
```

### 4. Initialize Database
```bash
# Create and apply migrations
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

### 5. Run the Application
```bash
dotnet run --project src/StudentManagement.WebApi
```

The API will be available at `http://localhost:5282`

## 📖 Usage

### API Documentation
Once the application is running, access the Swagger UI at:
- **Swagger UI**: `http://localhost:5282/swagger`
- **API Docs**: `http://localhost:5282/swagger/v1/swagger.json`

### Sample API Calls

#### Create a Student
```bash
curl -X POST "http://localhost:5282/api/students" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@email.com",
    "dateOfBirth": "2000-01-15",
    "phoneNumber": "555-0123",
    "address": "123 Main St"
  }'
```

#### Create a Course
```bash
curl -X POST "http://localhost:5282/api/courses" \
  -H "Content-Type: application/json" \
  -d '{
    "code": "CS101",
    "name": "Introduction to Computer Science",
    "description": "Fundamental concepts of computer science",
    "creditHours": 3,
    "department": "Computer Science",
    "maxEnrollment": 30
  }'
```

#### Get Students (with filtering)
```bash
curl "http://localhost:5282/api/students?pageNumber=1&pageSize=10&searchTerm=John&isActive=true"
```

## 🏛️ Project Structure

```
src/
├── StudentManagement.Domain/           # Core business logic
│   ├── Entities/                      # Domain entities (Student, Course, Enrollment)
│   ├── ValueObjects/                  # Value objects (Email, GPA, CourseCode)
│   ├── Events/                        # Domain events
│   └── Repositories/                  # Repository interfaces
├── StudentManagement.Application/      # Use cases & CQRS
│   ├── Commands/                      # Write operations (Create, Update)
│   ├── Queries/                       # Read operations (Get, List)
│   ├── DTOs/                          # Data transfer objects
│   ├── Behaviors/                     # Cross-cutting concerns
│   ├── Validators/                    # FluentValidation rules
│   └── Mappings/                      # AutoMapper profiles
├── StudentManagement.Infrastructure/   # Data access & external services
│   ├── Data/                          # EF Core DbContext & configurations
│   ├── Repositories/                  # Repository implementations
│   └── Migrations/                    # Database migrations
└── StudentManagement.WebApi/          # REST API & presentation
    ├── Controllers/                   # API controllers
    ├── Middleware/                    # Custom middleware
    └── Program.cs                     # Application entry point
```

## 🔧 Configuration

### Database
The application uses SQLite by default. The database file `studentmanagement.db` will be created in the WebApi output directory.

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=studentmanagement.db"
  }
}
```

### CORS
Development CORS policy allows all origins. Configure appropriately for production in `appsettings.json`.

## 🧪 Testing

### Manual API Testing
The project includes comprehensive API testing through Swagger UI and curl commands.

### Running Tests
```bash
# Build and verify no compilation errors
dotnet build

# Run the application for integration testing
dotnet run --project src/StudentManagement.WebApi
```

**Note**: Unit and integration test projects will be added in future phases.

## 📚 API Reference

### Students API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/students` | Get paginated students with filtering |
| GET | `/api/students/{id}` | Get student by ID |
| POST | `/api/students` | Create new student |
| PUT | `/api/students/{id}` | Update existing student |

### Courses API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/courses` | Get paginated courses with filtering |
| GET | `/api/courses/{id}` | Get course by ID |
| POST | `/api/courses` | Create new course |
| PUT | `/api/courses/{id}` | Update existing course |

### Enrollments API
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/enrollments` | Get paginated enrollments with filtering |
| GET | `/api/enrollments/{id}` | Get enrollment by ID |
| POST | `/api/enrollments` | Create new enrollment |
| POST | `/api/enrollments/{id}/assign-grade` | Assign grade to enrollment |

### Response Format
All API responses follow this structure:
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully",
  "errors": [],
  "timestamp": "2025-01-29T10:13:34.914429Z"
}
```

## 🛡️ Error Handling

The application includes comprehensive error handling:
- **Global Exception Middleware**: Catches and formats all exceptions
- **Validation Errors**: FluentValidation errors returned with 400 status
- **Not Found**: 404 errors for missing resources
- **Server Errors**: 500 errors for unexpected exceptions

## 📦 Dependencies

### Core Dependencies
- **.NET 8.0**: Target framework
- **MediatR**: CQRS pattern implementation
- **Entity Framework Core**: ORM and SQLite provider
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation

### Development Dependencies
- **Swashbuckle.AspNetCore**: API documentation
- **Microsoft.EntityFrameworkCore.Tools**: EF Core CLI tools

## 🚀 Deployment

### Development
```bash
dotnet run --project src/StudentManagement.WebApi
```

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add <MigrationName> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Update database
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

## 🔄 Development Workflow

### Essential Commands
```bash
# Build and run
dotnet build
dotnet run --project src/StudentManagement.WebApi

# Database operations
dotnet ef migrations add <Name> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi

# Clean and rebuild
dotnet clean
dotnet build
```

## 📈 Performance

### Current Performance Characteristics
- **In-Memory Operations**: Filtering and pagination currently done in memory
- **Response Compression**: Gzip compression enabled
- **AutoMapper**: Optimized object mapping
- **Connection Pooling**: EF Core connection pooling enabled

### Optimization Opportunities (Phase 6)
- Move filtering/pagination to database level
- Add response caching
- Implement database indexing
- Add performance monitoring

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the existing code style and architecture patterns
4. Ensure all builds pass (`dotnet build`)
5. Test your changes thoroughly
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Code Style
- Follow Clean Architecture principles
- Use SOLID design principles
- Maintain separation of concerns
- Include appropriate validation
- Follow C# naming conventions

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For questions and support:
- Review the [Architecture Documentation](ARCHITECTURE_EXPLANATION_VN.md) (Vietnamese)
- Check the API documentation at `/swagger` when running
- Review the database schema in [DATABASE_STRUCTURE.md](DATABASE_STRUCTURE.md)

---

**Built with Clean Architecture 🏗️ | Domain-Driven Design 🎯 | CQRS ⚡**
