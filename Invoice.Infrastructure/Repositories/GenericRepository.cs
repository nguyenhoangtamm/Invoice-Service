using Invoice.Domain.Entities.Base;
using Invoice.Domain.Interfaces;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseAuditableEntity
{
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<T> Entities => _dbContext.Set<T>();

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        T exist = _dbContext.Set<T>().Find(entity.Id)!;
        _dbContext.Entry(exist).CurrentValues.SetValues(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        // Soft-delete: mark the entity as deleted and update it
        entity.IsDeleted = true;
        _dbContext.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task<List<T>> GetAllAsync()
    {
        // Global Query Filter s? t? ??ng lo?i b? các b?n ghi có IsDeleted = true
        return await _dbContext
            .Set<T>()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        // Global Query Filter s? t? ??ng lo?i b? các b?n ghi có IsDeleted = true
        return await _dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Get entity by ID including soft-deleted records
    /// Use this method when you need to access soft-deleted records
    /// </summary>
    public async Task<T?> GetByIdIncludeDeletedAsync(int id)
    {
        return await _dbContext.Set<T>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Get all entities including soft-deleted records
    /// Use this method when you need to access soft-deleted records
    /// </summary>
    public async Task<List<T>> GetAllIncludeDeletedAsync()
    {
        return await _dbContext.Set<T>()
            .IgnoreQueryFilters()
            .ToListAsync();
    }

    /// <summary>
    /// Get only soft-deleted records
    /// </summary>
    public async Task<List<T>> GetDeletedAsync()
    {
        return await _dbContext.Set<T>()
            .IgnoreQueryFilters()
            .Where(x => x.IsDeleted)
            .ToListAsync();
    }

    /// <summary>
    /// Hard delete an entity (permanently remove from database)
    /// Use with caution - this cannot be undone
    /// </summary>
    public Task HardDeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Restore a soft-deleted entity
    /// </summary>
    public async Task<T?> RestoreAsync(int id)
    {
        var entity = await _dbContext.Set<T>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);

        if (entity != null)
        {
            entity.IsDeleted = false;
            await UpdateAsync(entity);
        }

        return entity;
    }
}

