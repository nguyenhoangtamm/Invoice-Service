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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all IEntityTypeConfiguration implementations in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
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


