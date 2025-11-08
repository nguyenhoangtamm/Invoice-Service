using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Responses;

public class InvoiceBatchResponse : IMapFrom<InvoiceBatch>
{
    public int Id { get; set; }
    public string? BatchId { get; set; }
    public int Count { get; set; }
    public string? MerkleRoot { get; set; }
    public string? BatchCid { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
