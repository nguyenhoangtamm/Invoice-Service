using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class InvoiceBatchRepository : IInvoiceBatchRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InvoiceBatchRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InvoiceBatch?> GetByIdAsync(int id)
    {
        return await _dbContext.InvoiceBatches.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }

    public async Task<List<InvoiceBatch>> GetAllAsync()
    {
        return await _dbContext.InvoiceBatches.Where(b => !b.IsDeleted).ToListAsync();
    }

    public async Task<InvoiceBatch> AddAsync(InvoiceBatch batch)
    {
        await _dbContext.InvoiceBatches.AddAsync(batch);
        return batch;
    }

    public Task UpdateAsync(InvoiceBatch batch)
    {
        var exist = _dbContext.InvoiceBatches.Find(batch.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(batch);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(InvoiceBatch batch)
    {
        _dbContext.InvoiceBatches.Remove(batch);
        return Task.CompletedTask;
    }
}
