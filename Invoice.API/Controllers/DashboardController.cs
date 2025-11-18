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
}