namespace Invoice.Domain.DTOs.Requests;

public record CreateInvoiceBatchRequest
{
    public string? BatchId { get; init; }
    public int Count { get; init; }
    public string? MerkleRoot { get; init; }
    public string? BatchCid { get; init; }
}

public record UpdateInvoiceBatchRequest
{
    public string? BatchId { get; init; }
    public int? Count { get; init; }
    public string? MerkleRoot { get; init; }
    public string? BatchCid { get; init; }
}

public record GetInvoiceBatchesQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
