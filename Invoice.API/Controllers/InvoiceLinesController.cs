using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class InvoiceLinesController : ApiControllerBase
{
    private readonly IInvoiceLineService _lineService;

    public InvoiceLinesController(ILogger<InvoiceLinesController> logger, IInvoiceLineService lineService)
        : base(logger)
    {
        _lineService = lineService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<InvoiceLineResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lineService.GetAll(cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice lines", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<InvoiceLineResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lineService.GetById(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice line id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("invoice/{invoiceId}")]
    public async Task<ActionResult<Result<int>>> Create(int invoiceId, [FromBody] InvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lineService.Create(invoiceId, request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice line", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<int>>> Update(int id, [FromBody] InvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lineService.Update(id, request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice line id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<int>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _lineService.Delete(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice line id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
