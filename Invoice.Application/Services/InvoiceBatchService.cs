using AutoMapper;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class InvoiceBatchService : BaseService, IInvoiceBatchService
{
    public InvoiceBatchService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceBatchService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<List<InvoiceBatchResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all invoice batches");
            var repo = _unitOfWork.Repository<InvoiceBatch>();
            var list = await repo.Entities.AsNoTracking().ToListAsync(cancellationToken);
            var dto = _mapper.Map<List<InvoiceBatchResponse>>(list);
            return Result<List<InvoiceBatchResponse>>.Success(dto, "Invoice batches retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice batches", ex);
            return Result<List<InvoiceBatchResponse>>.Failure("Failed to retrieve invoice batches");
        }
    }

    public async Task<Result<PaginatedResult<InvoiceBatchResponse>>> GetWithPagination(GetInvoiceBatchesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting invoice batches paged");
            var repo = _unitOfWork.Repository<InvoiceBatch>();
            var q = repo.Entities.AsNoTracking();

            var count = await q.CountAsync(cancellationToken);
            var items = await q.OrderBy(b => b.Id)
                               .Skip((query.PageNumber - 1) * query.PageSize)
                               .Take(query.PageSize)
                               .ToListAsync(cancellationToken);

            var dto = _mapper.Map<List<InvoiceBatchResponse>>(items);
            return Result<PaginatedResult<InvoiceBatchResponse>>.Success(new PaginatedResult<InvoiceBatchResponse>(true, dto, null, count, query.PageNumber, query.PageSize));
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice batches paged", ex);
            return Result<PaginatedResult<InvoiceBatchResponse>>.Failure("Failed to retrieve invoice batches");
        }
    }

    public async Task<Result<InvoiceBatchResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice batch by id: {id}");
            var repo = _unitOfWork.Repository<InvoiceBatch>();
            var item = await repo.Entities.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
            if (item == null) return Result<InvoiceBatchResponse>.Failure("InvoiceBatch not found");
            var dto = _mapper.Map<InvoiceBatchResponse>(item);
            return Result<InvoiceBatchResponse>.Success(dto, "InvoiceBatch retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice batch by id: {id}", ex);
            return Result<InvoiceBatchResponse>.Failure("Failed to retrieve invoice batch");
        }
    }

    public async Task<Result<int>> Create(CreateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Creating invoice batch");
            var repo = _unitOfWork.Repository<InvoiceBatch>();
            var entity = new InvoiceBatch
            {
                BatchId = request.BatchId,
                Count = request.Count,
                MerkleRoot = request.MerkleRoot,
                BatchCid = request.BatchCid,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await repo.AddAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "InvoiceBatch created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice batch", ex);
            return Result<int>.Failure("Failed to create invoice batch");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice batch id: {id}");
            var repo = _unitOfWork.Repository<InvoiceBatch>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("InvoiceBatch not found");

            if (request.BatchId != null) entity.BatchId = request.BatchId;
            if (request.Count.HasValue) entity.Count = request.Count.Value;
            if (request.MerkleRoot != null) entity.MerkleRoot = request.MerkleRoot;
            if (request.BatchCid != null) entity.BatchCid = request.BatchCid;

            entity.UpdatedBy = UserName;
            entity.UpdatedDate = DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "InvoiceBatch updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice batch id: {id}", ex);
            return Result<int>.Failure("Failed to update invoice batch");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice batch id: {id}");
            var repo = _unitOfWork.Repository<InvoiceBatch>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("InvoiceBatch not found");

            await repo.DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "InvoiceBatch deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice batch id: {id}", ex);
            return Result<int>.Failure("Failed to delete invoice batch");
        }
    }
}
