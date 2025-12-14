using Invoice.Domain.Entities.Base;
using Invoice.Domain.Enums;

namespace Invoice.Domain.Entities;

public class InvoiceReport : BaseAuditableEntity
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public int ReportedByUserId { get; set; }
    public User ReportedByUser { get; set; } = default!;

    public InvoiceReportReason Reason { get; set; }
    public string? Description { get; set; }
    
    public InvoiceReportStatus Status { get; set; } = InvoiceReportStatus.Pending;
}
