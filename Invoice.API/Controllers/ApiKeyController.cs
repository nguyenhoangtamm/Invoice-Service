using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeysController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    // POST: api/v1/ApiKeys/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        var result = await _apiKeyService.Create(request, cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    // POST: api/v1/ApiKeys/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateApiKeyRequest request, CancellationToken cancellationToken)
    {
        var result = await _apiKeyService.Update(id, request, cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    // POST: api/v1/ApiKeys/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _apiKeyService.Delete(id, cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    // GET: api/v1/ApiKeys/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _apiKeyService.GetById(id, cancellationToken);
        if (!result.Succeeded) return NotFound(result.Message);
        return Ok(result);
    }

    // GET: api/v1/ApiKeys/get-all
    [HttpGet("get-all")]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _apiKeyService.GetAll(cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }
}
