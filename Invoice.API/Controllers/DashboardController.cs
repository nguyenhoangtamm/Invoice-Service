using System.Security.Claims;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class DashboardController(ILogger<DashboardController> logger, IDashboardService dashboardService, IInvoiceService invoiceService) : ApiControllerBase(logger)
{
    private readonly IDashboardService _dashboardService = dashboardService;
    private readonly IInvoiceService _invoiceService = invoiceService;

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


    [HttpPost("invoices/create")]
    public async Task<ActionResult<Result<int>>> CreateInvoice([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Dashboard: Creating invoice");

            
            var userIdClaim = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(request.IssuedByUserId?.ToString()) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
            {
                request = request with { IssuedByUserId = userId };
            }

            return await _invoiceService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice from dashboard", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the invoice"));
        }
    }

    [HttpPost("invoices/update/{id}")]
    public async Task<ActionResult<Result<int>>> UpdateInvoice([FromRoute] int id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Dashboard: Updating invoice with ID: {id}");

            return await _invoiceService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice with ID: {id} from dashboard", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the invoice"));
        }
    }

    [HttpPost("invoices/delete/{id}")]
    public async Task<ActionResult<Result<int>>> DeleteInvoice([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Dashboard: Deleting invoice with ID: {id}");

            return await _invoiceService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice with ID: {id} from dashboard", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the invoice"));
        }
    }

    [HttpGet("invoices/get-by-id/{id}")]
    public async Task<ActionResult<Result<InvoiceResponse>>> GetInvoiceById(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Dashboard: Getting invoice with ID: {id}");

            var result = await _invoiceService.GetById(id, cancellationToken);
            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice with ID: {id} from dashboard", ex);
            return StatusCode(500, Result<InvoiceResponse>.Failure("An error occurred while retrieving the invoice"));
        }
    }

    [HttpGet("invoices/get-all")]
    public async Task<ActionResult<Result<List<InvoiceResponse>>>> GetAllInvoices(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Dashboard: Getting all invoices");

            return await _invoiceService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all invoices from dashboard", ex);
            return StatusCode(500, Result<List<InvoiceResponse>>.Failure("An error occurred while retrieving invoices"));
        }
    }

    [HttpGet("invoices/get-pagination")]
    public async Task<ActionResult<PaginatedResult<InvoiceResponse>>> GetInvoicesWithPagination([FromQuery] GetInvoiceWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Dashboard: Getting invoices with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _invoiceService.GetWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices with pagination from dashboard", ex);
            return StatusCode(500, Result<PaginatedResult<InvoiceResponse>>.Failure("An error occurred while retrieving invoices"));
        }
    }
}