using System.Security.Claims;
using AutoMapper;
using Invoice.Application.Interfaces;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class DashboardService : BaseService, IDashboardService
{
    public DashboardService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<DashboardService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<DashboardStatsResponse>> GetDashboardStats(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting dashboard stats");

            // Get current user ID from JWT token
            var userIdClaim = HttpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                LogError("User ID not found in token or invalid format", null!);
                return Result<DashboardStatsResponse>.Failure("User not authenticated or invalid token");
            }

            // Get all invoices for the current user
            var invoicesQuery = _unitOfWork.Repository<Domain.Entities.Invoice>().Entities
                .AsNoTracking()
                .Where(i => i.IssuedByUserId == userId);

            var invoices = await invoicesQuery.ToListAsync(cancellationToken);

            // Calculate statistics
            var totalInvoices = invoices.Count;
            var totalRevenue = invoices.Sum(i => i.TotalAmount ?? 0);
            var uniqueCustomers = invoices.Where(i => !string.IsNullOrEmpty(i.CustomerName))
                                          .Select(i => i.CustomerName)
                                          .Distinct()
                                          .Count();
            var avgInvoiceValue = totalInvoices > 0 ? totalRevenue / totalInvoices : 0;

            // Calculate monthly growth (comparing current month with previous month)
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var previousMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var previousYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var currentMonthRevenue = invoices
                .Where(i => i.IssuedDate.HasValue && 
                           i.IssuedDate.Value.Month == currentMonth && 
                           i.IssuedDate.Value.Year == currentYear)
                .Sum(i => i.TotalAmount ?? 0);

            var previousMonthRevenue = invoices
                .Where(i => i.IssuedDate.HasValue && 
                           i.IssuedDate.Value.Month == previousMonth && 
                           i.IssuedDate.Value.Year == previousYear)
                .Sum(i => i.TotalAmount ?? 0);

            var monthlyGrowth = previousMonthRevenue != 0 
                ? ((currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue) * 100 
                : 0;

            var stats = new DashboardStatsResponse
            {
                Id = userId.ToString(),
                TotalInvoices = totalInvoices,
                TotalRevenue = totalRevenue,
                TotalCustomers = uniqueCustomers,
                AvgInvoiceValue = Math.Round(avgInvoiceValue, 2),
                MonthlyGrowth = Math.Round(monthlyGrowth, 2),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            LogInformation("Dashboard stats retrieved successfully");
            return Result<DashboardStatsResponse>.Success(stats, "Dashboard stats retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting dashboard stats", ex);
            return Result<DashboardStatsResponse>.Failure("An error occurred while retrieving dashboard stats");
        }
    }
}