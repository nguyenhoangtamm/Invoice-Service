using AutoMapper;
using Invoice.Application.Interfaces;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class InvoiceService : BaseService, IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceBatchRepository _batchRepository;

    public InvoiceService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IInvoiceRepository invoiceRepository, IInvoiceBatchRepository batchRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _invoiceRepository = invoiceRepository;
        _batchRepository = batchRepository;
    }

    public async Task<Result<int>> Create(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice: {request.InvoiceNumber}");

            var invoice = new Invoice.Domain.Entities.Invoice
            {
                InvoiceNumber = request.InvoiceNumber,
                FormNumber = request.FormNumber,
                Serial = request.Serial,
                OrganizationId = request.OrganizationId,
                IssuedByUserId = request.IssuedByUserId,
                SellerName = request.SellerName,
                SellerTaxId = request.SellerTaxId,
                SellerAddress = request.SellerAddress,
                SellerPhone = request.SellerPhone,
                SellerEmail = request.SellerEmail,
                CustomerName = request.CustomerName,
                CustomerTaxId = request.CustomerTaxId,
                CustomerAddress = request.CustomerAddress,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                Status = request.Status,
                IssuedDate = request.IssuedDate,
                SubTotal = request.SubTotal,
                TaxAmount = request.TaxAmount,
                DiscountAmount = request.DiscountAmount,
                TotalAmount = request.TotalAmount,
                Currency = request.Currency,
                Note = request.Note,
                BatchId = request.BatchId,
                ImmutableHash = request.ImmutableHash,
                Cid = request.Cid,
                CidHash = request.CidHash,
                MerkleProof = request.MerkleProof,
                CreatedBy = UserName ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            await _invoiceRepository.AddAsync(invoice);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(invoice.Id, "Invoice created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return Result<int>.Failure("Failed to create invoice");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice ID: {id}");

            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return Result<int>.Failure("Invoice not found");

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.InvoiceNumber)) invoice.InvoiceNumber = request.InvoiceNumber;
            if (!string.IsNullOrEmpty(request.FormNumber)) invoice.FormNumber = request.FormNumber;
            if (!string.IsNullOrEmpty(request.Serial)) invoice.Serial = request.Serial;
            if (request.OrganizationId.HasValue) invoice.OrganizationId = request.OrganizationId.Value;
            if (request.IssuedByUserId.HasValue) invoice.IssuedByUserId = request.IssuedByUserId;

            invoice.SellerName = request.SellerName ?? invoice.SellerName;
            invoice.SellerTaxId = request.SellerTaxId ?? invoice.SellerTaxId;
            invoice.SellerAddress = request.SellerAddress ?? invoice.SellerAddress;
            invoice.SellerPhone = request.SellerPhone ?? invoice.SellerPhone;
            invoice.SellerEmail = request.SellerEmail ?? invoice.SellerEmail;

            invoice.CustomerName = request.CustomerName ?? invoice.CustomerName;
            invoice.CustomerTaxId = request.CustomerTaxId ?? invoice.CustomerTaxId;
            invoice.CustomerAddress = request.CustomerAddress ?? invoice.CustomerAddress;
            invoice.CustomerPhone = request.CustomerPhone ?? invoice.CustomerPhone;
            invoice.CustomerEmail = request.CustomerEmail ?? invoice.CustomerEmail;

            if (request.Status.HasValue) invoice.Status = request.Status.Value;
            invoice.IssuedDate = request.IssuedDate ?? invoice.IssuedDate;

            invoice.SubTotal = request.SubTotal ?? invoice.SubTotal;
            invoice.TaxAmount = request.TaxAmount ?? invoice.TaxAmount;
            invoice.DiscountAmount = request.DiscountAmount ?? invoice.DiscountAmount;
            invoice.TotalAmount = request.TotalAmount ?? invoice.TotalAmount;
            invoice.Currency = request.Currency ?? invoice.Currency;
            invoice.Note = request.Note ?? invoice.Note;

            if (request.BatchId.HasValue) invoice.BatchId = request.BatchId;
            invoice.ImmutableHash = request.ImmutableHash ?? invoice.ImmutableHash;
            invoice.Cid = request.Cid ?? invoice.Cid;
            invoice.CidHash = request.CidHash ?? invoice.CidHash;
            invoice.MerkleProof = request.MerkleProof ?? invoice.MerkleProof;

            invoice.UpdatedBy = UserName;
            invoice.UpdatedDate = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(invoice.Id, "Invoice updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice ID: {id}", ex);
            return Result<int>.Failure("Failed to update invoice");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice ID: {id}");

            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return Result<int>.Failure("Invoice not found");

            invoice.IsDeleted = true;
            invoice.UpdatedBy = UserName;
            invoice.UpdatedDate = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(invoice.Id, "Invoice deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice ID: {id}", ex);
            return Result<int>.Failure("Failed to delete invoice");
        }
    }

    public async Task<Result<InvoiceResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null)
                return Result<InvoiceResponse>.Failure("Invoice not found");

            var dto = _mapper.Map<InvoiceResponse>(invoice);
            return Result<InvoiceResponse>.Success(dto, "Invoice retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice ID: {id}", ex);
            return Result<InvoiceResponse>.Failure("Failed to get invoice");
        }
    }

    public async Task<Result<List<InvoiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var list = await _invoiceRepository.GetAllAsync();
            var dto = _mapper.Map<List<InvoiceResponse>>(list);
            return Result<List<InvoiceResponse>>.Success(dto, "Invoices retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices", ex);
            return Result<List<InvoiceResponse>>.Failure("Failed to get invoices");
        }
    }

    public async Task<Result<int>> Upload(int invoiceId, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null) return Result<int>.Failure("Invoice not found");

            invoice.Status = InvoiceStatus.Uploaded;
            invoice.UpdatedBy = UserName ?? "System";
            invoice.UpdatedDate = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(invoice.Id, "Invoice marked as uploaded");
        }
        catch (Exception ex)
        {
            LogError("Error uploading invoice", ex);
            return Result<int>.Failure("Failed to mark uploaded");
        }
    }

    public async Task<Result<int>> StoreToIpfs(StoreToIpfsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
            if (invoice == null) return Result<int>.Failure("Invoice not found");

            invoice.ImmutableHash = request.ImmutableHash;
            invoice.Cid = request.Cid;
            invoice.CidHash = request.CidHash;
            invoice.MerkleProof = request.MerkleProof;

            // If Cid present -> IpfsStored else IpfsFailed
            invoice.Status = string.IsNullOrEmpty(request.Cid) ? InvoiceStatus.IpfsFailed : InvoiceStatus.IpfsStored;

            invoice.UpdatedBy = UserName ?? "System";
            invoice.UpdatedDate = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(invoice.Id, "Invoice IPFS store status updated");
        }
        catch (Exception ex)
        {
            LogError("Error storing invoice to IPFS", ex);
            return Result<int>.Failure("Failed to store to IPFS");
        }
    }

    public async Task<Result<int>> CreateBatch(CreateBatchFromInvoicesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // create batch with INITIAL status
            var batch = new InvoiceBatch
            {
                BatchId = string.IsNullOrWhiteSpace(request.ExternalBatchId) ? Guid.NewGuid().ToString() : request.ExternalBatchId,
                Count = 0,
                MerkleRoot = request.MerkleRoot,
                Status = BatchStatus.Initial,
                CreatedBy = UserName ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            await _batchRepository.AddAsync(batch);
            await _unitOfWork.Save(cancellationToken); // persist to get Id

            var assignedCount = 0;

            if (request.InvoiceIds != null)
            {
                foreach (var id in request.InvoiceIds)
                {
                    var inv = await _invoiceRepository.GetByIdAsync(id);
                    if (inv == null) continue;

                    // Only assign invoices that have been stored on IPFS
                    if (inv.Status != InvoiceStatus.IpfsStored) continue;

                    inv.BatchId = batch.Id;
                    inv.Status = InvoiceStatus.Batched;
                    inv.UpdatedBy = UserName ?? "System";
                    inv.UpdatedDate = DateTime.UtcNow;
                    await _invoiceRepository.UpdateAsync(inv);
                    assignedCount++;
                }
            }

            if (assignedCount == 0)
            {
                // no eligible invoices, remove created batch
                await _batchRepository.DeleteAsync(batch);
                await _unitOfWork.Save(cancellationToken);
                return Result<int>.Failure("No eligible invoices (IPFS_STORED) provided for batching");
            }

            batch.Count = assignedCount;
            batch.UpdatedBy = UserName ?? "System";
            batch.UpdatedDate = DateTime.UtcNow;

            await _batchRepository.UpdateAsync(batch);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(batch.Id, "Batch created and invoices assigned");
        }
        catch (Exception ex)
        {
            LogError("Error creating batch", ex);
            return Result<int>.Failure("Failed to create batch");
        }
    }

    public async Task<Result<int>> ConfirmBlockchain(ConfirmBlockchainRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var batch = await _batchRepository.GetByIdAsync(request.BatchId);
            if (batch == null) return Result<int>.Failure("Batch not found");

            if (request.Success)
            {
                batch.Status = BatchStatus.BlockchainConfirmed;
                batch.TxHash = request.TxHash;
                batch.BlockNumber = request.BlockNumber;
                batch.ConfirmedAt = DateTime.UtcNow;

                // mark invoices
                foreach (var inv in batch.Invoices)
                {
                    inv.Status = InvoiceStatus.BlockchainConfirmed;
                    inv.UpdatedBy = UserName ?? "System";
                    inv.UpdatedDate = DateTime.UtcNow;
                    await _invoiceRepository.UpdateAsync(inv);
                }
            }
            else
            {
                batch.Status = BatchStatus.BlockchainFailed;
                // mark invoices failed
                foreach (var inv in batch.Invoices)
                {
                    inv.Status = InvoiceStatus.BlockchainFailed;
                    inv.UpdatedBy = UserName ?? "System";
                    inv.UpdatedDate = DateTime.UtcNow;
                    await _invoiceRepository.UpdateAsync(inv);
                }
            }

            batch.UpdatedBy = UserName ?? "System";
            batch.UpdatedDate = DateTime.UtcNow;

            await _batchRepository.UpdateAsync(batch);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(batch.Id, "Batch blockchain confirmation processed");
        }
        catch (Exception ex)
        {
            LogError("Error confirming blockchain", ex);
            return Result<int>.Failure("Failed to confirm blockchain");
        }
    }

    public async Task<Result<int>> Finalize(int batchId, CancellationToken cancellationToken)
    {
        try
        {
            var batch = await _batchRepository.GetByIdAsync(batchId);
            if (batch == null) return Result<int>.Failure("Batch not found");

            foreach (var inv in batch.Invoices)
            {
                inv.Status = InvoiceStatus.Finalized;
                inv.UpdatedBy = UserName ?? "System";
                inv.UpdatedDate = DateTime.UtcNow;
                await _invoiceRepository.UpdateAsync(inv);
            }

            batch.UpdatedBy = UserName ?? "System";
            batch.UpdatedDate = DateTime.UtcNow;

            await _batchRepository.UpdateAsync(batch);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(batch.Id, "Batch finalized and invoices finalized");
        }
        catch (Exception ex)
        {
            LogError("Error finalizing batch", ex);
            return Result<int>.Failure("Failed to finalize batch");
        }
    }
}
