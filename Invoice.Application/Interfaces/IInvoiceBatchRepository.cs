using Invoice.Domain.Entities;

namespace Invoice.Application.Interfaces;

public interface IInvoiceBatchRepository
{
    Task<InvoiceBatch?> GetByIdAsync(int id);
    Task<List<InvoiceBatch>> GetAllAsync();
    Task<InvoiceBatch> AddAsync(InvoiceBatch batch);
    Task UpdateAsync(InvoiceBatch batch);
    Task DeleteAsync(InvoiceBatch batch);
}
