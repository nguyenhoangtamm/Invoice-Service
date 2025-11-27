using Invoice.API.Attributes;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Invoice.API.Controllers;

[Authorize]
public class InvoicesController(ILogger<InvoicesController> logger, IInvoiceService invoiceService, IInvoiceFileService fileService) : ApiControllerBase(logger)
{
    private readonly IInvoiceService _invoiceService = invoiceService;
    private readonly IInvoiceFileService _fileService = fileService;

    // New API endpoint for uploading files separately
    [HttpPost("upload-file")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<Result<UploadInvoiceFileResponse>>> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Uploading invoice file");

            if (file == null || file.Length == 0)
            {
                return BadRequest(Result<UploadInvoiceFileResponse>.Failure("No file provided"));
            }

            return await _fileService.UploadFileAsync(file, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error uploading invoice file", ex);
            return StatusCode(500, Result<UploadInvoiceFileResponse>.Failure("An error occurred while uploading the file"));
        }
    }

    [HttpPost("create")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(Result<List<OrganizationResponse>>.Failure("User not authenticated"));
            }
            // Set the IssuedByUserId to the authenticated user's ID
            request = request with { IssuedByUserId = userId };

            return await _invoiceService.Create(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while creating the invoice"));
        }
    }

    // New API endpoint for uploading invoices with API key authentication
    [HttpPost("upload")]
    [ApiKeyAuth]
    [AllowAnonymous] // Override the controller's Authorize attribute since we're using API key auth
    [DisableRequestSizeLimit]
    public async Task<ActionResult<Result<int>>> UploadInvoice([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Uploading invoice via API key authentication");

            // Get organization ID from API key validation result
            var organizationId = HttpContext.Items["OrganizationId"] as int?;
            if (!organizationId.HasValue)
            {
                return BadRequest(Result<int>.Failure("Invalid API key - organization not found"));
            }

            // Set the organization ID from the API key
            var uploadRequest = request with { OrganizationId = organizationId.Value };

            // Log API key usage
            LogInformation($"Invoice upload via API key for organization: {organizationId}");

            return await _invoiceService.Create(uploadRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error uploading invoice via API key", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while uploading the invoice"));
        }
    }

    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult<Result<int>>> Update([FromRoute] int id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice with ID: {id}");

            return await _invoiceService.Update(id, request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while updating the invoice"));
        }
    }

    [HttpPost]
    [Route("delete/{id}")]
    public async Task<ActionResult<Result<int>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice with ID: {id}");

            return await _invoiceService.Delete(id, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice with ID: {id}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the invoice"));
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Result<InvoiceResponse>>> GetById(int id, CancellationToken cancellationToken)
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
            return StatusCode(500, Result<InvoiceResponse>.Failure("An error occurred while retrieving the invoice"));
        }
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<Result<List<InvoiceResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation("Getting all invoices");

            return await _invoiceService.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting all invoices", ex);
            return StatusCode(500, Result<List<InvoiceResponse>>.Failure("An error occurred while retrieving invoices"));
        }
    }

    [HttpGet("get-pagination")]
    public async Task<ActionResult<PaginatedResult<InvoiceResponse>>> GetInvoicesWithPagination([FromQuery] GetInvoiceWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Getting invoices with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _invoiceService.GetWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<InvoiceResponse>>.Failure("An error occurred while retrieving invoices"));
        }
    }

    [HttpGet("get-by-user")]
    public async Task<ActionResult<PaginatedResult<InvoiceResponse>>> GetInvoicesByUserWithPagination([FromQuery] GetInvoiceByUserWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.UserId == 0)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(Result<PaginatedResult<InvoiceResponse>>.Failure("User not authenticated"));
                }
                request = request with { UserId = userId };
            }
            LogInformation($"Getting invoices by user {request.UserId} with pagination - Page: {request.PageNumber}, Size: {request.PageSize}");

            return await _invoiceService.GetByUserWithPagination(request, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoices by user {request.UserId} with pagination", ex);
            return StatusCode(500, Result<PaginatedResult<InvoiceResponse>>.Failure("An error occurred while retrieving invoices by user"));
        }
    }
    [AllowAnonymous]
    [HttpGet("verify-invoice/{invoiceId}")]
    public async Task<ActionResult<Result<VerifyInvoiceResponse>>> VerifyInvoice(int invoiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Verifying invoice with ID: {invoiceId}");

            return await _invoiceService.VerifyInvoiceAsync(invoiceId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error verifying invoice with ID: {invoiceId}", ex);
            return StatusCode(500, Result<VerifyInvoiceResponse>.Failure("An error occurred while verifying the invoice"));
        }
    }

    // Public lookup endpoint - no authentication required
    [AllowAnonymous]
    [HttpGet("lookup")]
    public async Task<ActionResult<PaginatedResult<InvoiceLookUpResponse>>> Lookup([FromQuery] GetInvoiceLookUpWithPagination request, CancellationToken cancellationToken = default)
    {
        try
        {
            LogInformation($"Lookup invoice with code: {request.Code}");

            var result = await _invoiceService.LookupByCode(request, cancellationToken);
            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }
        catch (Exception ex)
        {
            LogError($"Error looking up invoice with code: {request.Code}", ex);
            return StatusCode(500, Result<InvoiceResponse>.Failure("An error occurred while looking up the invoice"));
        }
    }

    [HttpPost("sync-blockchain/{invoiceId}")]
    public async Task<ActionResult<Result<InvoiceResponse>>> SyncInvoiceFromBlockchain([FromRoute] int invoiceId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Syncing invoice with ID: {invoiceId} from blockchain");

            var result = await _invoiceService.SyncInvoiceFromBlockchainAsync(invoiceId, cancellationToken);
            
            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            LogError($"Error syncing invoice with ID: {invoiceId} from blockchain", ex);
            return StatusCode(500, Result<InvoiceResponse>.Failure("An error occurred while syncing invoice from blockchain"));
        }
    }

    // New API endpoint for linking uploaded files to invoice
    [HttpPost("link-files/{invoiceId}")]
    public async Task<ActionResult<Result<int>>> LinkFilesToInvoice([FromRoute] int invoiceId, [FromBody] List<int> fileIds, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Linking files to invoice with ID: {invoiceId}");

            return await _fileService.LinkFilesToInvoiceAsync(invoiceId, fileIds, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error linking files to invoice with ID: {invoiceId}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while linking files to invoice"));
        }
    }

    // New API endpoint for deleting uploaded files
    [HttpPost("delete-file/{fileId}")]
    public async Task<ActionResult<Result<int>>> DeleteFile([FromRoute] int fileId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting file with ID: {fileId}");

            return await _fileService.DeleteFileAsync(fileId, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error deleting file with ID: {fileId}", ex);
            return StatusCode(500, Result<int>.Failure("An error occurred while deleting the file"));
        }
    }

    // New API endpoint for downloading uploaded files
    [HttpGet("download-file/{fileId}")]
    public async Task<IActionResult> DownloadFile([FromRoute] int fileId, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Downloading file with ID: {fileId}");

            var (success, filePath, fileName, contentType) = await _fileService.GetFileAsync(fileId, cancellationToken);
            
            if (!success)
            {
                return NotFound(Result<string>.Failure("File not found"));
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            LogError($"Error downloading file with ID: {fileId}", ex);
            return StatusCode(500, Result<string>.Failure("An error occurred while downloading the file"));
        }
    }
}
