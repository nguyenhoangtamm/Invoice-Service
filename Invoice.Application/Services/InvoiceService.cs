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

public class InvoiceService : BaseService, IInvoiceService
{
    public InvoiceService(IHttpContextAccessor httpContextAccessor, ILogger<InvoiceService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<List<InvoiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all invoices");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>();
            var list = await repo.Entities.AsNoTracking().Include(i => i.Lines).ToListAsync(cancellationToken);
            var dto = _mapper.Map<List<InvoiceResponse>>(list);
            return Result<List<InvoiceResponse>>.Success(dto, "Invoices retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices", ex);
            return Result<List<InvoiceResponse>>.Failure("Failed to retrieve invoices");
        }
    }

    public async Task<Result<PaginatedResult<InvoiceResponse>>> GetWithPagination(GetInvoicesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting invoices paged");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>();
            IQueryable<Invoice.Domain.Entities.Invoice> q = repo.Entities.AsNoTracking().Include(i => i.Lines);

            if (query.TenantOrganizationId.HasValue)
                q = q.Where(i => i.TenantOrganizationId == query.TenantOrganizationId.Value);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var k = query.Keyword.Trim().ToLower();
                q = q.Where(i => i.InvoiceNumber.ToLower().Contains(k) || (i.CustomerName != null && i.CustomerName.ToLower().Contains(k)));
            }

            var count = await q.CountAsync(cancellationToken);
            var items = await q.OrderByDescending(i => i.IssuedDate)
                               .Skip((query.PageNumber - 1) * query.PageSize)
                               .Take(query.PageSize)
                               .ToListAsync(cancellationToken);

            var dto = _mapper.Map<List<InvoiceResponse>>(items);
            return Result<PaginatedResult<InvoiceResponse>>.Success(new PaginatedResult<InvoiceResponse>(true, dto, null, count, query.PageNumber, query.PageSize));
        }
        catch (Exception ex)
        {
            LogError("Error getting invoices paged", ex);
            return Result<PaginatedResult<InvoiceResponse>>.Failure("Failed to retrieve invoices");
        }
    }

    public async Task<Result<InvoiceResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting invoice by id: {id}");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>();
            var item = await repo.Entities.AsNoTracking().Include(i => i.Lines).FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            if (item == null) return Result<InvoiceResponse>.Failure("Invoice not found");
            var dto = _mapper.Map<InvoiceResponse>(item);
            return Result<InvoiceResponse>.Success(dto, "Invoice retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting invoice by id: {id}", ex);
            return Result<InvoiceResponse>.Failure("Failed to retrieve invoice");
        }
    }

    public async Task<Result<int>> Create(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Creating invoice");
            // Basic tenant check: if user is not admin, ensure user belongs to the tenant (via claim "organizationId")
            var orgClaim = HttpContextAccessor.HttpContext?.User?.FindFirst("organizationId")?.Value;
            if (!string.IsNullOrEmpty(orgClaim) && !Roles.Contains("Admin") && !Roles.Contains("Administrator"))
            {
                if (int.TryParse(orgClaim, out var orgId))
                {
                    if (orgId != request.TenantOrganizationId)
                    {
                        return Result<int>.Failure("Forbidden: tenant mismatch");
                    }
                }
            }

            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>();
            var entity = new Invoice.Domain.Entities.Invoice
            {
                InvoiceNumber = request.InvoiceNumber,
                FormNumber = request.FormNumber,
                Serial = request.Serial,
                TenantOrganizationId = request.TenantOrganizationId,
                IssuedByUserId = request.IssuedByUserId,
                SellerName = request.SellerName,
                SellerTaxId = request.SellerTaxId,
                SellerAddress = request.SellerAddress,
                SellerPhone = request.SellerPhone,
                SellerEmail = request.SellerEmail,
                CustomerName = request.CustomerName,
                CustomerTaxId = request.CustomerTaxId,
                CustomerAddress = request.CustomerAddress,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                Status = request.Status,
                IssuedDate = request.IssuedDate,
                Subtotal = request.Subtotal,
                TaxAmount = request.TaxAmount,
                DiscountAmount = request.DiscountAmount,
                TotalAmount = request.TotalAmount,
                Currency = request.Currency,
                Note = request.Note,
                BatchId = request.BatchId,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            if (request.Lines != null && request.Lines.Any())
            {
                foreach (var ln in request.Lines)
                {
                    var line = new Invoice.Domain.Entities.InvoiceLine
                    {
                        LineNumber = ln.LineNumber,
                        Description = ln.Description,
                        Unit = ln.Unit,
                        Quantity = ln.Quantity,
                        UnitPrice = ln.UnitPrice,
                        Discount = ln.Discount,
                        TaxRate = ln.TaxRate,
                        TaxAmount = Math.Round(ln.Quantity * ln.UnitPrice * (ln.TaxRate / 100m), 2),
                        LineTotal = Math.Round(ln.Quantity * ln.UnitPrice - ln.Discount + (ln.Quantity * ln.UnitPrice * (ln.TaxRate / 100m)), 2),
                        CreatedBy = UserName,
                        CreatedDate = DateTime.UtcNow
                    };
                    entity.Lines.Add(line);
                }
            }

            await repo.AddAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Invoice created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating invoice", ex);
            return Result<int>.Failure("Failed to create invoice");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating invoice id: {id}");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>();
            var entity = await repo.Entities.Include(i => i.Lines).FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            if (entity == null) return Result<int>.Failure("Invoice not found");

            if (request.InvoiceNumber != null) entity.InvoiceNumber = request.InvoiceNumber;
            if (request.FormNumber != null) entity.FormNumber = request.FormNumber;
            if (request.Serial != null) entity.Serial = request.Serial;
            if (request.Status != null) entity.Status = request.Status;
            if (request.IssuedDate.HasValue) entity.IssuedDate = request.IssuedDate;
            if (request.Subtotal.HasValue) entity.Subtotal = request.Subtotal.Value;
            if (request.TaxAmount.HasValue) entity.TaxAmount = request.TaxAmount.Value;
            if (request.DiscountAmount.HasValue) entity.DiscountAmount = request.DiscountAmount.Value;
            if (request.TotalAmount.HasValue) entity.TotalAmount = request.TotalAmount.Value;
            if (request.Currency != null) entity.Currency = request.Currency;
            if (request.Note != null) entity.Note = request.Note;
            if (request.BatchId.HasValue) entity.BatchId = request.BatchId;

            entity.UpdatedBy = UserName;
            entity.UpdatedDate = DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(entity.Id, "Invoice updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating invoice id: {id}", ex);
            return Result<int>.Failure("Failed to update invoice");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting invoice id: {id}");
            var repo = _unitOfWork.Repository<Invoice.Domain.Entities.Invoice>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return Result<int>.Failure("Invoice not found");

            await repo.DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "Invoice deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting invoice id: {id}", ex);
            return Result<int>.Failure("Failed to delete invoice");
        }
    }
}
