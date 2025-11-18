using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IDashboardService
{
    Task<Result<DashboardStatsResponse>> GetDashboardStats(CancellationToken cancellationToken);
}