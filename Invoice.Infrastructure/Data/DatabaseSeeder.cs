using Invoice.Domain.Entities;
using Invoice.Infrastructure.Persistence;
using Invoice.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Invoice.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<ApplicationDbContext>();
        var roleManager = provider.GetRequiredService<RoleManager<Role>>();
        var userManager = provider.GetRequiredService<UserManager<User>>();

        await RoleAndUserSeeder.SeedRolesAndUsersAsync(context, roleManager, userManager);
    }
}