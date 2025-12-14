using Invoice.Domain.Entities;

namespace Invoice.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice.Domain.Entities.Invoice?> GetByIdAsync(int id);
    Task<List<Invoice.Domain.Entities.Invoice>> GetAllAsync();
    Task<Invoice.Domain.Entities.Invoice> AddAsync(Invoice.Domain.Entities.Invoice invoice);
    Task UpdateAsync(Invoice.Domain.Entities.Invoice invoice);
    Task DeleteAsync(Invoice.Domain.Entities.Invoice invoice);
}
