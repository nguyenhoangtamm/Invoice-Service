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
    public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating API key");

            var result = await _apiKeyService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating API key", ex);
            return StatusCode(500, "An error occurred while creating the API key");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating API key with ID: {id}");

            var result = await _apiKeyService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating API key with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the API key");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting API key with ID: {id}");

            var result = await _apiKeyService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting API key with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the API key");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting API key with ID: {id}");

            var result = await _apiKeyService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting API key with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the API key");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all API keys");

            var result = await _apiKeyService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all API keys", ex);
            return StatusCode(500, "An error occurred while retrieving API keys");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetApiKeysWithPagination([FromQuery] GetApiKeyWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting API keys with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            var result = await _apiKeyService.GetWithPagination(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting API keys with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving API keys");
        }
    }
}
