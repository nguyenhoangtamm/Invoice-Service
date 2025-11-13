using Invoice.Domain.Entities;

namespace Invoice.Application.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByIdAsync(int id);
    Task<List<ApiKey>> GetAllAsync();
    Task<ApiKey> AddAsync(ApiKey apiKey);
    Task UpdateAsync(ApiKey apiKey);
    Task DeleteAsync(ApiKey apiKey);
    Task<ApiKey?> GetByKeyHashAsync(string keyHash);
}
