// Repository interface for InvoiceLine
using Invoice.Domain.Entities;

namespace Invoice.Application.Interfaces;

public interface IInvoiceLineRepository
{
    Task<InvoiceLine?> GetByIdAsync(int id);
    Task<List<InvoiceLine>> GetByInvoiceIdAsync(int invoiceId);
    Task<InvoiceLine> AddAsync(InvoiceLine line);
    Task UpdateAsync(InvoiceLine line);
    Task DeleteAsync(InvoiceLine line);
}
