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
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating menu");

            return await _menuService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating menu", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the menu"));
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating menu with ID: {id}");

            return await _menuService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating menu with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the menu"));
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting menu with ID: {id}");

            return await _menuService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting menu with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the menu"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Result<MenuResponse>>> GetById(int id, CancellationToken cancellationToken)
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
            return StatusCode(500, Result<MenuResponse>.Failure("An error occurred while retrieving the menu"));
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<MenuResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all menus");

            return await _menuService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all menus", ex);
            return StatusCode(500, Result<List<MenuResponse>>.Failure("An error occurred while retrieving menus"));
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<PaginatedResult<MenuResponse>>> GetMenusWithPagination([FromQuery] GetMenuWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting menus with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _menuService.GetWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting menus with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<MenuResponse>>.Failure("An error occurred while retrieving menus"));
        }
    }

    [HttpGet("tree")]
    public async Task<ActionResult<Result<List<MenuTreeResponse>>>> GetMenuTree(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting menu tree");

            return await _menuService.GetMenuTree(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting menu tree", ex);
            return StatusCode(500, Result<List<MenuTreeResponse>>.Failure("An error occurred while retrieving menu tree"));
        }
    }

    [HttpGet("tree/role/{roleId}")]
    public async Task<ActionResult<Result<List<MenuTreeResponse>>>> GetMenuTreeByRole(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting menu tree for role: {roleId}");

            return await _menuService.GetMenuTreeByRole(roleId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting menu tree for role: {roleId}", ex);
            return StatusCode(500, Result<List<MenuTreeResponse>>.Failure("An error occurred while retrieving menu tree for role"));
        }
    }

    [HttpGet("GetMenusByUserRoles")]
    public async Task<ActionResult<Result<List<UserMenuResponse>>>> GetMenusByUserRoles(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting menus by user roles");

            return await _menuService.GetMenusByUserRoles(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting menus by user roles", ex);
            return StatusCode(500, Result<List<UserMenuResponse>>.Failure("An error occurred while retrieving user menus"));
        }
    }

    [HttpPost("assign-to-role")]
    public async Task<ActionResult<Result<int>>> AssignMenusToRole([FromBody] AssignMenuToRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Assigning menus to role");

            return await _menuService.AssignMenusToRole(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error assigning menus to role", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while assigning menus to role"));
        }
    }

    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<Result<List<RoleMenuResponse>>>> GetRoleMenus(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting menus for role: {roleId}");

            return await _menuService.GetRoleMenus(roleId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting menus for role: {roleId}", ex);
            return StatusCode(500, Result<List<RoleMenuResponse>>.Failure("An error occurred while retrieving role menus"));
        }
    }
}

