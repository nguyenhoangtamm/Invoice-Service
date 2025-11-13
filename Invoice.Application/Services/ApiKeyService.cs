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

namespace Invoice.Application.Services;

public class ApiKeyService : BaseService, IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyService(IHttpContextAccessor httpContextAccessor, ILogger<ApiKeyService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IApiKeyRepository apiKeyRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    private static string ComputeHash(string key)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(key);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash); // .NET 5+
    }

    public async Task<Result<int>> Create(CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating API key for org: {request.OrganizationId}");

            var hash = ComputeHash(request.Key);
            // Ensure uniqueness
            var exist = await _apiKeyRepository.GetByKeyHashAsync(hash);
            if (exist != null) return Result<int>.Failure("Api key already exists");

            var apiKey = new ApiKey
            {
                KeyHash = hash,
                Name = request.Name,
                Active = request.Active,
                OrganizationId = request.OrganizationId,
                CreatedBy = UserName ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            await _apiKeyRepository.AddAsync(apiKey);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(apiKey.Id, "Api key created successfully");
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
            LogInformation($"Updating api key ID: {id}");

            var apiKey = await _apiKeyRepository.GetByIdAsync(id);
            if (apiKey == null) return Result<int>.Failure("Api key not found");

            if (request.Name != null) apiKey.Name = request.Name;
            if (request.Active.HasValue) apiKey.Active = request.Active.Value;
            if (request.RevokedAt.HasValue) apiKey.RevokedAt = request.RevokedAt.Value;
            if (request.OrganizationId.HasValue) apiKey.OrganizationId = request.OrganizationId.Value;

            apiKey.UpdatedBy = UserName ?? "System";
            apiKey.UpdatedDate = DateTime.UtcNow;

            await _apiKeyRepository.UpdateAsync(apiKey);
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

            var apiKey = await _apiKeyRepository.GetByIdAsync(id);
            if (apiKey == null) return Result<int>.Failure("Api key not found");

            await _apiKeyRepository.DeleteAsync(apiKey);
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
            var apiKey = await _apiKeyRepository.GetByIdAsync(id);
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
            var keys = await _apiKeyRepository.GetAllAsync();
            var response = _mapper.Map<List<ApiKeyResponse>>(keys);
            return Result<List<ApiKeyResponse>>.Success(response, "Api keys retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting api keys", ex);
            return Result<List<ApiKeyResponse>>.Failure("Failed to get api keys");
        }
    }
}
