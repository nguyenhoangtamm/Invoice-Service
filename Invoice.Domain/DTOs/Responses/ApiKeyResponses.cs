using Invoice.Domain.Common.Mappings;
using System;

namespace Invoice.Domain.DTOs.Responses;

public record ApiKeyResponse : IMapFrom<Invoice.Domain.Entities.ApiKey>
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public bool Active { get; init; }
    public DateTime? RevokedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public int OrganizationId { get; init; }
}

public record CreateApiKeyResponse
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string ApiKey { get; init; } = default!; // The actual API key value
    public bool Active { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public int OrganizationId { get; init; }
    public DateTime CreatedDate { get; init; }
}
