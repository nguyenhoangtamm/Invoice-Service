using AutoMapper;
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
    private readonly IInvoiceLineRepository _invoiceLineRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceLineService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceLineService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IInvoiceLineRepository invoiceLineRepository, IInvoiceRepository invoiceRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _invoiceLineRepository = invoiceLineRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Result<int>> Create(CreateInvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice line for invoice: {request.InvoiceId}");

            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
            if (invoice == null) return Result<int>.Failure("Invoice not found");

            var line = new InvoiceLine
            {
                InvoiceId = request.InvoiceId,
                LineNumber = request.LineNumber,
                Description = request.Description,
                Unit = request.Unit,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                Discount = request.Discount,
                TaxRate = request.TaxRate,
                TaxAmount = request.TaxAmount,
                LineTotal = request.LineTotal,
                CreatedBy = UserName ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            await _invoiceLineRepository.AddAsync(line);
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

            var line = await _invoiceLineRepository.GetByIdAsync(id);
            if (line == null) return Result<int>.Failure("Invoice line not found");

            if (request.InvoiceId.HasValue)
            {
                var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId.Value);
                if (invoice == null) return Result<int>.Failure("Invoice not found");
                line.InvoiceId = request.InvoiceId.Value;
            }

            if (request.LineNumber.HasValue) line.LineNumber = request.LineNumber.Value;
            line.Description = request.Description ?? line.Description;
            line.Unit = request.Unit ?? line.Unit;
            if (request.Quantity.HasValue) line.Quantity = request.Quantity.Value;
            if (request.UnitPrice.HasValue) line.UnitPrice = request.UnitPrice.Value;
            if (request.Discount.HasValue) line.Discount = request.Discount.Value;
            if (request.TaxRate.HasValue) line.TaxRate = request.TaxRate.Value;
            if (request.TaxAmount.HasValue) line.TaxAmount = request.TaxAmount.Value;
            if (request.LineTotal.HasValue) line.LineTotal = request.LineTotal.Value;

            line.UpdatedBy = UserName;
            line.UpdatedDate = DateTime.UtcNow;

            await _invoiceLineRepository.UpdateAsync(line);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(line.Id, "Invoice line updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice line ID: {id}", ex);
            return Result<int>.Failure("Failed to update invoice line");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice line ID: {id}");

            var line = await _invoiceLineRepository.GetByIdAsync(id);
            if (line == null) return Result<int>.Failure("Invoice line not found");

            line.IsDeleted = true;
            line.UpdatedBy = UserName;
            line.UpdatedDate = DateTime.UtcNow;

            await _invoiceLineRepository.UpdateAsync(line);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(line.Id, "Invoice line deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice line ID: {id}", ex);
            return Result<int>.Failure("Failed to delete invoice line");
        }
    }

    public async Task<Result<InvoiceLineResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var line = await _invoiceLineRepository.GetByIdAsync(id);
            if (line == null) return Result<InvoiceLineResponse>.Failure("Invoice line not found");

            var dto = _mapper.Map<InvoiceLineResponse>(line);
            return Result<InvoiceLineResponse>.Success(dto, "Invoice line retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice line ID: {id}", ex);
            return Result<InvoiceLineResponse>.Failure("Failed to get invoice line");
        }
    }

    public async Task<Result<List<InvoiceLineResponse>>> GetByInvoiceId(int invoiceId, CancellationToken cancellationToken)
    {
        try
        {
            var list = await _invoiceLineRepository.GetByInvoiceIdAsync(invoiceId);
            var dto = _mapper.Map<List<InvoiceLineResponse>>(list);
            return Result<List<InvoiceLineResponse>>.Success(dto, "Invoice lines retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice lines", ex);
            return Result<List<InvoiceLineResponse>>.Failure("Failed to get invoice lines");
        }
    }
}
