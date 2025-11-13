using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationService organizationService) : ApiControllerBase(logger)
{
    private readonly IOrganizationService _organizationService = organizationService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating organization");

            var result = await _organizationService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating organization", ex);
            return StatusCode(500, "An error occurred while creating the organization");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating organization with ID: {id}");

            var result = await _organizationService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating organization with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the organization");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting organization with ID: {id}");

            var result = await _organizationService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting organization with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the organization");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting organization with ID: {id}");

            var result = await _organizationService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting organization with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the organization");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all organizations");

            var result = await _organizationService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all organizations", ex);
            return StatusCode(500, "An error occurred while retrieving organizations");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetOrganizationsWithPagination([FromQuery] GetOrganizationWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting organizations with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            var result = await _organizationService.GetWithPagination(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving organizations");
        }
    }
}
