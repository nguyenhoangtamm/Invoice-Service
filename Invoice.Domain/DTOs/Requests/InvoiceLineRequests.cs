namespace Invoice.Domain.DTOs.Requests;

public record CreateInvoiceLineRequest
{
    public int InvoiceId { get; init; }
    public int LineNumber { get; init; }
    public string? Name { get; init; }
    public string? Unit { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal? Discount { get; init; }
    public decimal? TaxRate { get; init; }
    public decimal? TaxAmount { get; init; }
    public decimal LineTotal { get; init; }
}

public record UpdateInvoiceLineRequest
{
    public int Id { get; init; }
    public int? InvoiceId { get; init; }
    public int? LineNumber { get; init; }
    public string? Name { get; init; }
    public string? Unit { get; init; }
    public decimal? Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public decimal? Discount { get; init; }
    public decimal? TaxRate { get; init; }
    public decimal? TaxAmount { get; init; }
    public decimal? LineTotal { get; init; }
}

public record DeleteInvoiceLineRequest
{
    public int Id { get; init; }
}

public record GetInvoiceLineWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}
