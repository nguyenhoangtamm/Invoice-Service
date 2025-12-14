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

public class OrganizationService : BaseService, IOrganizationService
{
    public OrganizationService(IHttpContextAccessor httpContextAccessor, ILogger<OrganizationService> logger,
        IUnitOfWork unitOfWork, IMapper mapper)
        : base(httpContextAccessor, logger, unitOfWork, mapper)
    {
    }

    public async Task<Result<int>> Create(CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Creating organization: {request.OrganizationName}");
            var userId = GetCurrentUserId();
            var org = new Organization
            {
                OrganizationName = request.OrganizationName,
                OrganizationTaxId = request.OrganizationTaxId,
                OrganizationAddress = request.OrganizationAddress,
                OrganizationPhone = request.OrganizationPhone,
                OrganizationEmail = request.OrganizationEmail,
                OrganizationBankAccount = request.OrganizationBankAccount,
                CreatedDate = DateTime.UtcNow
            };

            // Only assign UserId if the current user is not an Admin
            var isAdmin = Roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));
            if (!isAdmin)
            {
                org.UserId = userId;
            }

            await _unitOfWork.Repository<Organization>().AddAsync(org);
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

            var org = await _unitOfWork.Repository<Organization>().GetByIdAsync(id);
            if (org == null) return Result<int>.Failure("Organization not found");

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.OrganizationName)) org.OrganizationName = request.OrganizationName;
            if (request.OrganizationTaxId != null) org.OrganizationTaxId = request.OrganizationTaxId;
            if (request.OrganizationAddress != null) org.OrganizationAddress = request.OrganizationAddress;
            if (request.OrganizationPhone != null) org.OrganizationPhone = request.OrganizationPhone;
            if (request.OrganizationEmail != null) org.OrganizationEmail = request.OrganizationEmail;
            if (request.OrganizationBankAccount != null) org.OrganizationBankAccount = request.OrganizationBankAccount;

            org.UpdatedDate = DateTime.UtcNow;

            await _unitOfWork.Repository<Organization>().UpdateAsync(org);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(org.Id, "Organization updated successfully");
        }
        catch (Exception ex)
        {
            LogError("Error updating organization", ex);
            return Result<int>.Failure("Failed to update organization");
        }
    }

    public async Task<Result<int>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Deleting organization ID: {id}");

            var org = await _unitOfWork.Repository<Organization>().GetByIdAsync(id);
            if (org == null) return Result<int>.Failure("Organization not found");

            await _unitOfWork.Repository<Organization>().DeleteAsync(org);
            await _unitOfWork.Save(cancellationToken);

            return Result<int>.Success(org.Id, "Organization deleted successfully");
        }
        catch (Exception ex)
        {
            LogError("Error deleting organization", ex);
            return Result<int>.Failure("Failed to delete organization");
        }
    }

    public async Task<Result<OrganizationResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var org = await _unitOfWork.Repository<Organization>().GetByIdAsync(id);
            if (org == null) return Result<OrganizationResponse>.Failure("Organization not found");

            var response = _mapper.Map<OrganizationResponse>(org);
            return Result<OrganizationResponse>.Success(response, "Organization retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting organization", ex);
            return Result<OrganizationResponse>.Failure("Failed to get organization");
        }
    }

    public async Task<Result<List<OrganizationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var organizations = await _unitOfWork.Repository<Organization>().GetAllAsync();
            var response = _mapper.Map<List<OrganizationResponse>>(organizations);
            return Result<List<OrganizationResponse>>.Success(response, "Organizations retrieved");
        }
        catch (Exception ex)
        {
            LogError("Error getting organizations", ex);
            return Result<List<OrganizationResponse>>.Failure("Failed to get organizations");
        }
    }

    public async Task<PaginatedResult<OrganizationResponse>> GetWithPagination(GetOrganizationWithPagination query, CancellationToken cancellationToken)
    {
        try
        {
            LogInformation($"Getting organizations with pagination - Page: {query.PageNumber}, Size: {query.PageSize}");

            var organizationsQuery = _unitOfWork.Repository<Organization>().Entities
                .Where(o => !o.IsDeleted)
                .AsQueryable();

            // Apply keyword filter if provided
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                var kw = query.Keyword.ToLower();
                organizationsQuery = organizationsQuery.Where(o => o.OrganizationName.ToLower().Contains(kw) ||
                                        (o.OrganizationTaxId ?? string.Empty).ToLower().Contains(kw) || (o.OrganizationEmail ?? string.Empty).ToLower().Contains(kw));
            }

            return await organizationsQuery.OrderBy(x => x.CreatedDate)
                .ProjectTo<OrganizationResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        }
        catch (Exception ex)
        {
            LogError("Error getting organizations with pagination", ex);
            throw new Exception("An error occurred while retrieving orgnization with pagination");
        }
    }

    // New method: get organization by its owner user id
    public async Task<Result<OrganizationResponse>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            var org = await _unitOfWork.Repository<Organization>().Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);

            if (org == null) return Result<OrganizationResponse>.Failure("Organization not found for user");

            var response = _mapper.Map<OrganizationResponse>(org);
            return Result<OrganizationResponse>.Success(response, "Organization retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting organization for user {userId}", ex);
            return Result<OrganizationResponse>.Failure("Failed to get organization for user");
        }
    }

    // New method: get all organizations for a user
    public async Task<Result<List<OrganizationResponse>>> GetOrganizationsByUserId(int userId, CancellationToken cancellationToken)
    {
        try
        {
            var organizations = await _unitOfWork.Repository<Organization>().Entities
                .AsNoTracking()
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!organizations.Any())
                return Result<List<OrganizationResponse>>.Failure("No organizations found for user");

            var response = _mapper.Map<List<OrganizationResponse>>(organizations);
            return Result<List<OrganizationResponse>>.Success(response, "Organizations retrieved");
        }
        catch (Exception ex)
        {
            LogError($"Error getting organizations for user {userId}", ex);
            return Result<List<OrganizationResponse>>.Failure("Failed to get organizations for user");
        }
    }
}
