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
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice");

            var result = await _invoiceService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return StatusCode(500, "An error occurred while creating the invoice");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice with ID: {id}");

            var result = await _invoiceService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the invoice");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice with ID: {id}");

            var result = await _invoiceService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the invoice");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
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
            return StatusCode(500, "An error occurred while retrieving the invoice");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all invoices");

            var result = await _invoiceService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all invoices", ex);
            return StatusCode(500, "An error occurred while retrieving invoices");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetInvoicesWithPagination([FromQuery] GetInvoiceWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting invoices with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            var result = await _invoiceService.GetWithPagination(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving invoices");
        }
    }

    [HttpGet("verify-invoice/{invoiceId}")]
    public async Task<IActionResult> VerifyInvoice(int invoiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Verifying invoice with ID: {invoiceId}");

            var result = await _invoiceService.VerifyInvoiceAsync(invoiceId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error verifying invoice with ID: {invoiceId}", ex);
            return StatusCode(500, "An error occurred while verifying the invoice");
        }
    }
}
