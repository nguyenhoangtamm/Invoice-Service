using Invoice.Domain.Common.Mappings;
using System;

namespace Invoice.Domain.DTOs.Responses;

public record ApiKeyResponse : IMapFrom<Invoice.Domain.Entities.ApiKey>
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public bool Active { get; init; }
    public DateTime? RevokedAt { get; init; }
    public int OrganizationId { get; init; }
}
