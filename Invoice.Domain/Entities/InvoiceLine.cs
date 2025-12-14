using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class InvoiceLine : BaseAuditableEntity
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public int LineNumber { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}
