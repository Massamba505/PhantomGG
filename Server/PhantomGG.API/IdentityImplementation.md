# ASP.NET Core Identity Implementation

## Required Packages

Run the following commands to add the necessary packages:

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

## Implementation Overview

This document outlines the steps to implement ASP.NET Core Identity with JWT authentication in the PhantomGG application.

### Main Components:

1. **ApplicationUser** - Extended Identity user class
2. **ApplicationDbContext** - Identity-enabled database context
3. **JWT Configuration** - Settings for token generation
4. **Identity Configuration** - User and role management setup
5. **Authentication Controllers** - Login, register, refresh token endpoints
6. **Token Services** - JWT token generation and validation

### User Roles

The application supports two user roles:

- **Organizer** - Can create and manage tournaments
- **User** - Regular application user

## Migration Instructions

After implementing all files, run the following commands to create and apply the necessary database migrations:

```bash
dotnet ef migrations add AddIdentity
dotnet ef database update
```
