using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class RolesController(ILogger<RolesController> logger, IRoleService roleService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating role");

            return await roleService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating role", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the role"));
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating role with ID: {id}");

            return await roleService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating role with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the role"));
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting role with ID: {id}");

            return await roleService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting role with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the role"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Result<object>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting role with ID: {id}");

            var result = await roleService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting role with ID: {id}", ex);
            return StatusCode(500, Result<object>.Failure("An error occurred while retrieving the role"));
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<object>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all roles");

            return await roleService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all roles", ex);
            return StatusCode(500, Result<List<object>>.Failure("An error occurred while retrieving roles"));
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<object>> GetRolesWithPagination([FromQuery] GetRolesWithPaginationQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting roles with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await roleService.GetRolesWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting roles with pagination", ex);
            return StatusCode(500, Result<object>.Failure("An error occurred while retrieving roles"));
        }
    }
}

