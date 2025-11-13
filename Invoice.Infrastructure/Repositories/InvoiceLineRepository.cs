using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class InvoiceLineRepository : IInvoiceLineRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InvoiceLineRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InvoiceLine?> GetByIdAsync(int id)
    {
        return await _dbContext.InvoiceLines.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
    }

    public async Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceId)
    {
        return await _dbContext.InvoiceLines.Where(l => l.InvoiceId == invoiceId && !l.IsDeleted).ToListAsync();
    }

    public async Task<InvoiceLine> AddAsync(InvoiceLine line)
    {
        await _dbContext.InvoiceLines.AddAsync(line);
        return line;
    }

    public Task UpdateAsync(InvoiceLine line)
    {
        var exist = _dbContext.InvoiceLines.Find(line.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(line);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(InvoiceLine line)
    {
        _dbContext.InvoiceLines.Remove(line);
        return Task.CompletedTask;
    }
}
