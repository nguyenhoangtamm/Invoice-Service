using AutoMapper;
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

    public async Task<Result<List<InvoiceLineResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all invoice lines");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.InvoiceLine>();
            var items = await repo.Entities.AsNoTracking().ToListAsync(cancellationToken);
            var dto = _mapper.Map<List<InvoiceLineResponse>>(items);
            return Result<List<InvoiceLineResponse>>.Success(dto, "Invoice lines retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoice lines", ex);
            return Result<List<InvoiceLineResponse>>.Failure("Failed to retrieve invoice lines");
        }
    }

    public async Task<Result<InvoiceLineResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice line id: {id}");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.InvoiceLine>();
            var item = await repo.Entities.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
            if (item == null) return Result<InvoiceLineResponse>.Failure("Invoice line not found");
            var dto = _mapper.Map<InvoiceLineResponse>(item);
            return Result<InvoiceLineResponse>.Success(dto, "Invoice line retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice line id: {id}", ex);
            return Result<InvoiceLineResponse>.Failure("Failed to retrieve invoice line");
        }
    }

    public async Task<Result<int>> Create(int invoiceId, InvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating invoice line for invoice id: {invoiceId}");
            var invoiceRepo = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>();
            var invoice = await invoiceRepo.GetByIdAsync(invoiceId);
            if (invoice == null) return Result<int>.Failure("Invoice not found");

            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.InvoiceLine>();
            var entity = new Invoice.Domain.Entities.InvoiceLine
            {
                InvoiceId = invoiceId,
                LineNumber = request.LineNumber,
                Description = request.Description,
                Unit = request.Unit,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                Discount = request.Discount,
                TaxRate = request.TaxRate,
                TaxAmount = Math.Round(request.Quantity * request.UnitPrice * (request.TaxRate / 100m), 2),
                LineTotal = Math.Round(request.Quantity * request.UnitPrice - request.Discount + (request.Quantity * request.UnitPrice * (request.TaxRate / 100m)), 2),
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await repo.AddAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Invoice line created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice line", ex);
            return Result<int>.Failure("Failed to create invoice line");
        }
    }

    public async Task<Result<int>> Update(int id, InvoiceLineRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice line id: {id}");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.InvoiceLine>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("Invoice line not found");

            entity.LineNumber = request.LineNumber;
            entity.Description = request.Description;
            entity.Unit = request.Unit;
            entity.Quantity = request.Quantity;
            entity.UnitPrice = request.UnitPrice;
            entity.Discount = request.Discount;
            entity.TaxRate = request.TaxRate;
            entity.TaxAmount = Math.Round(request.Quantity * request.UnitPrice * (request.TaxRate / 100m), 2);
            entity.LineTotal = Math.Round(request.Quantity * request.UnitPrice - request.Discount + (request.Quantity * request.UnitPrice * (request.TaxRate / 100m)), 2);

            entity.UpdatedBy = UserName;
            entity.UpdatedDate = DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Invoice line updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice line id: {id}", ex);
            return Result<int>.Failure("Failed to update invoice line");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice line id: {id}");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.InvoiceLine>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("Invoice line not found");

            await repo.DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "Invoice line deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice line id: {id}", ex);
            return Result<int>.Failure("Failed to delete invoice line");
        }
    }
}
