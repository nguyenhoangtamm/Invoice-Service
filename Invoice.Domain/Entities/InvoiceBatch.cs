using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class InvoiceBatch : BaseAuditableEntity
{
    public string BatchId { get; set; } = default!; // external id
    public int Count { get; set; }
    public string? MerkleRoot { get; set; }
    public string? BatchCid { get; set; }
    public int Status { get; set; }
    public string? TxHash { get; set; }
    public long? BlockNumber { get; set; }
    public DateTime? ConfirmedAt { get; set; }

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
