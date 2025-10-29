using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class MenusController : ApiControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(ILogger<MenusController> logger, IMenuService menuService)
        : base(logger)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Get all menus
    /// </summary>
    /// <returns>List of menus</returns>
    [HttpGet]
    public async Task<ActionResult<Result<List<MenuResponse>>>> GetAll(CancellationToken cancellationToken)
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
            LogError("Error occurred while getting all menus", ex);
            return StatusCode(500, "Internal server error occurred while getting menus");
        }
    }

    /// <summary>
    /// Get menu by ID
    /// </summary>
    /// <param name="id">Menu ID</param>
    /// <returns>Menu details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<MenuResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting menu by ID: {id}");
            var result = await _menuService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while getting menu by ID: {id}", ex);
            return StatusCode(500, "Internal server error occurred while getting menu");
        }
    }

    /// <summary>
    /// Get menu tree
    /// </summary>
    /// <returns>Menu tree structure</returns>
    [HttpGet("tree")]
    public async Task<ActionResult<Result<List<MenuTreeResponse>>>> GetMenuTree(CancellationToken cancellationToken)
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
            LogError("Error occurred while getting menu tree", ex);
            return StatusCode(500, "Internal server error occurred while getting menu tree");
        }
    }

    /// <summary>
    /// Get menu tree by role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>Menu tree with permissions for the role</returns>
    [HttpGet("tree/role/{roleId}")]
    public async Task<ActionResult<Result<List<MenuTreeResponse>>>> GetMenuTreeByRole(int roleId, CancellationToken cancellationToken)
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
            LogError($"Error occurred while getting menu tree for role: {roleId}", ex);
            return StatusCode(500, "Internal server error occurred while getting menu tree");
        }
    }

    /// <summary>
    /// Get menu tree for current authenticated user
    /// </summary>
    /// <returns>Menu tree with permissions for the current user's role</returns>
    [HttpGet("GetMenusByUserRoles")]
    public async Task<ActionResult<Result<List<UserMenuResponse>>>> GetMenusByUserRoles(CancellationToken cancellationToken)
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
            LogError("Error occurred while getting menus by user roles", ex);
            return StatusCode(500, "Internal server error occurred while getting user menus");
        }
    }

    /// <summary>
    /// Create new menu
    /// </summary>
    /// <param name="request">Menu creation request</param>
    /// <returns>Created menu ID</returns>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating menu: {request.Name}");
            var result = await _menuService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while creating menu: {request.Name}", ex);
            return StatusCode(500, "Internal server error occurred while creating menu");
        }
    }

    /// <summary>
    /// Update existing menu
    /// </summary>
    /// <param name="id">Menu ID</param>
    /// <param name="request">Menu update request</param>
    /// <returns>Updated menu ID</returns>
    [HttpPost("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update(int id, [FromBody] UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating menu: {id}");
            var result = await _menuService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while updating menu: {id}", ex);
            return StatusCode(500, "Internal server error occurred while updating menu");
        }
    }

    /// <summary>
    /// Delete menu
    /// </summary>
    /// <param name="id">Menu ID</param>
    /// <returns>Success status</returns>
    [HttpPost("delete/{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting menu: {id}");
            var result = await _menuService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while deleting menu: {id}", ex);
            return StatusCode(500, "Internal server error occurred while deleting menu");
        }
    }    /// <summary>
         /// Assign menus to role
         /// </summary>
         /// <param name="request">Menu assignment request</param>
         /// <returns>Success status</returns>
    [HttpPost("assign-to-role")]
    public async Task<ActionResult<Result<int>>> AssignMenusToRole([FromBody] AssignMenuToRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Assigning menus to role: {request.RoleId}");
            var result = await _menuService.AssignMenusToRole(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while assigning menus to role: {request.RoleId}", ex);
            return StatusCode(500, "Internal server error occurred while assigning menus to role");
        }
    }

    /// <summary>
    /// Get role menus
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <returns>List of role menus</returns>
    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<Result<List<RoleMenuResponse>>>> GetRoleMenus(int roleId, CancellationToken cancellationToken)
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
            LogError($"Error occurred while getting menus for role: {roleId}", ex);
            return StatusCode(500, "Internal server error occurred while getting role menus");
        }
    }
}

