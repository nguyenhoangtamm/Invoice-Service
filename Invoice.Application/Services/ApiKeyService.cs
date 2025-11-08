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

public class ApiKeyService : BaseService, IApiKeyService
{
    public ApiKeyService(IHttpContextAccessor httpContextAccessor, ILogger<ApiKeyService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<List<ApiKeyResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all API keys");
            var repo = _unitOfWork.Repository<ApiKey>();
            var list = await repo.Entities.AsNoTracking().ToListAsync(cancellationToken);
            var dto = _mapper.Map<List<ApiKeyResponse>>(list);
            return Result<List<ApiKeyResponse>>.Success(dto, "ApiKeys retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting api keys", ex);
            return Result<List<ApiKeyResponse>>.Failure("Failed to retrieve api keys");
        }
    }

    public async Task<Result<PaginatedResult<ApiKeyResponse>>> GetWithPagination(GetApiKeysQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting api keys paged");
            var repo = _unitOfWork.Repository<ApiKey>();
            var q = repo.Entities.AsNoTracking();

            if (query.OrganizationId.HasValue)
                q = q.Where(a => a.OrganizationId == query.OrganizationId.Value);

            var count = await q.CountAsync(cancellationToken);
            var items = await q.OrderBy(a => a.Id)
                               .Skip((query.PageNumber - 1) * query.PageSize)
                               .Take(query.PageSize)
                               .ToListAsync(cancellationToken);

            var dto = _mapper.Map<List<ApiKeyResponse>>(items);
            return Result<PaginatedResult<ApiKeyResponse>>.Success(new PaginatedResult<ApiKeyResponse>(true, dto, null, count, query.PageNumber, query.PageSize));
        }
        catch (Exception ex)
        {
            LogError("Error getting api keys paged", ex);
            return Result<PaginatedResult<ApiKeyResponse>>.Failure("Failed to retrieve api keys");
        }
    }

    public async Task<Result<ApiKeyResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting api key by id: {id}");
            var repo = _unitOfWork.Repository<ApiKey>();
            var item = await repo.Entities.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (item == null) return Result<ApiKeyResponse>.Failure("ApiKey not found");
            var dto = _mapper.Map<ApiKeyResponse>(item);
            return Result<ApiKeyResponse>.Success(dto, "ApiKey retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting api key by id: {id}", ex);
            return Result<ApiKeyResponse>.Failure("Failed to retrieve api key");
        }
    }

    public async Task<Result<int>> Create(CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Creating api key");
            var repo = _unitOfWork.Repository<ApiKey>();
            // For the key value itself, generate a random key and store its hash
            var keyValue = Guid.NewGuid().ToString("N");
            var keyHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyValue)); // simple placeholder; replace with proper hash

            var entity = new ApiKey
            {
                KeyHash = keyHash,
                Name = request.Name,
                OrganizationId = request.OrganizationId,
                Active = true,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await repo.AddAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            // Note: real API would return keyValue to the caller one-time; here we return Id
            return Result<int>.Success(entity.Id, "ApiKey created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating api key", ex);
            return Result<int>.Failure("Failed to create api key");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating api key id: {id}");
            var repo = _unitOfWork.Repository<ApiKey>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("ApiKey not found");

            if (request.Name != null) entity.Name = request.Name;
            if (request.Active.HasValue) entity.Active = request.Active.Value;

            entity.UpdatedBy = UserName;
            entity.UpdatedDate = DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "ApiKey updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating api key id: {id}", ex);
            return Result<int>.Failure("Failed to update api key");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting api key id: {id}");
            var repo = _unitOfWork.Repository<ApiKey>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("ApiKey not found");

            await repo.DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "ApiKey deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting api key id: {id}", ex);
            return Result<int>.Failure("Failed to delete api key");
        }
    }
}
