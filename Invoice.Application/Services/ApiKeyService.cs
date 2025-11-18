using AutoMapper;
using Invoice.Application.Interfaces;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Invoice.Application.Extensions;

namespace Invoice.Application.Services;

public class ApiKeyService : BaseService, IApiKeyService
{

    public ApiKeyService(IHttpContextAccessor httpContextAccessor, ILogger<ApiKeyService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    private static string GenerateApiKey()
    {
        // Generate a secure random API key
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32]; // 256 bits
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static string ComputeHash(string key)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(key);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash); // .NET 5+
    }

    public async Task<Result<CreateApiKeyResponse>> Create(CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating API key for org: {request.OrganizationId}");

            // Generate a new API key
            string apiKey;
            string hash;

            // Ensure uniqueness by regenerating if hash already exists
            do
            {
                apiKey = GenerateApiKey();
                hash = ComputeHash(apiKey);

                var exists = await _unitOfWork.Repository<ApiKey>().Entities
                    .AnyAsync(k => k.KeyHash == hash, cancellationToken);

                if (!exists) break;

            } while (true);

            // Calculate expiration time
            DateTime? expiresAt = null;
            if (request.ExpirationDays.HasValue && request.ExpirationDays.Value > 0)
            {
                expiresAt = DateTime.UtcNow.AddDays(request.ExpirationDays.Value);
            }

            var apiKeyEntity = new ApiKey
            {
                KeyHash = hash,
                Name = request.Name,
                Active = request.Active,
                OrganizationId = request.OrganizationId,
                ExpiresAt = expiresAt,
                CreatedBy = UserName ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<ApiKey>().AddAsync(apiKeyEntity);
            await _unitOfWork.Save(cancellationToken);

            var response = new CreateApiKeyResponse
            {
                Id = apiKeyEntity.Id,
                Name = apiKeyEntity.Name,
                ApiKey = apiKey, // Return the actual API key to the user
                Active = apiKeyEntity.Active,
                ExpiresAt = apiKeyEntity.ExpiresAt,
                OrganizationId = apiKeyEntity.OrganizationId,
                CreatedDate = apiKeyEntity.CreatedDate ?? DateTime.UtcNow
            };

            return Result<CreateApiKeyResponse>.Success(response, "Api key created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating api key", ex);
            return Result<CreateApiKeyResponse>.Failure("Failed to create api key");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating api key ID: {id}");

            var apiKey = await _unitOfWork.Repository<ApiKey>().Entities
                .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);
            if (apiKey == null) return Result<int>.Failure("Api key not found");

            if (request.Name != null) apiKey.Name = request.Name;
            if (request.Active.HasValue) apiKey.Active = request.Active.Value;
            if (request.RevokedAt.HasValue) apiKey.RevokedAt = request.RevokedAt.Value;
            if (request.OrganizationId.HasValue) apiKey.OrganizationId = request.OrganizationId.Value;

            apiKey.UpdatedBy = UserName ?? "System";
            apiKey.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<ApiKey>().UpdateAsync(apiKey);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(apiKey.Id, "Api key updated successfully");
        }
        catch (Exception ex)
        {
            LogError("Error updating api key", ex);
            return Result<int>.Failure("Failed to update api key");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting api key ID: {id}");

            var apiKey = await _unitOfWork.Repository<ApiKey>().GetByIdAsync(id);
            if (apiKey == null) return Result<int>.Failure("Api key not found");

            await _unitOfWork.Repository<ApiKey>().DeleteAsync(apiKey);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(apiKey.Id, "Api key deleted successfully");
        }
        catch (Exception ex)
        {
            LogError("Error deleting api key", ex);
            return Result<int>.Failure("Failed to delete api key");
        }
    }

    public async Task<Result<ApiKeyResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var apiKey = await _unitOfWork.Repository<ApiKey>().GetByIdAsync(id);
            if (apiKey == null) return Result<ApiKeyResponse>.Failure("Api key not found");

            var response = _mapper.Map<ApiKeyResponse>(apiKey);
            return Result<ApiKeyResponse>.Success(response, "Api key retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting api key", ex);
            return Result<ApiKeyResponse>.Failure("Failed to get api key");
        }
    }

    public async Task<Result<List<ApiKeyResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var keys = await _unitOfWork.Repository<ApiKey>().GetAllAsync();
            var response = _mapper.Map<List<ApiKeyResponse>>(keys);
            return Result<List<ApiKeyResponse>>.Success(response, "Api keys retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting api keys", ex);
            return Result<List<ApiKeyResponse>>.Failure("Failed to get api keys");
        }
    }
    public async Task<PaginatedResult<ApiKeyResponse>> GetWithPagination(GetApiKeyWithPagination query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting users with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var apiKeysQuery = _unitOfWork.Repository<ApiKey>().Entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                apiKeysQuery = apiKeysQuery.Where(k => k.Name.Contains(query.Keyword));
            }
            return await apiKeysQuery.OrderBy(x => x.CreatedDate)
                .ProjectTo<ApiKeyResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken)
                .ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            LogError("Error getting api keys with pagination", ex);
            throw new Exception("An error occurred while retrieving api key with pagination");
        }
    }

    public async Task<Result<ApiKeyResponse>> ValidateApiKey(string apiKey, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Validating API key");

            var hash = ComputeHash(apiKey);
            var apiKeyEntity = await _unitOfWork.Repository<ApiKey>().Entities
                .Include(k => k.Organization)
                .FirstOrDefaultAsync(k => k.KeyHash == hash && k.Active && k.RevokedAt == null, cancellationToken);

            if (apiKeyEntity == null)
            {
                return Result<ApiKeyResponse>.Failure("Invalid or inactive API key");
            }

            // Ki?m tra xem API key có h?t h?n hay không
            if (apiKeyEntity.ExpiresAt.HasValue && apiKeyEntity.ExpiresAt.Value < DateTime.UtcNow)
            {
                return Result<ApiKeyResponse>.Failure("API key has expired");
            }

            var response = _mapper.Map<ApiKeyResponse>(apiKeyEntity);
            return Result<ApiKeyResponse>.Success(response, "API key is valid");
        }
        catch (Exception ex)
        {
            LogError("Error validating API key", ex);
            return Result<ApiKeyResponse>.Failure("Failed to validate API key");
        }
    }
}
