using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrganizationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Organization?> GetByIdAsync(int id)
    {
        return await _dbContext.Organizations.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public async Task<List<Organization>> GetAllAsync()
    {
        return await _dbContext.Organizations.Where(o => !o.IsDeleted).ToListAsync();
    }

    public async Task<List<Organization>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Organizations.Where(o => o.UserId == userId && !o.IsDeleted).ToListAsync();
    }

    public async Task<Organization> AddAsync(Organization organization)
    {
        await _dbContext.Organizations.AddAsync(organization);
        return organization;
    }

    public Task UpdateAsync(Organization organization)
    {
        var exist = _dbContext.Organizations.Find(organization.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(organization);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Organization organization)
    {
        _dbContext.Organizations.Remove(organization);
        return Task.CompletedTask;
    }
}
