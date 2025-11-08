namespace Invoice.Domain.DTOs.Requests;

public record CreateOrganizationRequest
{
    public string OrganizationName { get; init; } = string.Empty;
    public string? OrganizationTaxId { get; init; }
    public string? OrganizationAddress { get; init; }
    public string? OrganizationPhone { get; init; }
    public string? OrganizationEmail { get; init; }
    public string? OrganizationBankAccount { get; init; }
    public int? UserId { get; init; }
}

public record UpdateOrganizationRequest
{
    public string? OrganizationName { get; init; }
    public string? OrganizationTaxId { get; init; }
    public string? OrganizationAddress { get; init; }
    public string? OrganizationPhone { get; init; }
    public string? OrganizationEmail { get; init; }
    public string? OrganizationBankAccount { get; init; }
}

public record GetOrganizationsQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Keyword { get; init; }
}
