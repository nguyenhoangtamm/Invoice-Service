using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Services;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public class InvoiceFileService : IInvoiceFileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InvoiceFileService> _logger;
    private readonly string _uploadsRoot;

    public InvoiceFileService(IUnitOfWork unitOfWork, ILogger<InvoiceFileService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "invoices");
    }

    public async Task<Result<UploadInvoiceFileResponse>> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Uploading file: {file.FileName}");

            if (!Directory.Exists(_uploadsRoot))
                Directory.CreateDirectory(_uploadsRoot);

            const long maxFileSize = 10 * 1024 * 1024; // 10 MB
            if (file.Length > maxFileSize)
            {
                return Result<UploadInvoiceFileResponse>.Failure($"File {file.FileName} exceeds maximum allowed size of {maxFileSize} bytes");
            }

            // Generate random file name to avoid conflicts
            var sanitizedFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadsRoot, sanitizedFileName);

            // Save file to disk
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            // Create attachment record in database (without invoice ID initially)
            var attachment = new InvoiceAttachment
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                Path = Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath).Replace('\\', '/'),
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<InvoiceAttachment>().AddAsync(attachment);
            await _unitOfWork.Save(cancellationToken);

            _logger.LogInformation($"File uploaded successfully with ID: {attachment.Id}");

            return Result<UploadInvoiceFileResponse>.Success(
                new UploadInvoiceFileResponse
                {
                    FileId = attachment.Id,
                    FileName = attachment.FileName,
                    ContentType = attachment.ContentType,
                    Size = attachment.Size,
                    Path = attachment.Path
                },
                "File uploaded successfully"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {file.FileName}", ex);
            return Result<UploadInvoiceFileResponse>.Failure("Failed to upload file");
        }
    }

    public async Task<Result<int>> LinkFilesToInvoiceAsync(int invoiceId, List<int> fileIds, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Linking {fileIds.Count} files to invoice {invoiceId}");

            if (fileIds == null || !fileIds.Any())
                return Result<int>.Success(invoiceId, "No files to link");

            // Get all attachments by IDs
            var attachments = await _unitOfWork.Repository<InvoiceAttachment>()
                .Entities
                .Where(a => fileIds.Contains(a.Id) && !a.InvoiceId.HasValue)
                .ToListAsync(cancellationToken);

            if (attachments.Count != fileIds.Count)
            {
                return Result<int>.Failure("Some files not found or already linked to an invoice");
            }

            // Link files to invoice
            foreach (var attachment in attachments)
            {
                attachment.InvoiceId = invoiceId;
                attachment.UpdatedDate = DateTime.UtcNow;
                await _unitOfWork.Repository<InvoiceAttachment>().UpdateAsync(attachment);
            }

            await _unitOfWork.Save(cancellationToken);

            _logger.LogInformation($"Successfully linked {attachments.Count} files to invoice {invoiceId}");
            return Result<int>.Success(invoiceId, "Files linked to invoice successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error linking files to invoice {invoiceId}", ex);
            return Result<int>.Failure("Failed to link files to invoice");
        }
    }

    public async Task<Result<int>> DeleteFileAsync(int fileId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Deleting file: {fileId}");

            var attachment = await _unitOfWork.Repository<InvoiceAttachment>().GetByIdAsync(fileId);
            if (attachment == null)
                return Result<int>.Failure("File not found");

            // Only allow deletion of files not yet linked to an invoice
            if (attachment.InvoiceId.HasValue)
                return Result<int>.Failure("Cannot delete file already linked to an invoice");

            // Delete physical file
            if (!string.IsNullOrEmpty(attachment.Path))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), attachment.Path);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            // Delete database record
            await _unitOfWork.Repository<InvoiceAttachment>().DeleteAsync(attachment);
            await _unitOfWork.Save(cancellationToken);

            _logger.LogInformation($"File deleted successfully: {fileId}");
            return Result<int>.Success(fileId, "File deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting file: {fileId}", ex);
            return Result<int>.Failure("Failed to delete file");
        }
    }

    public async Task<(bool Success, string FilePath, string FileName, string ContentType)> GetFileAsync(int fileId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Getting file: {fileId}");

            var attachment = await _unitOfWork.Repository<InvoiceAttachment>().GetByIdAsync(fileId);
            if (attachment == null)
                return (false, string.Empty, string.Empty, string.Empty);

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), attachment.Path);
            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning($"File not found on disk: {fullPath}");
                return (false, string.Empty, string.Empty, string.Empty);
            }

            _logger.LogInformation($"File retrieved successfully: {fileId}");
            return (true, fullPath, attachment.FileName, attachment.ContentType);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting file: {fileId}", ex);
            return (false, string.Empty, string.Empty, string.Empty);
        }
    }
}
