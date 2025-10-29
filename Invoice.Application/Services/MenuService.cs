using AutoMapper;
using FluentValidation;
using Invoice.Application.Services;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class MenuService : BaseService, IMenuService
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public MenuService(IHttpContextAccessor httpContextAccessor, ILogger<MenuService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, RoleManager<Role> roleManager,
        UserManager<User> userManager)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Result<List<MenuResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all menus");

            var menus = await _unitOfWork.Repository<Menu>()
                .Entities
                .Include(m => m.Children)
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);

            var response = _mapper.Map<List<MenuResponse>>(menus);
            return Result<List<MenuResponse>>.Success(response, "Menus retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error occurred while getting all menus", ex);
            return Result<List<MenuResponse>>.Failure("Failed to retrieve menus");
        }
    }

    public async Task<Result<MenuResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting menu by ID: {id}");

            var menu = await _unitOfWork.Repository<Menu>()
                .Entities
                .Include(m => m.Children)
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);

            if (menu == null)
            {
                return Result<MenuResponse>.Failure("Menu not found");
            }

            var response = _mapper.Map<MenuResponse>(menu);
            return Result<MenuResponse>.Success(response, "Menu retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while getting menu by ID: {id}", ex);
            return Result<MenuResponse>.Failure("Failed to retrieve menu");
        }
    }

    public async Task<Result<List<MenuTreeResponse>>> GetMenuTree(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting menu tree");

            var menus = await _unitOfWork.Repository<Menu>()
                .Entities
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);

            var menuTree = BuildMenuTree(menus, null);
            var response = _mapper.Map<List<MenuTreeResponse>>(menuTree);

            return Result<List<MenuTreeResponse>>.Success(response, "Menu tree retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error occurred while getting menu tree", ex);
            return Result<List<MenuTreeResponse>>.Failure("Failed to retrieve menu tree");
        }
    }

    public async Task<Result<List<MenuTreeResponse>>> GetMenuTreeByRole(int roleId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting menu tree by role ID: {roleId}");

            // Get role menus
            var roleMenus = await _unitOfWork.Repository<RoleMenu>()
                .Entities
                .Include(rm => rm.Menu)
                .Where(rm => rm.RoleId == roleId && !rm.IsDeleted && !rm.Menu.IsDeleted)
                .Select(rm => rm.Menu)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);

            // Get all menus for building tree structure
            var allMenus = await _unitOfWork.Repository<Menu>()
                .Entities
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);

            var menuTree = BuildMenuTreeWithPermissions(allMenus, roleMenus, null);

            return Result<List<MenuTreeResponse>>.Success(menuTree, "Role menu tree retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while getting menu tree by role ID: {roleId}", ex);
            return Result<List<MenuTreeResponse>>.Failure("Failed to retrieve role menu tree");
        }
    }

    public async Task<Result<List<UserMenuResponse>>> GetMenusByUserRoles(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting menus by user roles");

            // Get current user's username from context
            var userName = UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return Result<List<UserMenuResponse>>.Failure("User not authenticated");
            }

            // Get user by username using UserManager
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Result<List<UserMenuResponse>>.Failure("User not found");
            }

            // Get user's role menus
            var roleMenus = await _unitOfWork.Repository<RoleMenu>()
                .Entities
                .Include(rm => rm.Menu)
                .Where(rm => rm.RoleId == user.RoleId && !rm.IsDeleted && !rm.Menu.IsDeleted)
                .Select(rm => rm.Menu)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);

            var menuTree = BuildUserMenuTree(roleMenus, null);

            return Result<List<UserMenuResponse>>.Success(menuTree, "User menus retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error occurred while getting menus by user roles", ex);
            return Result<List<UserMenuResponse>>.Failure("Failed to retrieve user menus");
        }
    }

    public async Task<Result<int>> Create(CreateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating menu with name: {request.Name}");

            // Simple validation
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<int>.Failure("Menu name is required");
            }

            // Check for duplicate menu name
            var existingMenu = await _unitOfWork.Repository<Menu>().Entities
                .FirstOrDefaultAsync(m => m.Name == request.Name && !m.IsDeleted, cancellationToken);

            if (existingMenu != null)
            {
                return Result<int>.Failure("Menu with this name already exists");
            }

            var menu = _mapper.Map<Menu>(request);
            menu.CreatedBy = UserName;
            menu.CreatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Menu>().AddAsync(menu);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(menu.Id, "Menu created successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while creating menu: {request.Name}", ex);
            return Result<int>.Failure("Failed to create menu");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating menu with ID: {id}");

            var menu = await _unitOfWork.Repository<Menu>().GetByIdAsync(id);
            if (menu == null)
            {
                return Result<int>.Failure("Menu not found");
            }

            // Simple validation
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                // Check for duplicate menu name (excluding current menu)
                var existingMenu = await _unitOfWork.Repository<Menu>().Entities
                    .FirstOrDefaultAsync(m => m.Name == request.Name && m.Id != id && !m.IsDeleted, cancellationToken);

                if (existingMenu != null)
                {
                    return Result<int>.Failure("Menu with this name already exists");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Path))
            {
                // Check for duplicate menu path (excluding current menu)
                var existingPathMenu = await _unitOfWork.Repository<Menu>().Entities
                    .FirstOrDefaultAsync(m => m.Path == request.Path && m.Id != id && !m.IsDeleted, cancellationToken);

                if (existingPathMenu != null)
                {
                    return Result<int>.Failure("Menu with this path already exists");
                }
            }

            _mapper.Map(request, menu);
            menu.UpdatedBy = UserName;
            menu.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Menu>().UpdateAsync(menu);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(menu.Id, "Menu updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while updating menu with ID: {id}", ex);
            return Result<int>.Failure("Failed to update menu");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting menu with ID: {id}");

            var menu = await _unitOfWork.Repository<Menu>().GetByIdAsync(id);
            if (menu == null)
            {
                return Result<int>.Failure("Menu not found");
            }

            // Check if menu has children
            var hasChildren = await _unitOfWork.Repository<Menu>()
                .Entities
                .AnyAsync(m => m.ParentId == id && !m.IsDeleted, cancellationToken);

            if (hasChildren)
            {
                return Result<int>.Failure("Cannot delete menu that has child menus");
            }

            // Soft delete
            menu.IsDeleted = true;
            menu.UpdatedBy = UserName;
            menu.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Menu>().UpdateAsync(menu);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "Menu deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while deleting menu with ID: {id}", ex);
            return Result<int>.Failure("Failed to delete menu");
        }
    }

    public async Task<Result<int>> AssignMenusToRole(AssignMenuToRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Assigning menus to role ID: {request.RoleId}");

            // Simple validation
            if (request.RoleId <= 0)
            {
                return Result<int>.Failure("Invalid role ID");
            }

            if (request.MenuIds == null || !request.MenuIds.Any())
            {
                return Result<int>.Failure("Menu IDs are required");
            }

            // Remove existing role menus
            var existingRoleMenus = await _unitOfWork.Repository<RoleMenu>()
                .Entities
                .Where(rm => rm.RoleId == request.RoleId && !rm.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var roleMenu in existingRoleMenus)
            {
                roleMenu.IsDeleted = true;
                roleMenu.UpdatedBy = UserName;
                roleMenu.UpdatedDate = DateTime.UtcNow;
                await _unitOfWork.Repository<RoleMenu>().UpdateAsync(roleMenu);
            }

            // Add new role menus
            foreach (var menuId in request.MenuIds)
            {
                var roleMenu = new RoleMenu
                {
                    RoleId = request.RoleId,
                    MenuId = menuId,
                    CreatedBy = UserName,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.Repository<RoleMenu>().AddAsync(roleMenu);
            }

            await _unitOfWork.Save(cancellationToken);
            return Result<int>.Success(request.RoleId, "Menus assigned to role successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while assigning menus to role ID: {request.RoleId}", ex);
            return Result<int>.Failure("Failed to assign menus to role");
        }
    }

    public async Task<Result<List<RoleMenuResponse>>> GetRoleMenus(int roleId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting role menus for role ID: {roleId}");

            var roleMenus = await _unitOfWork.Repository<RoleMenu>()
                .Entities
                .Include(rm => rm.Role)
                .Include(rm => rm.Menu)
                .Where(rm => rm.RoleId == roleId && !rm.IsDeleted && !rm.Menu.IsDeleted)
                .ToListAsync(cancellationToken);

            var response = _mapper.Map<List<RoleMenuResponse>>(roleMenus);
            return Result<List<RoleMenuResponse>>.Success(response, "Role menus retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error occurred while getting role menus for role ID: {roleId}", ex);
            return Result<List<RoleMenuResponse>>.Failure("Failed to retrieve role menus");
        }
    }

    private List<Menu> BuildMenuTree(List<Menu> menus, int? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .Select(m => new Menu
            {
                Id = m.Id,
                Name = m.Name,
                Path = m.Path,
                Icon = m.Icon,
                Order = m.Order,
                ParentId = m.ParentId,
                CreatedDate = m.CreatedDate,
                UpdatedDate = m.UpdatedDate,
                Children = BuildMenuTree(menus, m.Id)
            })
            .OrderBy(m => m.Order)
            .ToList();
    }

    private List<MenuTreeResponse> BuildMenuTreeWithPermissions(List<Menu> allMenus, List<Menu> authorizedMenus, int? parentId)
    {
        return allMenus
            .Where(m => m.ParentId == parentId)
            .Select(m => new MenuTreeResponse
            {
                Id = m.Id,
                Name = m.Name,
                Path = m.Path,
                Icon = m.Icon,
                Order = m.Order,
                ParentId = m.ParentId,
                HasPermission = authorizedMenus.Any(am => am.Id == m.Id),
                Children = BuildMenuTreeWithPermissions(allMenus, authorizedMenus, m.Id)
            })
            .OrderBy(m => m.Order)
            .ToList();
    }

    private List<UserMenuResponse> BuildUserMenuTree(List<Menu> userMenus, int? parentId)
    {
        return userMenus
            .Where(m => m.ParentId == parentId)
            .Select(m => new UserMenuResponse
            {
                Id = m.Id,
                Name = m.Name,
                Icon = string.IsNullOrEmpty(m.Icon) ? null : m.Icon,
                Url = string.IsNullOrEmpty(m.Path) ? null : m.Path,
                IsBlank = false, // Default to false, can be customized based on requirements
                ParentId = m.ParentId,
                Order = m.Order,
                Children = BuildUserMenuTree(userMenus, m.Id)
            })
            .OrderBy(m => m.Order)
            .ToList();
    }

}

