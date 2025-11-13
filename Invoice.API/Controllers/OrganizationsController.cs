using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

public class OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationService organizationService) : ApiControllerBase(logger)
{
    private readonly IOrganizationService _organizationService = organizationService;

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var result = await _organizationService.Create(request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id) return BadRequest("ID mismatch");
        var result = await _organizationService.Update(id, request, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _organizationService.Delete(id, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _organizationService.GetById(id, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return NotFound(result);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await _organizationService.GetAll(cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("get-pagination")]
    public async Task<IActionResult> GetWithPagination([FromQuery] GetOrganizationsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _organizationService.GetWithPagination(query, cancellationToken);
        if (result.Succeeded) return Ok(result);
        return BadRequest(result);
    }
}
