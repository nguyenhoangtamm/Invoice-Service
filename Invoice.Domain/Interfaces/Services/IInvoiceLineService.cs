using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IInvoiceLineService
{
    Task<Result<List<InvoiceLineResponse>>> GetAll(CancellationToken cancellationToken);
    Task<Result<InvoiceLineResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<int>> Create(int invoiceId, InvoiceLineRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, InvoiceLineRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
}
