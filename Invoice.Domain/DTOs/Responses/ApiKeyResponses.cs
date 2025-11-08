using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Responses;

public class ApiKeyResponse : IMapFrom<ApiKey>
{
    public int Id { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int OrganizationId { get; set; }
    public bool Active { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
