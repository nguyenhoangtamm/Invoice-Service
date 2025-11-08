using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class InvoiceLine : BaseAuditableEntity
{
    public int InvoiceId { get; set; }
    public virtual Invoice Invoice { get; set; } = null!;

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
