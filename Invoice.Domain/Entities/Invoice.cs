using Invoice.Domain.Entities.Base;
using Invoice.Domain.Enums;

namespace Invoice.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public string? InvoiceNumber { get; set; }
    public string? FormNumber { get; set; }
    public string? Serial { get; set; }

    // Lookup code used for public invoice lookup
    public string? LookupCode { get; set; }

    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = default!;

    public int? IssuedByUserId { get; set; }
    public User? IssuedByUser { get; set; }

    // Seller
    public string? SellerName { get; set; }
    public string? SellerTaxId { get; set; }
    public string? SellerAddress { get; set; }
    public string? SellerPhone { get; set; }
    public string? SellerEmail { get; set; }

    // Customer
    public string? CustomerName { get; set; }
    public string? CustomerTaxId { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }

    public InvoiceStatus Status { get; set; }
    public DateTime? IssuedDate { get; set; }

    // Amounts
    public decimal? SubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? Note { get; set; }

    // Batch & proofs
    public int? BatchId { get; set; }
    public InvoiceBatch? Batch { get; set; }
    public string? ImmutableHash { get; set; }
    public string? Cid { get; set; }
    public string? CidHash { get; set; }
    public string? MerkleProof { get; set; }

    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();

    // Attachments for the invoice (files uploaded by user)
    public ICollection<InvoiceAttachment> Attachments { get; set; } = new List<InvoiceAttachment>();
}
