using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Invoice.Domain.Entities.Base;
using Invoice.Domain.Interfaces;
using DomainEntities = Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<DomainEntities.User, DomainEntities.Role, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // User Management - Users and Roles are provided by IdentityDbContext
    public DbSet<DomainEntities.Profile> Profiles { get; set; }
    public DbSet<DomainEntities.Menu> Menus { get; set; }
    public DbSet<DomainEntities.RoleMenu> RoleMenus { get; set; }

    // JWT Security
    public DbSet<DomainEntities.RefreshToken> RefreshTokens { get; set; }
    public DbSet<DomainEntities.TokenBlacklist> TokenBlacklists { get; set; }

    // Domain entities
    public DbSet<DomainEntities.Invoice> Invoices { get; set; }
    public DbSet<DomainEntities.InvoiceBatch> InvoiceBatches { get; set; }
    public DbSet<DomainEntities.InvoiceLine> InvoiceLines { get; set; }
    public DbSet<DomainEntities.Organization> Organizations { get; set; }
    public DbSet<DomainEntities.ApiKey> ApiKeys { get; set; }
    public DbSet<DomainEntities.InvoiceAttachment> InvoiceAttachments { get; set; }
    public DbSet<DomainEntities.InvoiceReport> InvoiceReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration implementations in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply Global Query Filters for soft delete
        ApplyGlobalQueryFilters(modelBuilder);
    }

    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        // Apply global query filter for all entities that implement IAuditableEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Check if the entity implements IAuditableEntity
            if (typeof(IAuditableEntity).IsAssignableFrom(clrType))
            {
                // Create a parameter expression for the entity
                var parameter = System.Linq.Expressions.Expression.Parameter(clrType, "e");

                // Create the property access expression for IsDeleted
                var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, nameof(IAuditableEntity.IsDeleted));

                // Create the comparison expression: IsDeleted == false
                var comparison = System.Linq.Expressions.Expression.Equal(propertyAccess, System.Linq.Expressions.Expression.Constant(false));

                // Create the lambda expression: e => e.IsDeleted == false
                var lambda = System.Linq.Expressions.Expression.Lambda(comparison, parameter);

                // Apply the global query filter
                modelBuilder.Entity(clrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Temporarily ignore global query filters for specific operations
    /// Call this method when you need to access soft-deleted records
    /// </summary>
    /// <returns>DbContext with query filters ignored</returns>
    public ApplicationDbContext IgnoreQueryFilters()
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        return this;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDate = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}


