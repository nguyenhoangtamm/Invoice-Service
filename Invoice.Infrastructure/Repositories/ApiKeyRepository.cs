using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ApiKeyRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiKey?> GetByIdAsync(int id)
    {
        return await _dbContext.ApiKeys.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<List<ApiKey>> GetAllAsync()
    {
        return await _dbContext.ApiKeys.Where(a => !a.IsDeleted).ToListAsync();
    }

    public async Task<ApiKey> AddAsync(ApiKey apiKey)
    {
        await _dbContext.ApiKeys.AddAsync(apiKey);
        return apiKey;
    }

    public Task UpdateAsync(ApiKey apiKey)
    {
        var exist = _dbContext.ApiKeys.Find(apiKey.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(apiKey);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ApiKey apiKey)
    {
        _dbContext.ApiKeys.Remove(apiKey);
        return Task.CompletedTask;
    }

    public async Task<ApiKey?> GetByKeyHashAsync(string keyHash)
    {
        return await _dbContext.ApiKeys.FirstOrDefaultAsync(a => a.KeyHash == keyHash && !a.IsDeleted);
    }
}
