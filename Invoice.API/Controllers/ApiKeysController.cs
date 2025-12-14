using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;


public class ApiKeysController(ILogger<ApiKeysController> logger, IApiKeyService apiKeyService) : ApiControllerBase(logger)
{
    private readonly IApiKeyService _apiKeyService = apiKeyService;

    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateApiKeyResponse>>> Create([FromBody] CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating API key with expiration: {request.ExpirationDays} days");

            return await _apiKeyService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating API key", ex);
            return StatusCode(500, Result<CreateApiKeyResponse>.Failure("An error occurred while creating the API key"));
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating API key with ID: {id}");

            return await _apiKeyService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating API key with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the API key"));
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting API key with ID: {id}");

            return await _apiKeyService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting API key with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the API key"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Result<ApiKeyResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting API key with ID: {id}");

            return await _apiKeyService.GetById(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting API key with ID: {id}", ex);
            return StatusCode(500, Result<ApiKeyResponse>.Failure("An error occurred while retrieving the API key"));
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<ApiKeyResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all API keys");

            return await _apiKeyService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all API keys", ex);
            return StatusCode(500, Result<List<ApiKeyResponse>>.Failure("An error occurred while retrieving API keys"));
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<PaginatedResult<ApiKeyResponse>>> GetApiKeysWithPagination([FromQuery] GetApiKeyWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting API keys with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _apiKeyService.GetWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting API keys with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<ApiKeyResponse>>.Failure("An error occurred while retrieving API keys"));
        }
    }
}
