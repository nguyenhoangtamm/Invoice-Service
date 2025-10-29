using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IUserService
{
    Task<Result<int>> Create(CreateUserRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<GetUserDto>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<GetAllUsersDto>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<GetUsersWithPaginationDto>>> GetUsersWithPagination(GetUsersWithPaginationQuery query, CancellationToken cancellationToken);
    Task<Result<GetUserDto>> GetMe(CancellationToken cancellationToken);
    Task<Result<DashboardStatsDto>> GetDashboardStats(CancellationToken cancellationToken);
}


