using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Invoice.Domain.DTOs.Responses;

public record InvoiceBatchResponse : IMapFrom<Invoice.Domain.Entities.InvoiceBatch>
{
    public int Id { get; init; }
    public string? BatchId { get; init; }
    public int Count { get; init; }
    public string? MerkleRoot { get; init; }
    public string? BatchCid { get; init; }
    public BatchStatus Status { get; init; }
    public string? TxHash { get; init; }
    public long? BlockNumber { get; init; }
    public DateTime? ConfirmedAt { get; init; }
}
