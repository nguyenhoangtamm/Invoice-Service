using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoiceBatchesController : ControllerBase
{
    private readonly IInvoiceBatchService _batchService;

    public InvoiceBatchesController(IInvoiceBatchService batchService)
    {
        _batchService = batchService;
    }

    // POST: api/v1/InvoiceBatches/create
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _batchService.Create(request, cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    // POST: api/v1/InvoiceBatches/update/{id}
    [HttpPost("update/{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        var result = await _batchService.Update(id, request, cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    // POST: api/v1/InvoiceBatches/delete/{id}
    [HttpPost("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _batchService.Delete(id, cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }

    // GET: api/v1/InvoiceBatches/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _batchService.GetById(id, cancellationToken);
        if (!result.Succeeded) return NotFound(result.Message);
        return Ok(result);
    }

    // GET: api/v1/InvoiceBatches/get-all
    [HttpGet("get-all")]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _batchService.GetAll(cancellationToken);
        if (!result.Succeeded) return BadRequest(result.Message);
        return Ok(result);
    }
}
