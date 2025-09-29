# JWT Authentication Configuration

## JWT Settings Structure
JWT configuration requires these `appsettings.json` keys:

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

## Authentication Flow
- **JWT Bearer tokens** with ASP.NET Core Identity
- **Role-based authorization**: Admin, Teacher, Student, Staff
- **Database token storage** via Identity tables
- **Token refresh capabilities** with configurable expiry
- **Claims-based identity** for fine-grained permissions

## Configuration Requirements
- **Secret Key**: Minimum 256-bit secret for token signing
- **Issuer**: Application identifier for token validation
- **Audience**: Target audience for tokens (typically users)
- **Token Expiry**: Configurable access token lifetime
- **Refresh Token Expiry**: Longer-lived refresh token duration

## Implementation Details
- JWT configuration lives in `appsettings.json` under `JwtSettings`
- ASP.NET Core Identity for user management and password handling
- Role definitions stored in Identity tables
- Token generation and validation services in Infrastructure layer
- Bearer token middleware configured in WebApi startup

## Security Considerations
- Secret key should be environment-specific and securely stored
- Use HTTPS only for token transmission
- Implement token refresh mechanism for better security
- Consider short access token lifetimes with refresh tokens
- Validate issuer and audience in all environments