using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Models;

namespace PhantomGG.API.Data;

/// <summary>
/// Application database context using ASP.NET Core Identity
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext
    /// </summary>
    /// <param name="options">Database context options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet for refresh tokens
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    /// <summary>
    /// Configures the database model
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
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
    }
}
