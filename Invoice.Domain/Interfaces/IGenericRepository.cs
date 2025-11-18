using Invoice.Domain.Entities.Base;
namespace Invoice.Domain.Interfaces;

public interface IGenericRepository<T> where T : class, IEntity
{
    IQueryable<T> Entities { get; }

    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    // Additional methods for handling soft delete with Global Query Filters
    Task<T?> GetByIdIncludeDeletedAsync(int id);
    Task<List<T>> GetAllIncludeDeletedAsync();
    Task<List<T>> GetDeletedAsync();
    Task HardDeleteAsync(T entity);
    Task<T?> RestoreAsync(int id);
}

