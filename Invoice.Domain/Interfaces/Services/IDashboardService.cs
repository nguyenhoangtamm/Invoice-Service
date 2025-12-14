using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IDashboardService
{
    Task<Result<DashboardStatsResponse>> GetDashboardStats(CancellationToken cancellationToken);
    Task<Result<List<RevenueChartDataDto>>> GetRevenueChart(CancellationToken cancellationToken);
    Task<Result<List<TopCustomerDto>>> GetTopCustomers(int limit = 10, CancellationToken cancellationToken = default);
    Task<Result<List<RecentActivityDto>>> GetRecentActivity(int limit = 20, CancellationToken cancellationToken = default);
}