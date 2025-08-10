# Implementing ASP.NET Core Identity in PhantomGG

This document provides a step-by-step guide on how to implement ASP.NET Core Identity in the PhantomGG application.

## Table of Contents

1. [Required Packages](#required-packages)
2. [Implementation Steps](#implementation-steps)
3. [Model Changes](#model-changes)
4. [Database Context](#database-context)
5. [Authentication Services](#authentication-services)
6. [Controllers](#controllers)
7. [Program Configuration](#program-configuration)
8. [Clean Up](#clean-up)

## Required Packages

First, install the necessary packages:

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

The other required packages are already in your project:

- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools

## Implementation Steps

### 1. Create ApplicationUser Class

Replace the existing `User` model with `ApplicationUser` that extends `IdentityUser`:

```csharp
// Models/ApplicationUser.cs
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
```

### 2. Update the RefreshToken Model

Update the `RefreshToken` model to work with the new `ApplicationUser`:

```csharp
// Models/RefreshToken.cs
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
    public bool IsActive => RevokedAt == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
```

### 3. Create ApplicationDbContext

Create a new `ApplicationDbContext` that extends `IdentityDbContext`:

```csharp
// Data/ApplicationDbContext.cs
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<PlayerStat> PlayerStats { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure identity tables to use a custom schema
        modelBuilder.Entity<ApplicationUser>().ToTable("Users", "Identity");
        modelBuilder.Entity<IdentityRole>().ToTable("Roles", "Identity");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Identity");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Identity");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "Identity");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Identity");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Identity");

        // Configure relationships
        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tournament>()
            .HasOne(t => t.Organizer)
            .WithMany(u => u.Tournaments)
            .HasForeignKey(t => t.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 4. Update Token Service

Create a token service that works with ASP.NET Core Identity:

```csharp
// Services/Interfaces/ITokenService.cs
public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user);
    RefreshToken GenerateRefreshToken(ApplicationUser user, string? ipAddress = null);
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, string? ipAddress = null);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    Task<bool> RevokeTokenAsync(string token, string? ipAddress = null, string? reason = null, string? replacedByToken = null);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
}
```

Implement the token service:

```csharp
// Services/Implementations/TokenService.cs
public class TokenService : ITokenService
{
    private readonly JwtConfig _jwtConfig;
    private readonly ApplicationDbContext _context;

    // Implementation methods here...
}
```

### 5. Update the Auth Controller

Update the `AuthController` to use ASP.NET Core Identity:

```csharp
// Controllers/AuthController.cs
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;

    // Implementation methods here...
}
```

### 6. Update Program.cs

Update `Program.cs` to configure ASP.NET Core Identity and JWT authentication:

```csharp
// Program.cs
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        // Configure database
        ConfigureDatabase(builder.Services, builder.Configuration);

        // Configure Identity
        ConfigureIdentity(builder.Services);

        // Configure JWT authentication
        ConfigureJwt(builder.Services, builder.Configuration);

        // Configure application services
        ConfigureServices(builder.Services);

        // Configure middleware and more...
    }

    // Helper methods for configuration...
}
```

## Clean Up

1. Remove the old `User` model
2. Remove the old `PhantomGGContext` class
3. Remove the old authentication services that are no longer needed:
   - `IAuthService` and `AuthService`
   - `IUserService` and `UserService`
   - `JwtUtils`
   - `IPasswordService` and `PasswordService`
   - Any repositories related to the old authentication system

## Database Migration

After implementing all the changes, create a new migration and update the database:

```bash
dotnet ef migrations add AddIdentity
dotnet ef database update
```

## Important Notes

1. The `ApplicationUser` uses string IDs (GUIDs in string format) instead of the previous Guid type
2. The `RefreshToken` model now stores the actual token value instead of a hash
3. Role-based authorization is now handled by ASP.NET Core Identity
4. The authentication workflow remains similar but uses the Identity APIs
