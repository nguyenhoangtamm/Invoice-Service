using AutoMapper;
using AutoMapper.QueryableExtensions;
using Invoice.Application.Extensions;
using Invoice.Application.Interfaces;
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

public class InvoiceLineService : BaseService, IInvoiceLineService
{
    public InvoiceLineService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceLineService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateInvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice line for invoice: {request.InvoiceId}");

            var invoice = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().GetByIdAsync(request.InvoiceId);
            if (invoice == null) return Result<int>.Failure("Invoice not found");

            var line = new InvoiceLine
            {
                InvoiceId = request.InvoiceId,
                LineNumber = request.LineNumber,
                Name = request.Name,
                Unit = request.Unit,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                Discount = request.Discount ?? 0m,
                TaxRate = request.TaxRate ?? 0m,
                TaxAmount = request.TaxAmount ?? 0m,
                LineTotal = request.LineTotal,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<InvoiceLine>().AddAsync(line);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(line.Id, "Invoice line created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice line", ex);
            return Result<int>.Failure("Failed to create invoice line");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateInvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice line ID: {id}");

            var line = await _unitOfWork.Repository<InvoiceLine>().GetByIdAsync(id);
            if (line == null) return Result<int>.Failure("Invoice line not found");

            if (request.InvoiceId.HasValue)
            {
                var invoice = await _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>().GetByIdAsync(request.InvoiceId.Value);
                if (invoice == null) return Result<int>.Failure("Invoice not found");
                line.InvoiceId = request.InvoiceId.Value;
            }

            if (request.LineNumber.HasValue) line.LineNumber = request.LineNumber.Value;
            if (request.Name != null) line.Name = request.Name;
            if (request.Unit != null) line.Unit = request.Unit;
            if (request.Quantity.HasValue) line.Quantity = request.Quantity.Value;
            if (request.UnitPrice.HasValue) line.UnitPrice = request.UnitPrice.Value;
            if (request.Discount.HasValue) line.Discount = request.Discount.Value;
            if (request.TaxRate.HasValue) line.TaxRate = request.TaxRate.Value;
            if (request.TaxAmount.HasValue) line.TaxAmount = request.TaxAmount.Value;
            if (request.LineTotal.HasValue) line.LineTotal = request.LineTotal.Value;

            line.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<InvoiceLine>().UpdateAsync(line);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(line.Id, "Invoice line updated successfully");
        }
        catch (Exception ex)
        {
            LogError("Error updating invoice line", ex);
            return Result<int>.Failure("Failed to update invoice line");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice line ID: {id}");

            var line = await _unitOfWork.Repository<InvoiceLine>().GetByIdAsync(id);
            if (line == null) return Result<int>.Failure("Invoice line not found");

            await _unitOfWork.Repository<InvoiceLine>().DeleteAsync(line);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(line.Id, "Invoice line deleted successfully");
        }
        catch (Exception ex)
        {
            LogError("Error deleting invoice line", ex);
            return Result<int>.Failure("Failed to delete invoice line");
        }
    }

    public async Task<Result<InvoiceLineResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var line = await _unitOfWork.Repository<InvoiceLine>().GetByIdAsync(id);
            if (line == null) return Result<InvoiceLineResponse>.Failure("Invoice line not found");

            var response = _mapper.Map<InvoiceLineResponse>(line);
            return Result<InvoiceLineResponse>.Success(response, "Invoice line retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice line", ex);
            return Result<InvoiceLineResponse>.Failure("Failed to get invoice line");
        }
    }

    public async Task<Result<List<InvoiceLineResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var lines = await _unitOfWork.Repository<InvoiceLine>().GetAllAsync();
            var response = _mapper.Map<List<InvoiceLineResponse>>(lines);
            return Result<List<InvoiceLineResponse>>.Success(response, "Invoice lines retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice lines", ex);
            return Result<List<InvoiceLineResponse>>.Failure("Failed to get invoice lines");
        }
    }

    public async Task<Result<List<InvoiceLineResponse>>> GetByInvoiceId(int invoiceId, CancellationToken cancellationToken)
    {
        try
        {
            var lines = await _unitOfWork.Repository<InvoiceLine>().Entities
                .Where(l => l.InvoiceId == invoiceId)
                .ToListAsync(cancellationToken);
            var response = _mapper.Map<List<InvoiceLineResponse>>(lines);
            return Result<List<InvoiceLineResponse>>.Success(response, "Invoice lines retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice lines", ex);
            return Result<List<InvoiceLineResponse>>.Failure("Failed to get invoice lines");
        }
    }

    public async Task<PaginatedResult<InvoiceLineResponse>> GetWithPagination(GetInvoiceLineWithPagination query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice lines with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var invoiceLinesQuery = _unitOfWork.Repository<InvoiceLine>().Entities.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                invoiceLinesQuery = invoiceLinesQuery.Where(l => l.Name.Contains(query.Keyword));
            }
            return await invoiceLinesQuery.OrderBy(x => x.CreatedDate)
                .ProjectTo<InvoiceLineResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        }
        catch (Exception ex)
        {
            LogError("Error getting invoice lines with pagination", ex);
            throw new Exception("An error occurred while retrieving invoice line with pagination");
        }
    }
}
