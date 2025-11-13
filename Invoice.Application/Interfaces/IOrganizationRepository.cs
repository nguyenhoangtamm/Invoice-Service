using Invoice.Domain.Entities;

namespace Invoice.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(int id);
    Task<List<Organization>> GetAllAsync();
    Task<List<Organization>> GetByUserIdAsync(int userId);

    Task<Organization> AddAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Organization organization);
}
