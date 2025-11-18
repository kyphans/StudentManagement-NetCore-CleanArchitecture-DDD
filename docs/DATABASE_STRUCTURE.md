# Student Management System - Database Structure & Entities

## Overview

The Student Management System uses **SQLite** database with **Entity Framework Core 9.0** and follows **Domain-Driven Design (DDD)** principles. The database schema is implemented using Clean Architecture patterns with strongly-typed entities and value objects.

**Database File**: `studentmanagement.db` (SQLite)
**Location**: WebApi output directory
**Connection String**: `Data Source=studentmanagement.db`

## Entity Relationship Diagram

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Student   │────────▶│  Enrollment  │◀────────│   Course    │
│             │         │              │         │             │
│ + StudentId │         │ + Id (Guid)  │         │ + Id (Guid) │
│ + FirstName │         │ + StudentId  │         │ + Code      │
│ + LastName  │         │ + CourseId   │         │ + Name      │
│ + Email     │         │ + Status     │         │ + Department│
│ + DOB       │         │ + Grade      │         │ + Credits   │
│ + IsActive  │         │              │         │ + IsActive  │
└─────────────┘         └──────────────┘         └─────────────┘
                                │                        │
                                ▼                        │
                        ┌──────────────┐                 │
                        │    Grade     │                 │
                        │              │                 │
                        │ + Id (Guid)  │                 │
                        │ + Letter     │                 │
                        │ + Points     │                 │
                        │ + Score      │                 │
                        │ + Comments   │                 │
                        └──────────────┘                 │
                                                         │
                                                         ▼
                                                ┌──────────────┐
                                                │Prerequisites │
                                                │ (Self-Ref)   │
                                                │              │
                                                │ Course ──▶   │
                                                │ Course       │
                                                └──────────────┘
