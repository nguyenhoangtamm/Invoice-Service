using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Invoice.API.Controllers;

[Authorize]
public class OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationService organizationService) : ApiControllerBase(logger)
{
    private readonly IOrganizationService _organizationService = organizationService;

    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating organization");

            return await _organizationService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating organization", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the organization"));
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating organization with ID: {id}");

            return await _organizationService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating organization with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the organization"));
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting organization with ID: {id}");

            return await _organizationService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting organization with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the organization"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Result<OrganizationResponse>>> GetById(int id, CancellationToken cancellationToken)
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
            return StatusCode(500, Result<OrganizationResponse>.Failure("An error occurred while retrieving the organization"));
        }
    }

    [HttpGet("get-by-user/{userId}")]
    public async Task<ActionResult<Result<OrganizationResponse>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting organization for user ID: {userId}");

            var result = await _organizationService.GetByUserId(userId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting organization for user ID: {userId}", ex);
            return StatusCode(500, Result<OrganizationResponse>.Failure("An error occurred while retrieving the organization for user"));
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<Result<List<OrganizationResponse>>>> GetMyOrganization(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting organizations for current user");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(Result<List<OrganizationResponse>>.Failure("User not authenticated"));
            }

            var result = await _organizationService.GetOrganizationsByUserId(userId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations for current user", ex);
            return StatusCode(500, Result<List<OrganizationResponse>>.Failure("An error occurred while retrieving organizations for current user"));
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<OrganizationResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all organizations");

            return await _organizationService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all organizations", ex);
            return StatusCode(500, Result<List<OrganizationResponse>>.Failure("An error occurred while retrieving organizations"));
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<PaginatedResult<OrganizationResponse>>> GetOrganizationsWithPagination([FromQuery] GetOrganizationWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting organizations with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _organizationService.GetWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<OrganizationResponse>>.Failure("An error occurred while retrieving organizations"));
        }
    }
}
