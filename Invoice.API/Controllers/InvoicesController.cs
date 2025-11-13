using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            return await _invoiceService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the invoice"));
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
}
