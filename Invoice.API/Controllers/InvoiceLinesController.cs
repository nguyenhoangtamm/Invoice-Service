using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class InvoiceLinesController(ILogger<InvoiceLinesController> logger, IInvoiceLineService invoiceLineService) : ApiControllerBase(logger)
{
    private readonly IInvoiceLineService _invoiceLineService = invoiceLineService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice line");

            var result = await _invoiceLineService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice line", ex);
            return StatusCode(500, "An error occurred while creating the invoice line");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice line with ID: {id}");

            var result = await _invoiceLineService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice line with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the invoice line");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice line with ID: {id}");

            var result = await _invoiceLineService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice line with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the invoice line");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice line with ID: {id}");

            var result = await _invoiceLineService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice line with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the invoice line");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all invoice lines");

            var result = await _invoiceLineService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all invoice lines", ex);
            return StatusCode(500, "An error occurred while retrieving invoice lines");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetInvoiceLinesWithPagination([FromQuery] GetInvoiceLineWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting invoice lines with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            var result = await _invoiceLineService.GetWithPagination(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice lines with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving invoice lines");
        }
    }

    [HttpGet("by-invoice/{invoiceId}")]
    public async Task<IActionResult> GetByInvoiceId([FromRoute] int invoiceId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice lines for invoice ID: {invoiceId}");

            var result = await _invoiceLineService.GetByInvoiceId(invoiceId, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice lines for invoice ID: {invoiceId}", ex);
            return StatusCode(500, "An error occurred while retrieving invoice lines");
        }
    }
}
