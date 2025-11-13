// Request DTOs for Organization CRUD
namespace Invoice.Domain.DTOs.Requests;

public record CreateOrganizationRequest
{
    public string OrganizationName { get; init; } = string.Empty;
    public string? OrganizationTaxId { get; init; }
    public string? OrganizationAddress { get; init; }
    public string? OrganizationPhone { get; init; }
    public string? OrganizationEmail { get; init; }
    public string? OrganizationBankAccount { get; init; }
}

public record UpdateOrganizationRequest
{
    public int Id { get; init; }
    public string? OrganizationName { get; init; }
    public string? OrganizationTaxId { get; init; }
    public string? OrganizationAddress { get; init; }
    public string? OrganizationPhone { get; init; }
    public string? OrganizationEmail { get; init; }
    public string? OrganizationBankAccount { get; init; }
}

public record DeleteOrganizationRequest
{
    public int Id { get; init; }
}

public record GetOrganizationWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}
