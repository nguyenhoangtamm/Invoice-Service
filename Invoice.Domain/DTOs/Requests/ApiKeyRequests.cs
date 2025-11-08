namespace Invoice.Domain.DTOs.Requests;

public record CreateApiKeyRequest
{
    public string Name { get; init; } = string.Empty;
    public int OrganizationId { get; init; }
}

public record UpdateApiKeyRequest
{
    public string? Name { get; init; }
    public bool? Active { get; init; }
}

public record GetApiKeysQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int? OrganizationId { get; init; }
}
