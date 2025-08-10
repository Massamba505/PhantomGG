using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Models;

namespace PhantomGG.API.Data;

/// <summary>
/// Initializes the database with seed data
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initializes the database with seed data
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply any pending migrations
        await context.Database.MigrateAsync();

        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed admin user
        await SeedAdminUserAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Add roles if they don't exist
        string[] roles = { "Admin", "Organizer", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        // Check if admin user exists
        var adminEmail = "admin@phantomgg.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            // Create admin user
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                // Add to admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
