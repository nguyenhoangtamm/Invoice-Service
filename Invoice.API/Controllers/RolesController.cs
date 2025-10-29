using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

public class RolesController(ILogger<RolesController> logger, IRoleService roleService) : ApiControllerBase(logger)
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating role with name: {request.Name}");

            var result = await roleService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating role", ex);
            return StatusCode(500, "An error occurred while creating the role");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("ID in route does not match ID in body");
            }

            LogInformation($"Updating role with ID: {request.Id}");

            var result = await roleService.Update(request.Id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating role with ID: {request.Id}", ex);
            return StatusCode(500, "An error occurred while updating the role");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting role with ID: {id}");

            var result = await roleService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting role with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the role");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
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
            return StatusCode(500, "An error occurred while retrieving the role");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting all roles");

            var result = await roleService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all roles", ex);
            return StatusCode(500, "An error occurred while retrieving roles");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetRolesWithPagination([FromQuery] GetRolesWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting roles with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var result = await roleService.GetRolesWithPagination(query, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting roles with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving roles");
        }
    }
}

