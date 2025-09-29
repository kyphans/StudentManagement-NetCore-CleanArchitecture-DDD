# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Student Management System** built using **Clean Architecture** with **Domain-Driven Design (DDD)** principles in .NET 8.0.

**Architecture**: 4-layer Clean Architecture (Domain → Application → Infrastructure → WebApi)  
**Database**: SQLite with Entity Framework Core  
**Authentication**: JWT Bearer tokens with ASP.NET Core Identity  
**Patterns**: CQRS via MediatR, Repository Pattern, Domain Events

> **📋 Detailed Information**: See memory bank files for comprehensive architecture rules, tech stack details, and implementation guidance.

## Essential Commands

### Quick Start
```bash
# Build and run
dotnet build
dotnet run --project src/StudentManagement.WebApi

# Database migrations
dotnet ef migrations add <Name> -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
dotnet ef database update -p src/StudentManagement.Infrastructure -s src/StudentManagement.WebApi
```

> **📋 Complete Command Reference**: See `suggested_commands` memory file for comprehensive development commands.

## Core Architecture Rules

### Dependency Flow (Strict)
- **Domain** → No external dependencies  
- **Application** → Domain only  
- **Infrastructure** → Domain + Application  
- **WebApi** → Application + Infrastructure

### Key Patterns
- **CQRS**: Commands (modify) and Queries (read) via MediatR
- **Repository**: Interfaces in Domain, implementations in Infrastructure
- **JWT Auth**: Role-based authorization (Admin, Teacher, Student, Staff)

> **📋 Complete Architecture Guide**: See `architecture-comprehensive` memory file for detailed rules, patterns, and conventions.

## Configuration

### Database
- **File**: `studentmanagement.db` (SQLite, created in WebApi output directory)
- **Connection**: `Data Source=studentmanagement.db` in `appsettings.json`

### JWT Settings (Required)
```json
{
  "JwtSettings": {
    "Secret": "256-bit-secret-key",
    "Issuer": "StudentManagement", 
    "Audience": "StudentManagementUsers",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

> **📋 Complete Configuration**: See `jwt_authentication_config` memory file for detailed auth setup.

## Implementation Status

**Current Phase**: Phase 1 (Foundation) ✅ COMPLETE  
**Next Phase**: Phase 2 (Domain Entities) - Ready to implement

> **📋 Detailed Status**: See `implementation-status-comprehensive` memory file for complete phase planning and current limitations.