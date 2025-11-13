using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

[Authorize]
public class InvoiceBatchesController(ILogger<InvoiceBatchesController> logger, IInvoiceBatchService batchService) : ApiControllerBase(logger)
{
    private readonly IInvoiceBatchService _batchService = batchService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice batch");

            var result = await _batchService.Create(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice batch", ex);
            return StatusCode(500, "An error occurred while creating the invoice batch");
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceBatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice batch with ID: {id}");

            var result = await _batchService.Update(id, request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice batch with ID: {id}", ex);
            return StatusCode(500, "An error occurred while updating the invoice batch");
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice batch with ID: {id}");

            var result = await _batchService.Delete(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice batch with ID: {id}", ex);
            return StatusCode(500, "An error occurred while deleting the invoice batch");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice batch with ID: {id}");

            var result = await _batchService.GetById(id, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice batch with ID: {id}", ex);
            return StatusCode(500, "An error occurred while retrieving the invoice batch");
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all invoice batches");

            var result = await _batchService.GetAll(cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting all invoice batches", ex);
            return StatusCode(500, "An error occurred while retrieving invoice batches");
        }
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetInvoiceBatchesWithPagination([FromQuery] GetInvoiceBatchWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting invoice batches with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            var result = await _batchService.GetWithPagination(request, cancellationToken);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice batches with pagination", ex);
            return StatusCode(500, "An error occurred while retrieving invoice batches");
        }
    }
}
