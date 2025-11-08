using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Responses;

public class InvoiceLineResponse : IMapFrom<Invoice.Domain.Entities.InvoiceLine>
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int LineNumber { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

public class InvoiceResponse : IMapFrom<Invoice.Domain.Entities.Invoice>
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? FormNumber { get; set; }
    public string? Serial { get; set; }
    public int TenantOrganizationId { get; set; }
    public int IssuedByUserId { get; set; }

    public string? SellerName { get; set; }
    public string? SellerTaxId { get; set; }
    public string? SellerAddress { get; set; }
    public string? SellerPhone { get; set; }
    public string? SellerEmail { get; set; }

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

    public int? BatchId { get; set; }

    public List<InvoiceLineResponse> Lines { get; set; } = new();

    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
