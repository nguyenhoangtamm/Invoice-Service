using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class InvoiceAttachment : BaseAuditableEntity
{
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long Size { get; set; }
    public string Path { get; set; } = default!; // where file is stored (relative path or url)
}
