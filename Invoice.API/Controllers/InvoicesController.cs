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
public class InvoicesController(ILogger<InvoicesController> logger, IInvoiceService invoiceService) : ApiControllerBase(logger)
{
    private readonly IInvoiceService _invoiceService = invoiceService;

    [HttpPost("create")]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(Result<List<OrganizationResponse>>.Failure("User not authenticated"));
            }
            // Set the IssuedByUserId to the authenticated user's ID
            request = request with { IssuedByUserId = userId };

            return await _invoiceService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the invoice"));
        }
    }

    // New API endpoint for uploading invoices with API key authentication
    [HttpPost("upload")]
    [ApiKeyAuth]
    [AllowAnonymous] // Override the controller's Authorize attribute since we're using API key auth
    public async Task<ActionResult<Result<int>>> UploadInvoice([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Uploading invoice via API key authentication");

            // Get organization ID from API key validation result
            var organizationId = HttpContext.Items["OrganizationId"] as int?;
            if (!organizationId.HasValue)
            {
                return BadRequest(Result<int>.Failure("Invalid API key - organization not found"));
            }

            // Set the organization ID from the API key
            var uploadRequest = request with { OrganizationId = organizationId.Value };

            // Log API key usage
            LogInformation($"Invoice upload via API key for organization: {organizationId}");

            return await _invoiceService.Create(uploadRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error uploading invoice via API key", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while uploading the invoice"));
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice with ID: {id}");

            return await _invoiceService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the invoice"));
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice with ID: {id}");

            return await _invoiceService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the invoice"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Result<InvoiceResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice with ID: {id}");

            var result = await _invoiceService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice with ID: {id}", ex);
            return StatusCode(500, Result<InvoiceResponse>.Failure("An error occurred while retrieving the invoice"));
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<InvoiceResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all invoices");

            return await _invoiceService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all invoices", ex);
            return StatusCode(500, Result<List<InvoiceResponse>>.Failure("An error occurred while retrieving invoices"));
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<PaginatedResult<InvoiceResponse>>> GetInvoicesWithPagination([FromQuery] GetInvoiceWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting invoices with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _invoiceService.GetWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<InvoiceResponse>>.Failure("An error occurred while retrieving invoices"));
        }
    }

    [HttpGet("get-by-user")]
    public async Task<ActionResult<PaginatedResult<InvoiceResponse>>> GetInvoicesByUserWithPagination([FromQuery] GetInvoiceByUserWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting invoices by user {request.UserId} with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _invoiceService.GetByUserWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoices by user {request.UserId} with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<InvoiceResponse>>.Failure("An error occurred while retrieving invoices by user"));
        }
    }

    [HttpGet("verify-invoice/{invoiceId}")]
    public async Task<ActionResult<Result<VerifyInvoiceResponse>>> VerifyInvoice(int invoiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Verifying invoice with ID: {invoiceId}");

            return await _invoiceService.VerifyInvoiceAsync(invoiceId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error verifying invoice with ID: {invoiceId}", ex);
            return StatusCode(500, Result<VerifyInvoiceResponse>.Failure("An error occurred while verifying the invoice"));
        }
    }

    // Public lookup endpoint - no authentication required
    [AllowAnonymous]
    [HttpGet("lookup/{code}")]
    public async Task<ActionResult<Result<InvoiceResponse>>> Lookup(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Lookup invoice with code: {code}");

            var result = await _invoiceService.LookupByCode(code, cancellationToken);
            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error looking up invoice with code: {code}", ex);
            return StatusCode(500, Result<InvoiceResponse>.Failure("An error occurred while looking up the invoice"));
        }
    }
}
