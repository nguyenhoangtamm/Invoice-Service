using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IInvoiceService
{
    Task<Result<List<InvoiceResponse>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<InvoiceResponse>>> GetWithPagination(GetInvoicesQuery query, CancellationToken cancellationToken);
    Task<Result<InvoiceResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<int>> Create(CreateInvoiceRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateInvoiceRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<VerifyInvoiceResponse>> VerifyInvoiceAsync(int invoiceId, CancellationToken cancellationToken);
}
