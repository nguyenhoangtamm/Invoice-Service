using Invoice.API.Attributes;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Invoice.API.Controllers;

[Authorize]
public class InvoiceReportsController(ILogger<InvoiceReportsController> logger, IInvoiceReportService reportService) : ApiControllerBase(logger)
{
    private readonly IInvoiceReportService _reportService = reportService;

    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateInvoiceReportRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating report for invoice ID: {request.InvoiceId}");

            return await _reportService.CreateAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice report", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the report"));
        }
    }

    [HttpPost("update/{id}")]
    [Authorize(Roles = "Admin,Administrator")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceReportRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating report with ID: {id}");

            return await _reportService.UpdateAsync(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating report with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the report"));
        }
    }

    [HttpPost("delete/{id}")]
    [Authorize(Roles = "Admin,Administrator")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting report with ID: {id}");

            return await _reportService.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting report with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the report"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    [Authorize(Roles = "Admin,Administrator")]
    public async Task<ActionResult<Result<InvoiceReportResponse>>> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting report with ID: {id}");

            var result = await _reportService.GetByIdAsync(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting report with ID: {id}", ex);
            return StatusCode(500, Result<InvoiceReportResponse>.Failure("An error occurred while retrieving the report"));
        }
    }

    [HttpGet("get-all")]
    [Authorize(Roles = "Admin,Administrator")]
    public async Task<ActionResult<Result<List<InvoiceReportResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all reports");

            return await _reportService.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all reports", ex);
            return StatusCode(500, Result<List<InvoiceReportResponse>>.Failure("An error occurred while retrieving reports"));
        }
    }

    [HttpGet("get-pagination")]
    [Authorize(Roles = "Admin,Administrator")]
    public async Task<ActionResult<PaginatedResult<InvoiceReportDetailResponse>>> GetWithPagination([FromQuery] GetInvoiceReportWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting reports with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _reportService.GetWithPaginationAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting reports with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<InvoiceReportDetailResponse>>.Failure("An error occurred while retrieving reports"));
        }
    }

    [HttpGet("get-by-user")]
    public async Task<ActionResult<PaginatedResult<InvoiceReportDetailResponse>>> GetByUserWithPagination([FromQuery] GetInvoiceReportByUserWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.UserId == 0)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(Result<PaginatedResult<InvoiceReportDetailResponse>>.Failure("User not authenticated"));
                }
                request = request with { UserId = userId };
            }

            LogInformation($"Getting reports by user {request.UserId} with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _reportService.GetByUserWithPaginationAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting reports by user {request.UserId} with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<InvoiceReportDetailResponse>>.Failure("An error occurred while retrieving reports"));
        }
    }
}
