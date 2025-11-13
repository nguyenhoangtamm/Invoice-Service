namespace Invoice.Domain.DTOs.Requests;

public record InvoiceLineRequest
{
    public int LineNumber { get; init; }
    public string? Description { get; init; }
    public string? Unit { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Discount { get; init; }
    public decimal TaxRate { get; init; }
}

public record CreateInvoiceRequest
{
    public string InvoiceNumber { get; init; } = string.Empty;
    public string? FormNumber { get; init; }
    public string? Serial { get; init; }
    public int TenantOrganizationId { get; init; }
    public int IssuedByUserId { get; init; }

    public string? SellerName { get; init; }
    public string? SellerTaxId { get; init; }
    public string? SellerAddress { get; init; }
    public string? SellerPhone { get; init; }
    public string? SellerEmail { get; init; }

    public string? CustomerName { get; init; }
    public string? CustomerTaxId { get; init; }
    public string? CustomerAddress { get; init; }
    public string? CustomerPhone { get; init; }
    public string? CustomerEmail { get; init; }

    public int Status { get; init; }
    public DateTime? IssuedDate { get; init; }

    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }

    public string? Currency { get; init; }
    public string? Note { get; init; }

    public int? BatchId { get; init; }

    public List<InvoiceLineRequest>? Lines { get; init; }
}

public record UpdateInvoiceRequest
{
    public string? InvoiceNumber { get; init; }
    public string? FormNumber { get; init; }
    public string? Serial { get; init; }
    public string? Status { get; init; }
    public DateTime? IssuedDate { get; init; }
    public decimal? Subtotal { get; init; }
    public decimal? TaxAmount { get; init; }
    public decimal? DiscountAmount { get; init; }
    public decimal? TotalAmount { get; init; }
    public string? Currency { get; init; }
    public string? Note { get; init; }
    public int? BatchId { get; init; }
}

public record GetInvoicesQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public int? OrganizationId { get; init; }
    public string? Keyword { get; init; }
}
