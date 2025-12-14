using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class DashboardController(ILogger<DashboardController> logger, IDashboardService dashboardService) : ApiControllerBase(logger)
{
    private readonly IDashboardService _dashboardService = dashboardService;

    [HttpGet("stats")]
    public async Task<ActionResult<Result<DashboardStatsResponse>>> GetDashboardStats(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting dashboard statistics");

            var result = await _dashboardService.GetDashboardStats(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard statistics", ex);
            return StatusCode(500, Result<DashboardStatsResponse>.Failure("An error occurred while retrieving dashboard statistics"));
        }
    }

    [HttpGet("revenue-chart")]
    public async Task<ActionResult<Result<List<RevenueChartDataDto>>>> GetRevenueChart(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting revenue chart data");

            var result = await _dashboardService.GetRevenueChart(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting revenue chart data", ex);
            return StatusCode(500, Result<List<RevenueChartDataDto>>.Failure("An error occurred while retrieving revenue chart data"));
        }
    }

    [HttpGet("top-customers")]
    public async Task<ActionResult<Result<List<TopCustomerDto>>>> GetTopCustomers([FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting top {limit} customers");

            if (limit < 1 || limit > 100)
            {
                return BadRequest(Result<List<TopCustomerDto>>.Failure("Limit must be between 1 and 100"));
            }

            var result = await _dashboardService.GetTopCustomers(limit, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting top customers", ex);
            return StatusCode(500, Result<List<TopCustomerDto>>.Failure("An error occurred while retrieving top customers"));
        }
    }

    [HttpGet("recent-activity")]
    public async Task<ActionResult<Result<List<RecentActivityDto>>>> GetRecentActivity([FromQuery] int limit = 5, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting recent activity (limit: {limit})");

            if (limit < 1 || limit > 100)
            {
                return BadRequest(Result<List<RecentActivityDto>>.Failure("Limit must be between 1 and 100"));
            }

            var result = await _dashboardService.GetRecentActivity(limit, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting recent activity", ex);
            return StatusCode(500, Result<List<RecentActivityDto>>.Failure("An error occurred while retrieving recent activity"));
        }
    }
}