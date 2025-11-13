using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IInvoiceLineService
{
    Task<Result<int>> Create(CreateInvoiceLineRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateInvoiceLineRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<InvoiceLineResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<InvoiceLineResponse>>> GetByInvoiceId(int invoiceId, CancellationToken cancellationToken);
}
