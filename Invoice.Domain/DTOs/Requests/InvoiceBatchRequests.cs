using System;
using System.Collections.Generic;
using Invoice.Domain.Enums;

namespace Invoice.Domain.DTOs.Requests;

public record CreateInvoiceBatchRequest
{
    public string BatchId { get; init; } = string.Empty;
    public int Count { get; init; }
    public string? MerkleRoot { get; init; }
    public string? BatchCid { get; init; }
    public BatchStatus Status { get; init; }
    public string? TxHash { get; init; }
    public long? BlockNumber { get; init; }
    public DateTime? ConfirmedAt { get; init; }

    // New: list of invoice ids to include
    public List<int> InvoiceIds { get; init; } = new();
}

public record UpdateInvoiceBatchRequest
{
    public int Id { get; init; }
    public string? BatchId { get; init; }
    public int? Count { get; init; }
    public string? MerkleRoot { get; init; }
    public string? BatchCid { get; init; }
    public BatchStatus? Status { get; init; }
    public string? TxHash { get; init; }
    public long? BlockNumber { get; init; }
    public DateTime? ConfirmedAt { get; init; }
}

public record DeleteInvoiceBatchRequest
{
    public int Id { get; init; }
}

public record GetInvoiceBatchWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}
