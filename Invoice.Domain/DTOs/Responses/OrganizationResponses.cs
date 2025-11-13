using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Responses;

public record OrganizationResponse : IMapFrom<Organization>
{
    public int Id { get; init; }
    public string OrganizationName { get; init; } = string.Empty;
    public string? OrganizationTaxId { get; init; }
    public string? OrganizationAddress { get; init; }
    public string? OrganizationPhone { get; init; }
    public string? OrganizationEmail { get; init; }
    public string? OrganizationBankAccount { get; init; }
    public int UserId { get; init; }
}

public record GetOrganizationsWithPaginationQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Keyword { get; init; }
}