```

## Core Entities

### 1. Student Entity

**Table**: `Students`
**Primary Key**: `StudentId` (Strong-typed GUID wrapper)

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | GUID | PK, NOT NULL | Unique student identifier |
| FirstName | NVARCHAR(50) | NOT NULL | Student's first name (2-50 chars) |
| LastName | NVARCHAR(50) | NOT NULL | Student's last name (2-50 chars) |
| Email | NVARCHAR(255) | NOT NULL, UNIQUE | Student's email (validated) |
| DateOfBirth | DATE | NOT NULL | Birth date (13-120 years old) |
| EnrollmentDate | DATETIME | NOT NULL | Initial enrollment date |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status flag |
| CreatedAt | DATETIME | NOT NULL | Record creation timestamp |
| UpdatedAt | DATETIME | NOT NULL | Last update timestamp |

**Domain Rules**:
- Names must be 2-50 characters, trimmed
- Age must be between 13-120 years
- Email format validation with uniqueness constraint
- Automatic timestamp management
- Soft delete via `IsActive` flag

**Business Methods**:
- `CalculateGPA()` - Computes GPA from completed enrollments
- `AddEnrollment()` - Validates and adds course enrollment
- `UpdatePersonalInfo()` - Updates name and email with validation
- `Activate()` / `Deactivate()` - Manages active status

### 2. Course Entity

**Table**: `Courses`
**Primary Key**: `Id` (GUID)

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | GUID | PK, NOT NULL | Unique course identifier |
| Code | NVARCHAR(20) | NOT NULL, UNIQUE | Course code (e.g., "CS101") |
| Name | NVARCHAR(100) | NOT NULL | Course name (3-100 chars) |
| Description | NVARCHAR(500) | NULL | Course description |
| CreditHours | INT | NOT NULL | Credit hours (1-10) |
| Department | NVARCHAR(50) | NOT NULL | Department name (2-50 chars) |
| MaxEnrollment | INT | NOT NULL, DEFAULT 30 | Maximum students (1-500) |
| Prerequisites | NVARCHAR(MAX) | NULL | Comma-separated course IDs |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active status flag |
| CreatedAt | DATETIME | NOT NULL | Record creation timestamp |
| UpdatedAt | DATETIME | NOT NULL | Last update timestamp |

**Domain Rules**:
- Course code follows institutional format validation
- Credit hours limited to 1-10 range
- Department name validation (2-50 characters)
- Prerequisites stored as serialized GUID list
- Maximum enrollment capacity validation (1-500)

**Business Methods**:
- `CanEnroll()` - Checks availability and active status
- `AddPrerequisite()` / `RemovePrerequisite()` - Manages prerequisites
- `AddEnrollment()` - Validates enrollment capacity
- `UpdateCourseInfo()` - Updates course details with validation

### 3. Enrollment Entity

**Table**: `Enrollments`
**Primary Key**: `Id` (GUID)

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | GUID | PK, NOT NULL | Unique enrollment identifier |
| StudentId | GUID | FK, NOT NULL | Reference to Student |
| CourseId | GUID | FK, NOT NULL | Reference to Course |
| EnrollmentDate | DATETIME | NOT NULL | Enrollment timestamp |
| CompletionDate | DATETIME | NULL | Completion/withdrawal date |
| Status | INT | NOT NULL | Enrollment status enum |
| CreditHours | INT | NOT NULL | Credit hours for this enrollment |
| GradeId | GUID | FK, NULL | Reference to Grade (optional) |
| CreatedAt | DATETIME | NOT NULL | Record creation timestamp |
| UpdatedAt | DATETIME | NOT NULL | Last update timestamp |

**Enrollment Status Enum**:
- `Active (1)` - Currently enrolled
- `Completed (2)` - Successfully completed
- `Withdrawn (3)` - Withdrawn from course

**Domain Rules**:
- Credit hours validation (1-10)
- One active enrollment per student per course
- Status transitions: Active → Completed/Withdrawn
- Grade required before completion
- Completion date auto-set on status change

**Business Methods**:
- `AssignGrade()` - Assigns grade to active enrollment
- `Complete()` - Marks as completed (requires grade)
- `Withdraw()` - Withdraws from course
- `Reactivate()` - Reactivates withdrawn enrollment

### 4. Grade Entity

**Table**: `Grades`
**Primary Key**: `Id` (GUID)

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | GUID | PK, NOT NULL | Unique grade identifier |
| LetterGrade | NVARCHAR(5) | NOT NULL | Letter grade (A+, A, B+, etc.) |
| GradePoints | DECIMAL(3,2) | NOT NULL | GPA points (0.0-4.0) |
| NumericScore | DECIMAL(5,2) | NULL | Numeric score (0-100) |
| Comments | NVARCHAR(500) | NULL | Instructor comments |
| GradedDate | DATETIME | NOT NULL | Grade assignment date |
| GradedBy | NVARCHAR(100) | NOT NULL | Instructor identifier |
| CreatedAt | DATETIME | NOT NULL | Record creation timestamp |
| UpdatedAt | DATETIME | NOT NULL | Last update timestamp |

**Letter Grades**: A+, A, A-, B+, B, B-, C+, C, C-, D+, D, D-, F, I (Incomplete), W (Withdrawn)

**Grade Point Scale**:
- A+ / A: 4.0
- A-: 3.7
- B+: 3.3
- B: 3.0
- B-: 2.7
- C+: 2.3
- C: 2.0
- C-: 1.7
- D+: 1.3
- D: 1.0
- D-: 0.7
- F: 0.0

**Domain Rules**:
- Letter grade validation against allowed values
- Grade points range validation (0.0-4.0)
- Numeric score validation (0-100)
- Comments length limitation (500 chars)
- Automatic grade point calculation from numeric score

**Business Methods**:
- `CreateFromNumericScore()` - Auto-converts numeric to letter grade
- `UpdateGrade()` - Updates grade information with validation
- `UpdateComments()` - Updates instructor comments

## Value Objects

### 1. StudentId
Strong-typed wrapper for GUID ensuring type safety for student identifiers.

### 2. Email
Value object with built-in email format validation and case normalization.

### 3. CourseCode
Validated course code following institutional standards (e.g., "CS101", "MATH200").

### 4. GPA
Calculated GPA value object with precision validation (0.0-4.0 range).

## Base Entity

All entities inherit from `BaseEntity<TId>` providing:

- **Id**: Primary key of type `TId`
- **CreatedAt**: Automatic creation timestamp
- **UpdatedAt**: Automatic update timestamp
- **UpdateTimestamp()**: Manual timestamp update
- **Equality**: ID-based equality comparison
- **Operators**: == and != operator overloads

## Database Relationships

### One-to-Many Relationships

1. **Student → Enrollments**
   - One student can have multiple enrollments
   - Foreign Key: `Enrollment.StudentId → Student.Id`
   - Navigation: `Student.Enrollments` (read-only collection)

2. **Course → Enrollments**
   - One course can have multiple enrollments
   - Foreign Key: `Enrollment.CourseId → Course.Id`
   - Navigation: `Course.Enrollments` (read-only collection)

### One-to-One Relationships

3. **Enrollment → Grade**
   - One enrollment can have zero or one grade
   - Foreign Key: `Enrollment.GradeId → Grade.Id`
   - Navigation: `Enrollment.Grade` (nullable)

### Self-Referencing Relationships

4. **Course → Prerequisites**
   - Many-to-many self-reference through serialized GUID list
   - Storage: JSON array in `Course.Prerequisites` column
   - Navigation: `Course.Prerequisites` (read-only GUID collection)

## Entity Framework Configuration

### Key Features

- **Strong Typing**: Custom value objects for IDs and domains
- **Value Conversions**: Email, CourseCode, StudentId conversions
- **JSON Serialization**: Prerequisites stored as JSON array
- **Cascade Rules**: Proper cascade delete configuration
- **Indexes**: Performance optimized with strategic indexing
- **Constraints**: Database-level validation enforcement

### Configuration Files

- `StudentConfiguration.cs` - Student entity mapping
- `CourseConfiguration.cs` - Course entity and prerequisites mapping with value comparer
- `EnrollmentConfiguration.cs` - Enrollment relationships and constraints
- `GradeConfiguration.cs` - Grade entity mapping and validations

### Recent Improvements

- **Value Comparer for Prerequisites**: Added proper collection comparison for Course prerequisites to eliminate EF Core warnings and ensure accurate change tracking

## Sample Data Structure

```json
{
  "Students": [
    {
      "Id": "550e8400-e29b-41d4-a716-446655440001",
      "FirstName": "John",
      "LastName": "Doe",
      "Email": "john.doe@university.edu",
      "DateOfBirth": "2000-01-15",
      "EnrollmentDate": "2024-09-01",
      "IsActive": true
    }
  ],
  "Courses": [
    {
      "Id": "550e8400-e29b-41d4-a716-446655440002",
      "Code": "CS101",
      "Name": "Introduction to Computer Science",
      "Description": "Fundamental concepts of programming",
      "CreditHours": 3,
      "Department": "Computer Science",
      "MaxEnrollment": 30,
      "Prerequisites": "[]",
      "IsActive": true
    }
  ],
  "Enrollments": [
    {
      "Id": "550e8400-e29b-41d4-a716-446655440003",
      "StudentId": "550e8400-e29b-41d4-a716-446655440001",
      "CourseId": "550e8400-e29b-41d4-a716-446655440002",
      "EnrollmentDate": "2024-09-01",
      "Status": 1,
      "CreditHours": 3,
      "GradeId": null
    }
  ]
}
```

## Database Constraints & Indexes

### Primary Keys
- All entities use GUID primary keys for global uniqueness
- Clustered indexes on primary keys for performance

### Unique Constraints
- `Students.Email` - Unique constraint
- `Courses.Code` - Unique constraint

### Foreign Key Constraints
- `Enrollments.StudentId → Students.Id`
- `Enrollments.CourseId → Courses.Id`
- `Enrollments.GradeId → Grades.Id` (nullable)

### Check Constraints
- Credit hours validation (1-10)
- Grade points validation (0.0-4.0)
- Age validation (13-120 years)
- Enrollment status validation (1-3)

### Performance Indexes
- `IX_Enrollments_StudentId` - Student enrollment queries
- `IX_Enrollments_CourseId` - Course enrollment queries
- `IX_Students_Email` - Email lookup optimization
- `IX_Courses_Code` - Course code searches
- `IX_Courses_Department` - Department filtering

## Migration History

The database schema is managed through Entity Framework Core migrations:

- **Initial Migration**: Base entity structure
- **Course Prerequisites**: JSON serialization for prerequisites
- **Grade System**: Comprehensive grading implementation
- **Performance Indexes**: Optimized query performance
- **Validation Constraints**: Domain rule enforcement

---

*Generated: 2024-09-29*
*Student Management System v1.0*
*Clean Architecture with Domain-Driven Design*