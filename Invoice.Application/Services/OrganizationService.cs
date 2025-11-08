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

public class OrganizationService : BaseService, IOrganizationService
{
    public OrganizationService(IHttpContextAccessor httpContextAccessor, ILogger<OrganizationService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<List<OrganizationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all organizations");
            var repo = _unitOfWork.Repository<Organization>();
            var list = await repo.Entities.AsNoTracking().ToListAsync(cancellationToken);
            var dto = _mapper.Map<List<OrganizationResponse>>(list);
            return Result<List<OrganizationResponse>>.Success(dto, "Organizations retrieved successfully");
        }
        catch (Exception ex)
        {
            LogError("Error while getting organizations", ex);
            return Result<List<OrganizationResponse>>.Failure("Failed to retrieve organizations");
        }
    }

    public async Task<Result<PaginatedResult<OrganizationResponse>>> GetWithPagination(GetOrganizationsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting organizations with pagination");
            var repo = _unitOfWork.Repository<Organization>();
            var q = repo.Entities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var k = query.Keyword.Trim().ToLower();
                q = q.Where(o => o.OrganizationName.ToLower().Contains(k) || (o.OrganizationEmail != null && o.OrganizationEmail.ToLower().Contains(k)));
            }

            var count = await q.CountAsync(cancellationToken);
            var items = await q.OrderBy(o => o.OrganizationName)
                               .Skip((query.PageNumber - 1) * query.PageSize)
                               .Take(query.PageSize)
                               .ToListAsync(cancellationToken);

            var dto = _mapper.Map<List<OrganizationResponse>>(items);
            return Result<PaginatedResult<OrganizationResponse>>.Success(new PaginatedResult<OrganizationResponse>(true, dto, null, count, query.PageNumber, query.PageSize));
        }
        catch (Exception ex)
        {
            LogError("Error while getting organizations with pagination", ex);
            return Result<PaginatedResult<OrganizationResponse>>.Failure("Failed to retrieve organizations");
        }
    }

    public async Task<Result<OrganizationResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting organization by id: {id}");
            var repo = _unitOfWork.Repository<Organization>();
            var org = await repo.Entities.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
            if (org == null) return Result<OrganizationResponse>.Failure("Organization not found");
            var dto = _mapper.Map<OrganizationResponse>(org);
            return Result<OrganizationResponse>.Success(dto, "Organization retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting organization by id: {id}", ex);
            return Result<OrganizationResponse>.Failure("Failed to retrieve organization");
        }
    }

    public async Task<Result<int>> Create(CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Creating organization");
            var repo = _unitOfWork.Repository<Organization>();
            var org = new Organization
            {
                OrganizationName = request.OrganizationName,
                OrganizationTaxId = request.OrganizationTaxId,
                OrganizationAddress = request.OrganizationAddress,
                OrganizationPhone = request.OrganizationPhone,
                OrganizationEmail = request.OrganizationEmail,
                OrganizationBankAccount = request.OrganizationBankAccount,
                UserId = request.UserId,
                CreatedBy = UserName,
                CreatedDate = DateTime.UtcNow
            };

            await repo.AddAsync(org);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(org.Id, "Organization created successfully");
        }
        catch (Exception ex)
        {
            LogError("Error creating organization", ex);
            return Result<int>.Failure("Failed to create organization");
        }
    }

    public async Task<Result<int>> Update(int id, UpdateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Updating organization id: {id}");
            var repo = _unitOfWork.Repository<Organization>();
            var org = await repo.GetByIdAsync(id);
            if (org == null) return Result<int>.Failure("Organization not found");

            if (!string.IsNullOrWhiteSpace(request.OrganizationName)) org.OrganizationName = request.OrganizationName;
            if (request.OrganizationTaxId != null) org.OrganizationTaxId = request.OrganizationTaxId;
            if (request.OrganizationAddress != null) org.OrganizationAddress = request.OrganizationAddress;
            if (request.OrganizationPhone != null) org.OrganizationPhone = request.OrganizationPhone;
            if (request.OrganizationEmail != null) org.OrganizationEmail = request.OrganizationEmail;
            if (request.OrganizationBankAccount != null) org.OrganizationBankAccount = request.OrganizationBankAccount;

            org.UpdatedBy = UserName;
            org.UpdatedDate = DateTime.UtcNow;

            await repo.UpdateAsync(org);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(org.Id, "Organization updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating organization id: {id}", ex);
            return Result<int>.Failure("Failed to update organization");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting organization id: {id}");
            var repo = _unitOfWork.Repository<Organization>();
            var org = await repo.GetByIdAsync(id);
            if (org == null) return Result<int>.Failure("Organization not found");

            await repo.DeleteAsync(org);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(id, "Organization deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting organization id: {id}", ex);
            return Result<int>.Failure("Failed to delete organization");
        }
    }
}
