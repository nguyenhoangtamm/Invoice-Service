using System.Reflection;
using FluentValidation;
using Invoice.Application.Services;
using Invoice.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Invoice.Domain.Common.Mappings;

namespace Invoice.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AutoMapper - include Domain assembly so DTO mapping profiles are discovered
        services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(MappingProfile).Assembly);

        // Register HTTP Context Accessor
        services.AddHttpContextAccessor();

        // Register application services
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IMenuService, MenuService>();

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}


