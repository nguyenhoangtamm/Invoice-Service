using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IInvoiceService
{
    Task<Result<int>> Create(CreateInvoiceRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateInvoiceRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<InvoiceResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<InvoiceResponse>>> GetAll(CancellationToken cancellationToken);

    // Actions required
    Task<Result<int>> Upload(int invoiceId, CancellationToken cancellationToken); // mark Uploaded
    Task<Result<int>> StoreToIpfs(StoreToIpfsRequest request, CancellationToken cancellationToken); // set IpfsStored or IpfsFailed
    Task<Result<int>> CreateBatch(CreateBatchFromInvoicesRequest request, CancellationToken cancellationToken); // create batch and set Batched
    Task<Result<int>> ConfirmBlockchain(ConfirmBlockchainRequest request, CancellationToken cancellationToken); // mark BlockchainConfirmed or BlockchainFailed
    Task<Result<int>> Finalize(int batchId, CancellationToken cancellationToken); // mark Finalized for invoices in batch
}
