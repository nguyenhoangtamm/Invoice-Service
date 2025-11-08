using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class ApiKeysController : ApiControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeysController(ILogger<ApiKeysController> logger, IApiKeyService apiKeyService)
        : base(logger)
    {
        _apiKeyService = apiKeyService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<ApiKeyResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all api keys");
            var result = await _apiKeyService.GetAll(cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting api keys", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("paged")]
    public async Task<ActionResult<Result<PaginatedResult<ApiKeyResponse>>>> GetPaged([FromQuery] GetApiKeysQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting api keys paged");
            var result = await _apiKeyService.GetWithPagination(query, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting api keys paged", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ApiKeyResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting api key by id: {id}");
            var result = await _apiKeyService.GetById(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting api key by id: {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Creating api key");
            var result = await _apiKeyService.Create(request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating api key", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<int>>> Update(int id, [FromBody] UpdateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating api key id: {id}");
            var result = await _apiKeyService.Update(id, request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating api key id: {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<int>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting api key id: {id}");
            var result = await _apiKeyService.Delete(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting api key id: {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
