using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Responses;

public class OrganizationResponse : IMapFrom<Organization>
{
    public int Id { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string? OrganizationTaxId { get; set; }
    public string? OrganizationAddress { get; set; }
    public string? OrganizationPhone { get; set; }
    public string? OrganizationEmail { get; set; }
    public string? OrganizationBankAccount { get; set; }

    public int? UserId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
