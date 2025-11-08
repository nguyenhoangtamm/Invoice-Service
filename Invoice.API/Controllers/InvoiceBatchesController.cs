using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class InvoiceBatchesController : ApiControllerBase
{
    private readonly IInvoiceBatchService _batchService;

    public InvoiceBatchesController(ILogger<InvoiceBatchesController> logger, IInvoiceBatchService batchService)
        : base(logger)
    {
        _batchService = batchService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<InvoiceBatchResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _batchService.GetAll(cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice batches", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("paged")]
    public async Task<ActionResult<Result<PaginatedResult<InvoiceBatchResponse>>>> GetPaged([FromQuery] GetInvoiceBatchesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _batchService.GetWithPagination(query, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice batches paged", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<InvoiceBatchResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _batchService.GetById(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice batch id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _batchService.Create(request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice batch", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<int>>> Update(int id, [FromBody] UpdateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _batchService.Update(id, request, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice batch id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<int>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _batchService.Delete(id, cancellationToken);
            if (result.Succeeded) return Ok(result);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice batch id {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }
}
