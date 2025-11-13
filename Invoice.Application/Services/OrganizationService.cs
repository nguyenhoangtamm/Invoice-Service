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

public class OrganizationService : BaseService, IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(IHttpContextAccessor httpContextAccessor, ILogger<OrganizationService> logger,
        IUnitOfWork unitOfWork, IMapper mapper, IOrganizationRepository organizationRepository)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result<int>> Create(CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating organization: {request.OrganizationName}");

            var org = new Organization
            {
                OrganizationName = request.OrganizationName,
                OrganizationTaxId = request.OrganizationTaxId,
                OrganizationAddress = request.OrganizationAddress,
                OrganizationPhone = request.OrganizationPhone,
                OrganizationEmail = request.OrganizationEmail,
                OrganizationBankAccount = request.OrganizationBankAccount,
                CreatedBy = UserName ?? "System",
                CreatedDate = DateTime.UtcNow
            };

            await _organizationRepository.AddAsync(org);
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
            LogInformation($"Updating organization ID: {id}");

            var org = await _organizationRepository.GetByIdAsync(id);
            if (org == null)
                return Result<int>.Failure("Organization not found");

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.OrganizationName)) org.OrganizationName = request.OrganizationName;
            if (request.OrganizationTaxId != null) org.OrganizationTaxId = request.OrganizationTaxId;
            if (request.OrganizationAddress != null) org.OrganizationAddress = request.OrganizationAddress;
            if (request.OrganizationPhone != null) org.OrganizationPhone = request.OrganizationPhone;
            if (request.OrganizationEmail != null) org.OrganizationEmail = request.OrganizationEmail;
            if (request.OrganizationBankAccount != null) org.OrganizationBankAccount = request.OrganizationBankAccount;

            org.UpdatedBy = UserName;
            org.UpdatedDate = DateTime.UtcNow;

            await _organizationRepository.UpdateAsync(org);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(org.Id, "Organization updated successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error updating organization ID: {id}", ex);
            return Result<int>.Failure("Failed to update organization");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting organization ID: {id}");

            var org = await _organizationRepository.GetByIdAsync(id);
            if (org == null)
                return Result<int>.Failure("Organization not found");

            // Soft delete
            org.IsDeleted = true;
            org.UpdatedBy = UserName;
            org.UpdatedDate = DateTime.UtcNow;

            await _organizationRepository.UpdateAsync(org);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(org.Id, "Organization deleted successfully");
        }
        catch (Exception ex)
        {
            LogError($"Error deleting organization ID: {id}", ex);
            return Result<int>.Failure("Failed to delete organization");
        }
    }

    public async Task<Result<OrganizationResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting organization ID: {id}");

            var org = await _organizationRepository.GetByIdAsync(id);
            if (org == null)
                return Result<OrganizationResponse>.Failure("Organization not found");

            var dto = _mapper.Map<OrganizationResponse>(org);
            return Result<OrganizationResponse>.Success(dto, "Organization retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting organization ID: {id}", ex);
            return Result<OrganizationResponse>.Failure("Failed to get organization");
        }
    }

    public async Task<Result<List<OrganizationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            LogInformation("Getting all organizations");
            var list = await _organizationRepository.GetAllAsync();
            var dto = _mapper.Map<List<OrganizationResponse>>(list);
            return Result<List<OrganizationResponse>>.Success(dto, "Organizations retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations", ex);
            return Result<List<OrganizationResponse>>.Failure("Failed to get organizations");
        }
    }

    public async Task<Result<PaginatedResult<OrganizationResponse>>> GetWithPagination(GetOrganizationsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting organizations pagination: {query.PageNumber}/{query.PageSize}");
            var q = _organizationRepository.GetAllAsync().Result.AsQueryable();
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                var kw = query.Keyword.ToLower();
                q = q.Where(o => o.OrganizationName.ToLower().Contains(kw) || (o.OrganizationTaxId ?? string.Empty).ToLower().Contains(kw));
            }

            var total = q.Count();
            var items = q.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList();
            var dto = _mapper.Map<List<OrganizationResponse>>(items);
            var paged = PaginatedResult<OrganizationResponse>.Create(dto, total, query.PageNumber, query.PageSize);
            return Result<PaginatedResult<OrganizationResponse>>.Success(paged, "Organizations retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations with pagination", ex);
            return Result<PaginatedResult<OrganizationResponse>>.Failure("Failed to get organizations");
        }
    }
}
