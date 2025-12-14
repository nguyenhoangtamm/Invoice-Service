using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IInvoiceReportService
{
    Task<Result<int>> CreateAsync(CreateInvoiceReportRequest request, CancellationToken cancellationToken);
    Task<Result<int>> UpdateAsync(int id, UpdateInvoiceReportRequest request, CancellationToken cancellationToken);
    Task<Result<int>> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<Result<InvoiceReportResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<List<InvoiceReportResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<PaginatedResult<InvoiceReportDetailResponse>> GetWithPaginationAsync(GetInvoiceReportWithPagination query, CancellationToken cancellationToken);
    Task<PaginatedResult<InvoiceReportDetailResponse>> GetByUserWithPaginationAsync(GetInvoiceReportByUserWithPagination query, CancellationToken cancellationToken);
}
