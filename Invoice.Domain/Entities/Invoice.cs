using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? FormNumber { get; set; }
    public string? Serial { get; set; }

    // Tenant / Organization
    public int TenantOrganizationId { get; set; }
    public virtual Organization TenantOrganization { get; set; } = null!;

    // Issued by user
    public int IssuedByUserId { get; set; }
    public virtual User IssuedByUser { get; set; } = null!;

    // Seller info
    public string? SellerName { get; set; }
    public string? SellerTaxId { get; set; }
    public string? SellerAddress { get; set; }
    public string? SellerPhone { get; set; }
    public string? SellerEmail { get; set; }

    // Customer info
    public string? CustomerName { get; set; }
    public string? CustomerTaxId { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }

    public string? Status { get; set; }
    public DateTime? IssuedDate { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string? Currency { get; set; }
    public string? Note { get; set; }

    // Batch
    public int? BatchId { get; set; }
    public virtual InvoiceBatch? Batch { get; set; }

    // Blockchain related
    public string? ImmutableHash { get; set; }
    public string? Cid { get; set; }
    public string? CidHash { get; set; }
    public string? MerkleProof { get; set; }

    // Lines
    public virtual ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
}
