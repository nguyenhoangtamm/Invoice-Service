using System;

namespace Invoice.Domain.DTOs.Requests;

public record CreateApiKeyRequest
{
    public string Key { get; init; } = string.Empty; // raw key provided/generated
    public string? Name { get; init; }
    public bool Active { get; init; } = true;
    public int OrganizationId { get; init; }
}

public record UpdateApiKeyRequest
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public bool? Active { get; init; }
    public DateTime? RevokedAt { get; init; }
    public int? OrganizationId { get; init; }
}

public record DeleteApiKeyRequest
{
    public int Id { get; init; }
}
