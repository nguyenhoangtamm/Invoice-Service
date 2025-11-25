using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;

namespace Invoice.Domain.Interfaces.Services;

public interface IInvoiceFileService
{
    Task<Result<UploadInvoiceFileResponse>> UploadFileAsync(IFormFile file, CancellationToken cancellationToken);
    Task<Result<int>> LinkFilesToInvoiceAsync(int invoiceId, List<int> fileIds, CancellationToken cancellationToken);
    Task<Result<int>> DeleteFileAsync(int fileId, CancellationToken cancellationToken);
    Task<(bool Success, string FilePath, string FileName, string ContentType)> GetFileAsync(int fileId, CancellationToken cancellationToken);
}
