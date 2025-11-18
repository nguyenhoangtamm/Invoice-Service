using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IApiKeyService
{
    Task<Result<CreateApiKeyResponse>> Create(CreateApiKeyRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateApiKeyRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<ApiKeyResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<ApiKeyResponse>>> GetAll(CancellationToken cancellationToken);
    Task<PaginatedResult<ApiKeyResponse>> GetWithPagination(GetApiKeyWithPagination request, CancellationToken cancellationToken);
    Task<Result<ApiKeyResponse>> ValidateApiKey(string apiKey, CancellationToken cancellationToken);
}
