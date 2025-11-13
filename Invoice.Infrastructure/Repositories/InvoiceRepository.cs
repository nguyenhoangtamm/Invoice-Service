using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InvoiceRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Invoice.Domain.Entities.Invoice?> GetByIdAsync(int id)
    {
        return await _dbContext.Invoices
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<List<Invoice.Domain.Entities.Invoice>> GetAllAsync()
    {
        return await _dbContext.Invoices
            .Include(i => i.Lines)
            .Where(i => !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<Invoice.Domain.Entities.Invoice> AddAsync(Invoice.Domain.Entities.Invoice invoice)
    {
        await _dbContext.Invoices.AddAsync(invoice);
        return invoice;
    }

    public Task UpdateAsync(Invoice.Domain.Entities.Invoice invoice)
    {
        var exist = _dbContext.Invoices.Find(invoice.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(invoice);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Invoice.Domain.Entities.Invoice invoice)
    {
        _dbContext.Invoices.Remove(invoice);
        return Task.CompletedTask;
    }
}
