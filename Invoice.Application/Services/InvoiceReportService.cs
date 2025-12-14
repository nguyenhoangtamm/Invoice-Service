using AutoMapper;
using AutoMapper.QueryableExtensions;
using Invoice.Application.Extensions;
using Invoice.Application.Interfaces;
using Invoice.Application.Utilities;
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

public class InvoiceReportService : BaseService, IInvoiceReportService
{
    public InvoiceReportService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceReportService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> CreateAsync(CreateInvoiceReportRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating report for invoice ID: {request.InvoiceId}");

            // Verify invoice exists
            var invoiceExists = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>()
                .Entities
                .AnyAsync(i => i.Id == request.InvoiceId, cancellationToken);

            if (!invoiceExists)
                return Result<int>.Failure("Invoice not found");

            // Get the current user ID from claims
            var userIdClaim = HttpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                userIdClaim = HttpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out userId))
                    return Result<int>.Failure("User not authenticated");
            }

            var entity = new InvoiceReport
            {
                InvoiceId = request.InvoiceId,
                ReportedByUserId = userId,
                Reason = request.Reason,
                Description = request.Description,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<InvoiceReport>().AddAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Report created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice report", ex);
            return Result<int>.Failure("Failed to create invoice report");
        }
    }

    public async Task<Result<int>> UpdateAsync(int id, UpdateInvoiceReportRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice report ID: {id}");

            var entity = await _unitOfWork.Repository<InvoiceReport>()
                .GetByIdAsync(id);

            if (entity == null)
                return Result<int>.Failure("Report not found");

            if (request.Reason.HasValue)
                entity.Reason = request.Reason.Value;

            if (request.Description != null)
                entity.Description = request.Description;

            if (request.Status.HasValue)
                entity.Status = request.Status.Value;

            entity.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<InvoiceReport>().UpdateAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Report updated successfully");
        }
        catch (Exception ex)
        {
            LogError("Error updating invoice report", ex);
            return Result<int>.Failure("Failed to update invoice report");
        }
    }

    public async Task<Result<int>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice report ID: {id}");

            var entity = await _unitOfWork.Repository<InvoiceReport>().GetByIdAsync(id);
            if (entity == null)
                return Result<int>.Failure("Report not found");

            await _unitOfWork.Repository<InvoiceReport>().DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "Report deleted successfully");
        }
        catch (Exception ex)
        {
            LogError("Error deleting invoice report", ex);
            return Result<int>.Failure("Failed to delete invoice report");
        }
    }

    public async Task<Result<InvoiceReportResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _unitOfWork.Repository<InvoiceReport>()
                .Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (entity == null)
                return Result<InvoiceReportResponse>.Failure("Report not found");

            var response = _mapper.Map<InvoiceReportResponse>(entity);
            return Result<InvoiceReportResponse>.Success(response, "Report retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice report", ex);
            return Result<InvoiceReportResponse>.Failure("Failed to get invoice report");
        }
    }

    public async Task<Result<List<InvoiceReportResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _unitOfWork.Repository<InvoiceReport>()
                .Entities
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var response = _mapper.Map<List<InvoiceReportResponse>>(entities);
            return Result<List<InvoiceReportResponse>>.Success(response, "Reports retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice reports", ex);
            return Result<List<InvoiceReportResponse>>.Failure("Failed to get invoice reports");
        }
    }

    public async Task<PaginatedResult<InvoiceReportDetailResponse>> GetWithPaginationAsync(GetInvoiceReportWithPagination query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice reports with pagination - Page: {query.PageNumber}, Size: {query.PageSize}, StartDate: {query.StartDate}, EndDate: {query.EndDate}");

            var reportsQuery = _unitOfWork.Repository<InvoiceReport>()
                .Entities
                .AsNoTracking()
                .Include(r => r.ReportedByUser)
                .AsQueryable();

            if (query.InvoiceId.HasValue)
                reportsQuery = reportsQuery.Where(r => r.InvoiceId == query.InvoiceId.Value);

            if (query.ReportedByUserId.HasValue)
                reportsQuery = reportsQuery.Where(r => r.ReportedByUserId == query.ReportedByUserId.Value);

            if (query.Status.HasValue)
                reportsQuery = reportsQuery.Where(r => r.Status == query.Status.Value);

            if (query.Reason.HasValue)
                reportsQuery = reportsQuery.Where(r => r.Reason == query.Reason.Value);

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                reportsQuery = reportsQuery.Where(r =>
                    r.Description != null && r.Description.Contains(query.Keyword));
            }

            // Apply date range filter using utility
            if (DateTimeUtility.TryParseToUtc(query.StartDate, out var startDateUtc))
            {
                reportsQuery = reportsQuery.Where(r => r.CreatedDate >= startDateUtc);
            }

            if (DateTimeUtility.TryParseToUtc(query.EndDate, out var endDateUtc))
            {
                var endDateInclusive = DateTimeUtility.GetEndOfDayUtc(endDateUtc);
                reportsQuery = reportsQuery.Where(r => r.CreatedDate <= endDateInclusive);
            }

            return await reportsQuery
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .ProjectTo<InvoiceReportDetailResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice reports with pagination", ex);
            throw new Exception("An error occurred while retrieving invoice reports with pagination");
        }
    }

    public async Task<PaginatedResult<InvoiceReportDetailResponse>> GetByUserWithPaginationAsync(GetInvoiceReportByUserWithPagination query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice reports by user {query.UserId} with pagination - Page: {query.PageNumber}, Size: {query.PageSize}, StartDate: {query.StartDate}, EndDate: {query.EndDate}");

            var reportsQuery = _unitOfWork.Repository<InvoiceReport>()
                .Entities
                .AsNoTracking()
                .Include(r => r.ReportedByUser)
                .Where(r => r.ReportedByUserId == query.UserId);

            if (query.Status.HasValue)
                reportsQuery = reportsQuery.Where(r => r.Status == query.Status.Value);

            // Apply date range filter using utility
            if (DateTimeUtility.TryParseToUtc(query.StartDate, out var startDateUtc))
            {
                reportsQuery = reportsQuery.Where(r => r.CreatedDate >= startDateUtc);
            }

            if (DateTimeUtility.TryParseToUtc(query.EndDate, out var endDateUtc))
            {
                var endDateInclusive = DateTimeUtility.GetEndOfDayUtc(endDateUtc);
                reportsQuery = reportsQuery.Where(r => r.CreatedDate <= endDateInclusive);
            }

            return await reportsQuery
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .ProjectTo<InvoiceReportDetailResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice reports by user {query.UserId} with pagination", ex);
            throw new Exception("An error occurred while retrieving invoice reports by user with pagination");
        }
    }
}
