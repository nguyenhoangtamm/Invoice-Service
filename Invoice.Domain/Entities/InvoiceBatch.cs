using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class InvoiceBatch : BaseAuditableEntity
{
    public string? BatchId { get; set; }
    public int Count { get; set; }
    public string? MerkleRoot { get; set; }
    public string? BatchCid { get; set; }

    // Batch processing / blockchain fields
    public int? Status { get; set; }
    public string? TxHash { get; set; }
    public long? BlockNumber { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
