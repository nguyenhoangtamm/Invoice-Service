using FluentValidation;
using Invoice.Application.Services;
using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Configurations;
using Invoice.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using System.Reflection;

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
        services.AddTransient<IOrganizationService, OrganizationService>();
        services.AddTransient<IInvoiceService, InvoiceService>();
        services.AddTransient<IInvoiceLineService, InvoiceLineService>();
        services.AddTransient<IInvoiceBatchService, InvoiceBatchService>();
        services.AddTransient<IApiKeyService, ApiKeyService>();
        services.AddTransient<IDashboardService, DashboardService>();
        services.AddTransient<IInvoiceFileService, InvoiceFileService>();

        // New services
        services.AddTransient<IOrganizationService, OrganizationService>();
        services.AddTransient<IApiKeyService, ApiKeyService>();
        services.AddTransient<IInvoiceBatchService, InvoiceBatchService>();
        services.AddTransient<IInvoiceService, InvoiceService>();
        services.AddTransient<IInvoiceLineService, InvoiceLineService>();

        // Register FluentValidation validators from this assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Additional configuration
        services.Configure<BlockchainConfiguration>(
           configuration.GetSection(BlockchainConfiguration.SectionName));

        // Register BlockchainConfiguration as singleton
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<BlockchainConfiguration>>().Value);

        // Register IWeb3
        services.AddSingleton<IWeb3>(sp =>
        {
            var blockchainConfig = sp.GetRequiredService<IOptions<BlockchainConfiguration>>().Value;
            return new Web3(blockchainConfig.RpcUrl);
        });

        return services;
    }
}


