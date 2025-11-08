using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Invoice.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Determine environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Try startup project folder (API) first
        var basePath = Directory.GetCurrentDirectory();

        // If current directory is not the API project, try to locate Invoice.API
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            var possible = Path.Combine(basePath, "..", "Invoice.API");
            if (Directory.Exists(possible)) basePath = possible;
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        var connStr = config.GetConnectionString("DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("DefaultConnection")
                       ?? "Host=localhost;Port=5432;Database=Invoice;Username=postgres;Password=12345;";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connStr, npgsqlOptions => npgsqlOptions.MigrationsAssembly("Invoice.Infrastructure"));

        // IHttpContextAccessor is optional for design-time
        return new ApplicationDbContext(optionsBuilder.Options, new HttpContextAccessor());
    }
}
