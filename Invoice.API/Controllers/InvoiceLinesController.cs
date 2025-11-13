using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

public class InvoiceLinesController(ILogger<InvoiceLinesController> logger, IInvoiceLineService invoiceLineService) : ApiControllerBase(logger)
{
    private readonly IInvoiceLineService _invoiceLineService = invoiceLineService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceLineRequest request, CancellationToken cancellationToken)
    {
        var result = await _invoiceLineService.Create(request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceLineRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id) return BadRequest("ID mismatch");
        var result = await _invoiceLineService.Update(id, request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _invoiceLineService.Delete(id, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _invoiceLineService.GetById(id, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return NotFound(result);
    }

    [HttpGet("by-invoice/{invoiceId}")]
    public async Task<IActionResult> GetByInvoiceId([FromRoute] int invoiceId, CancellationToken cancellationToken)
    {
        var result = await _invoiceLineService.GetByInvoiceId(invoiceId, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }
}
