using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class InvoicesController : ApiControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(ILogger<InvoicesController> logger, IInvoiceService invoiceService)
        : base(logger)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<InvoiceResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetAll(cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("paged")]
    public async Task<ActionResult<Result<PaginatedResult<InvoiceResponse>>>> GetPaged([FromQuery] GetInvoicesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetWithPagination(query, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices paged", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<InvoiceResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.GetById(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.Create(request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<int>>> Update(int id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.Update(id, request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<int>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _invoiceService.Delete(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
