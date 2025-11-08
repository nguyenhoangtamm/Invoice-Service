using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class OrganizationsController : ApiControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationService organizationService)
        : base(logger)
    {
        _organizationService = organizationService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<OrganizationResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all organizations");
            var result = await _organizationService.GetAll(cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("paged")]
    public async Task<ActionResult<Result<PaginatedResult<OrganizationResponse>>>> GetPaged([FromQuery] GetOrganizationsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting organizations paged");
            var result = await _organizationService.GetWithPagination(query, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations paged", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<OrganizationResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting organization by id: {id}");
            var result = await _organizationService.GetById(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting organization by id: {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Creating organization");
            var result = await _organizationService.Create(request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating organization", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<int>>> Update(int id, [FromBody] UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating organization id: {id}");
            var result = await _organizationService.Update(id, request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating organization id: {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<int>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting organization id: {id}");
            var result = await _organizationService.Delete(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting organization id: {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
