using System.Security.Claims;
using AutoMapper;
using Invoice.Application.Interfaces;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class DashboardService : BaseService, IDashboardService
{
    private readonly IUserRepository _userRepository;
    public DashboardService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<DashboardService> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper, IUserRepository userRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _userRepository = userRepository;
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
                .AsNoTracking();

            var invoices = await invoicesQuery.ToListAsync(cancellationToken);

            // Calculate statistics
            var totalInvoices = invoices.Count;
            var totalRevenue = invoices.Sum(i => i.TotalAmount ?? 0);
            var users = await _userRepository.GetAllAsync();
            var userCount = users.Count;
            var avgInvoiceValue = totalInvoices > 0 ? totalRevenue / totalInvoices : 0;

            var totalOrganizations = await _unitOfWork.Repository<Domain.Entities.Organization>()
                .Entities
                .AsNoTracking()
                .CountAsync(cancellationToken);

            var pendingInvoices = invoices.Count(i => i.Status != Domain.Enums.InvoiceStatus.BlockchainConfirmed);

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
                TotalCustomers = userCount,
                PendingInvoices = pendingInvoices,
                TotalOrganizations = totalOrganizations,
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

    public async Task<Result<List<RevenueChartDataDto>>> GetRevenueChart(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting revenue chart data");

            // Get all invoices for the current user
            var invoicesQuery = _unitOfWork.Repository<Domain.Entities.Invoice>().Entities
                .AsNoTracking();

            var invoices = await invoicesQuery.ToListAsync(cancellationToken);

            // Group invoices by date and calculate revenue
            var revenueData = invoices
                .Where(i => i.IssuedDate.HasValue)
                .GroupBy(i => i.IssuedDate!.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new RevenueChartDataDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(i => i.TotalAmount ?? 0),
                    Period = g.Key.ToString("MMMM yyyy")
                })
                .ToList();

            LogInformation($"Revenue chart data retrieved successfully with {revenueData.Count} data points");
            return Result<List<RevenueChartDataDto>>.Success(revenueData, "Revenue chart data retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting revenue chart data", ex);
            return Result<List<RevenueChartDataDto>>.Failure("An error occurred while retrieving revenue chart data");
        }
    }

    public async Task<Result<List<TopCustomerDto>>> GetTopCustomers(int limit = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting top {limit} customers");

            // Get all invoices with customer information
            var invoicesQuery = _unitOfWork.Repository<Domain.Entities.Invoice>().Entities
                .AsNoTracking();

            var invoices = await invoicesQuery.ToListAsync(cancellationToken);

            // Group invoices by customer name and calculate totals
            var topCustomers = invoices
                .GroupBy(i => new { i.CustomerName, i.CustomerEmail })
                .Select(g => new TopCustomerDto
                {
                    Id = g.Key.CustomerEmail ?? Guid.NewGuid().ToString(),
                    Name = g.Key.CustomerName ?? "Unknown Customer",
                    TotalSpent = g.Sum(i => i.TotalAmount ?? 0),
                    InvoiceCount = g.Count(),
                    LastPurchase = g
                        .Where(i => i.IssuedDate.HasValue)
                        .OrderByDescending(i => i.IssuedDate)
                        .FirstOrDefault()?
                        .IssuedDate?
                        .ToString("yyyy-MM-dd")
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(limit)
                .ToList();

            LogInformation($"Top {limit} customers retrieved successfully with {topCustomers.Count} records");
            return Result<List<TopCustomerDto>>.Success(topCustomers, "Top customers retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting top customers", ex);
            return Result<List<TopCustomerDto>>.Failure("An error occurred while retrieving top customers");
        }
    }

    public async Task<Result<List<RecentActivityDto>>> GetRecentActivity(int limit = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting recent activity (limit: {limit})");

            var invoicesQuery = _unitOfWork.Repository<Domain.Entities.Invoice>().Entities
                .AsNoTracking();

            var invoices = await invoicesQuery.ToListAsync(cancellationToken);

            // Create a list of all activities
            var activities = new List<RecentActivityDto>();

            // Add invoice created activities
            foreach (var invoice in invoices.Where(i => i.CreatedDate.HasValue))
            {
                activities.Add(new RecentActivityDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "invoice_created",
                    Description = $"Invoice {invoice.InvoiceNumber} created for {invoice.CustomerName}",
                    Timestamp = invoice.CreatedDate!.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    UserId = invoice.CreatedBy?.ToString(),
                    EntityId = invoice.Id.ToString()
                });
            }

            // Add invoice updated activities
            foreach (var invoice in invoices.Where(i => i.UpdatedDate.HasValue && i.UpdatedDate != i.CreatedDate))
            {
                activities.Add(new RecentActivityDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "invoice_updated",
                    Description = $"Invoice {invoice.InvoiceNumber} updated",
                    Timestamp = invoice.UpdatedDate!.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    UserId = invoice.UpdatedBy?.ToString(),
                    EntityId = invoice.Id.ToString()
                });
            }

            // Get all users
            var users = await _userRepository.GetAllAsync();

            // Add customer added activities
            foreach (var user in users.Where(u => u.CreatedDate.HasValue))
            {
                activities.Add(new RecentActivityDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = "customer_added",
                    Description = $"Customer {user.FullName} added to the system",
                    Timestamp = user.CreatedDate!.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                    UserId = user.CreatedById?.ToString(),
                    EntityId = user.Id.ToString()
                });
            }

            // Sort by timestamp descending and take the limit
            var recentActivities = activities
                .OrderByDescending(a => DateTime.Parse(a.Timestamp))
                .Take(limit)
                .ToList();

            LogInformation($"Recent activity retrieved successfully with {recentActivities.Count} records");
            return Result<List<RecentActivityDto>>.Success(recentActivities, "Recent activity retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting recent activity", ex);
            return Result<List<RecentActivityDto>>.Failure("An error occurred while retrieving recent activity");
        }
    }
}