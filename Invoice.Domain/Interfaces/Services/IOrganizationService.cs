using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;

namespace Invoice.Domain.Interfaces.Services;

public interface IOrganizationService
{
    Task<Result<int>> Create(CreateOrganizationRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Update(int id, UpdateOrganizationRequest request, CancellationToken cancellationToken);
    Task<Result<int>> Delete(int id, CancellationToken cancellationToken);
    Task<Result<OrganizationResponse>> GetById(int id, CancellationToken cancellationToken);
    Task<Result<List<OrganizationResponse>>> GetAll(CancellationToken cancellationToken);
    Task<Result<PaginatedResult<OrganizationResponse>>> GetWithPagination(GetOrganizationWithPagination request, CancellationToken cancellationToken);
}
