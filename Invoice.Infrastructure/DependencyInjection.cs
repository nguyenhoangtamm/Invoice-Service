using Invoice.Application.Interfaces;
using Invoice.Domain.Common.Security;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Repositories;
using Invoice.Infrastructure.Persistence;
using Invoice.Infrastructure.Repositories;
using Invoice.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Invoice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JwtSettings from configuration
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Register infrastructure services
        services.AddScoped<IJwtService, JwtService>();

        // Register background services
        services.AddHostedService<TokenCleanupService>();

        // Register DbContext (PostgreSQL as default if connection string provided). Ensure you have a connection string named "DefaultConnection" in appsettings.
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        // Configure Identity
        services.AddIdentity<User, Role>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Register repositories and unit of work
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}


