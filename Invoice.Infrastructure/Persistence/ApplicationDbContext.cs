using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Invoice.Domain.Entities.Base;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;

namespace Invoice.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // User Management - Users and Roles are already included from IdentityDbContext
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }

    // JWT Security
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TokenBlacklist> TokenBlacklists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations from this assembly
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


