using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Invoice.Domain.Entities.Base;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Reflection;

namespace Invoice.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // User Management - Users and Roles are already included from IdentityDbContext
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }

    // JWT Security
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TokenBlacklist> TokenBlacklists { get; set; }

    // New domain entities (use fully-qualified names for types that might conflict with namespaces)
    public DbSet<Invoice.Domain.Entities.Organization> Organizations { get; set; }
    public DbSet<Invoice.Domain.Entities.ApiKey> ApiKeys { get; set; }
    public DbSet<Invoice.Domain.Entities.InvoiceBatch> InvoiceBatches { get; set; }
    public DbSet<Invoice.Domain.Entities.Invoice> Invoices { get; set; }
    public DbSet<Invoice.Domain.Entities.InvoiceLine> InvoiceLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply global query filter for soft-delete (IAuditableEntity)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null)
                continue;

            if (typeof(IAuditableEntity).IsAssignableFrom(clrType))
            {
                var method = typeof(ApplicationDbContext).GetMethod(nameof(ApplyIsDeletedQueryFilter), BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                {
                    var genericMethod = method.MakeGenericMethod(clrType);
                    genericMethod.Invoke(null, new object[] { modelBuilder });
                }
            }
        }
    }

    private static void ApplyIsDeletedQueryFilter<T>(ModelBuilder modelBuilder) where T : class, IAuditableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        // Try get current user id from HttpContext (JWT). Fall back to "System" when unavailable.
        string? userId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? _httpContextAccessor?.HttpContext?.User?.FindFirstValue("sub");

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
                entry.Entity.CreatedBy = userId ?? "System";
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDate = DateTime.UtcNow;
                entry.Entity.UpdatedBy = userId ?? "System";
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}


