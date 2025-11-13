using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class MenusController(ILogger<MenusController> logger, IMenuService menuService) : ApiControllerBase(logger)
{
    private readonly IMenuService _menuService = menuService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating menu");

            var result = await _menuService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating menu", ex);
            return StatusCode(500, "An error occurred while creating the menu");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating menu with ID: {id}");

            var result = await _menuService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating menu with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the menu");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting menu with ID: {id}");

            var result = await _menuService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting menu with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the menu");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting menu with ID: {id}");

            var result = await _menuService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting menu with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the menu");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all menus");

            var result = await _menuService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all menus", ex);
            return StatusCode(500, "An error occurred while retrieving menus");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetMenusWithPagination([FromQuery] GetMenuWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting menus with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            var result = await _menuService.GetWithPagination(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting menus with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving menus");
        }
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetMenuTree(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting menu tree");

            var result = await _menuService.GetMenuTree(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting menu tree", ex);
            return StatusCode(500, "An error occurred while retrieving menu tree");
        }
    }

    [HttpGet("tree/role/{roleId}")]
    public async Task<IActionResult> GetMenuTreeByRole(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting menu tree for role: {roleId}");

            var result = await _menuService.GetMenuTreeByRole(roleId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting menu tree for role: {roleId}", ex);
            return StatusCode(500, "An error occurred while retrieving menu tree for role");
        }
    }

    [HttpGet("GetMenusByUserRoles")]
    public async Task<IActionResult> GetMenusByUserRoles(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting menus by user roles");

            var result = await _menuService.GetMenusByUserRoles(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting menus by user roles", ex);
            return StatusCode(500, "An error occurred while retrieving user menus");
        }
    }

    [HttpPost("assign-to-role")]
    public async Task<IActionResult> AssignMenusToRole([FromBody] AssignMenuToRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Assigning menus to role");

            var result = await _menuService.AssignMenusToRole(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error assigning menus to role", ex);
            return StatusCode(500, "An error occurred while assigning menus to role");
        }
    }

    [HttpGet("role/{roleId}")]
    public async Task<IActionResult> GetRoleMenus(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting menus for role: {roleId}");

            var result = await _menuService.GetRoleMenus(roleId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting menus for role: {roleId}", ex);
            return StatusCode(500, "An error occurred while retrieving role menus");
        }
    }
}

