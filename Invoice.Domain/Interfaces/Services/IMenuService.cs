using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IMenuService
{
    Task<Result<List<MenuResponse>>> GetAll(CancellationToken cancellationToken);
    Task<Result<MenuResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<MenuTreeResponse>>> GetMenuTree(CancellationToken cancellationToken);
    Task<Result<List<MenuTreeResponse>>> GetMenuTreeByRole(int roleId, CancellationToken cancellationToken);
    Task<Result<List<UserMenuResponse>>> GetMenusByUserRoles(CancellationToken cancellationToken);
    Task<Result<int>> Create(CreateMenuRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateMenuRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<int>> AssignMenusToRole(AssignMenuToRoleRequest request, CancellationToken cancellationToken);
    Task<Result<List<RoleMenuResponse>>> GetRoleMenus(int roleId, CancellationToken cancellationToken);
}

