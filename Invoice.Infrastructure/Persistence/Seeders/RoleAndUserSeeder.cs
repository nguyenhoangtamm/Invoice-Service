using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Persistence.Seeders;

public static class RoleAndUserSeeder
{
    public static async Task SeedRolesAndUsersAsync(
        ApplicationDbContext context,
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
    {
        try
        {
            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users
            await SeedUsersAsync(context, userManager);
        }
        catch (Exception ex)
        {
        }
    }

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        // Check if roles already exist
        var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
        var userRoleExists = await roleManager.RoleExistsAsync("User");

        if (!adminRoleExists)
        {
            var adminRole = new Role
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator role with full system access",
                CreatedDate = DateTime.UtcNow
            };

            var result = await roleManager.CreateAsync(adminRole);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create Admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!userRoleExists)
        {
            var userRole = new Role
            {
                Name = "User",
                NormalizedName = "USER",
                Description = "Regular user role with standard access",
                CreatedDate = DateTime.UtcNow
            };

            var result = await roleManager.CreateAsync(userRole);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create User role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }

    private static async Task SeedUsersAsync(
        ApplicationDbContext context,
        UserManager<User> userManager)
    {
        // Get role IDs
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

        if (adminRole == null || userRole == null)
        {
            throw new Exception("Roles not found. Please ensure roles are seeded first.");
        }

        // Seed Admin User
        var adminUserEmail = "admin@Invoice.com";
        var adminUserExists = await userManager.FindByEmailAsync(adminUserEmail);

        if (adminUserExists == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminUserEmail,
                EmailConfirmed = true,
                RoleId = adminRole.Id,
                Status = UserStatus.Active,
                FullName = "Administrator",
                Gender = "Other",
                Address = "System",
                Phone = "+84-0-0000000",
                Bio = "System Administrator Account",
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Add to Admin role
            await userManager.AddToRoleAsync(adminUser, "Admin");
            await context.SaveChangesAsync();
        }

        // Seed Regular User
        var regularUserEmail = "user@Invoice.com";
        var regularUserExists = await userManager.FindByEmailAsync(regularUserEmail);

        if (regularUserExists == null)
        {
            var regularUser = new User
            {
                UserName = "user",
                Email = regularUserEmail,
                EmailConfirmed = true,
                RoleId = userRole.Id,
                Status = UserStatus.Active,
                FullName = "Test User",
                Gender = "Male",
                BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Address = "123 Test Street, Test City",
                Phone = "+84-123456789",
                Bio = "This is a test user account",
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(regularUser, "User@123");
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create regular user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Add to User role
            await userManager.AddToRoleAsync(regularUser, "User");
            await context.SaveChangesAsync();
        }
    }
}


