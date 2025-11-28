using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.Domain.Interfaces.Services;

public interface IRoleService
{
    Task<Result<int>> Create(CreateRoleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateRoleRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<object>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<object>>> GetAll(CancellationToken cancellationToken);
    Task<PaginatedResult<GetRolesWithPaginationDto>> GetRolesWithPagination(GetRolesWithPaginationQuery query, CancellationToken cancellationToken);
}

