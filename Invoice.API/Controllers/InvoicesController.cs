using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

public class InvoicesController(ILogger<InvoicesController> logger, IInvoiceService invoiceService) : ApiControllerBase(logger)
{
    private readonly IInvoiceService _invoiceService = invoiceService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.Create(request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id) return BadRequest("ID mismatch");
        var result = await _invoiceService.Update(id, request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.Delete(id, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GetById(id, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return NotFound(result);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await _invoiceService.GetAll(cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    // Workflow endpoints
    [HttpPost("upload/{id}")]
    public async Task<IActionResult> Upload(int id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.Upload(id, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("store-ipfs")]
    public async Task<IActionResult> StoreToIpfs([FromBody] StoreToIpfsRequest request, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.StoreToIpfs(request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("create-batch")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchFromInvoicesRequest request, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.CreateBatch(request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("confirm-blockchain")]
    public async Task<IActionResult> ConfirmBlockchain([FromBody] ConfirmBlockchainRequest request, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.ConfirmBlockchain(request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("finalize/{batchId}")]
    public async Task<IActionResult> Finalize(int batchId, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.Finalize(batchId, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }
}
