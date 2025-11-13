using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IInvoiceBatchService
{
    Task<Result<int>> Create(CreateInvoiceBatchRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateInvoiceBatchRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<InvoiceBatchResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<InvoiceBatchResponse>>> GetAll(CancellationToken cancellationToken);
}
