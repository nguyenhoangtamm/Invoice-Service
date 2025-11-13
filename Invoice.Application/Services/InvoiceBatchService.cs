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
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class InvoiceBatchService : BaseService, IInvoiceBatchService
{
    private readonly IInvoiceBatchRepository _batchRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceBatchService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceBatchService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IInvoiceBatchRepository batchRepository, IInvoiceRepository invoiceRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _batchRepository = batchRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<int>> Create(CreateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating batch: {request.BatchId}");

            // Always create batch in INITIAL state regardless of request.Status
            var batch = new InvoiceBatch
            {
                BatchId = string.IsNullOrWhiteSpace(request.BatchId) ? Guid.NewGuid().ToString() : request.BatchId,
                MerkleRoot = request.MerkleRoot,
                BatchCid = request.BatchCid,
                Status = BatchStatus.Initial,
                TxHash = request.TxHash,
                BlockNumber = request.BlockNumber,
                ConfirmedAt = request.ConfirmedAt,
                CreatedBy = UserName ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            // persist to get generated Id
            await _batchRepository.AddAsync(batch);
            await _unitOfWork.Save(cancellationToken);

            var assignedCount = 0;

            // Assign only invoices that are in IPFS_STORED state
            if (request.InvoiceIds != null)
            {
                foreach (var invoiceId in request.InvoiceIds)
                {
                    var inv = await _invoiceRepository.GetByIdAsync(invoiceId);
                    if (inv == null) continue;
                    if (inv.Status != InvoiceStatus.IpfsStored) continue; // only eligible invoices

                    inv.BatchId = batch.Id;
                    inv.Status = InvoiceStatus.Batched;
                    inv.UpdatedBy = UserName ?? "System";
                    inv.UpdatedDate = DateTime.UtcNow;
                    await _invoiceRepository.UpdateAsync(inv);
                    assignedCount++;
                }
            }

            // If no eligible invoices were assigned, roll back batch creation
            if (assignedCount == 0)
            {
                await _batchRepository.DeleteAsync(batch);
                await _unitOfWork.Save(cancellationToken);
                return Result<int>.Failure("No eligible invoices (IPFS_STORED) provided for batching");
            }

            // update batch count
            batch.Count = assignedCount;
            batch.UpdatedBy = UserName ?? "System";
            batch.UpdatedDate = DateTime.UtcNow;

            await _batchRepository.UpdateAsync(batch);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(batch.Id, "Batch created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating batch", ex);
            return Result<int>.Failure("Failed to create batch");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating batch ID: {id}");

            var batch = await _batchRepository.GetByIdAsync(id);
            if (batch == null) return Result<int>.Failure("Batch not found");

            if (!string.IsNullOrEmpty(request.BatchId)) batch.BatchId = request.BatchId!;
            if (request.Count.HasValue) batch.Count = request.Count.Value;
            if (request.MerkleRoot != null) batch.MerkleRoot = request.MerkleRoot;
            if (request.BatchCid != null) batch.BatchCid = request.BatchCid;
            if (request.Status.HasValue) batch.Status = request.Status.Value;
            if (request.TxHash != null) batch.TxHash = request.TxHash;
            if (request.BlockNumber.HasValue) batch.BlockNumber = request.BlockNumber.Value;
            if (request.ConfirmedAt.HasValue) batch.ConfirmedAt = request.ConfirmedAt.Value;

            batch.UpdatedBy = UserName ?? "System";
            batch.UpdatedDate = DateTime.UtcNow;

            await _batchRepository.UpdateAsync(batch);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(batch.Id, "Batch updated successfully");
        }
        catch (Exception ex)
        {
            LogError("Error updating batch", ex);
            return Result<int>.Failure("Failed to update batch");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting batch ID: {id}");

            var batch = await _batchRepository.GetByIdAsync(id);
            if (batch == null) return Result<int>.Failure("Batch not found");

            await _batchRepository.DeleteAsync(batch);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(batch.Id, "Batch deleted successfully");
        }
        catch (Exception ex)
        {
            LogError("Error deleting batch", ex);
            return Result<int>.Failure("Failed to delete batch");
        }
    }

    public async Task<Result<InvoiceBatchResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting batch by ID: {id}");

            var batch = await _batchRepository.GetByIdAsync(id);
            if (batch == null) return Result<InvoiceBatchResponse>.Failure("Batch not found");

            var response = _mapper.Map<InvoiceBatchResponse>(batch);
            return Result<InvoiceBatchResponse>.Success(response, "Batch retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting batch", ex);
            return Result<InvoiceBatchResponse>.Failure("Failed to get batch");
        }
    }

    public async Task<Result<List<InvoiceBatchResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all batches");

            var batches = await _batchRepository.GetAllAsync();
            var response = _mapper.Map<List<InvoiceBatchResponse>>(batches);
            return Result<List<InvoiceBatchResponse>>.Success(response, "Batches retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting batches", ex);
            return Result<List<InvoiceBatchResponse>>.Failure("Failed to get batches");
        }
    }
}
