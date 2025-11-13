using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Invoice.Domain.DTOs.Responses;

public record InvoiceLineResponse : IMapFrom<Invoice.Domain.Entities.InvoiceLine>
{
    public int Id { get; init; }
    public int InvoiceId { get; init; }
    public int LineNumber { get; init; }
    public string? Description { get; init; }
    public decimal Quantity { get; init; }
    public string? Unit { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Discount { get; init; }
    public decimal TaxRate { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal LineTotal { get; init; }
}

public record InvoiceResponse : IMapFrom<Invoice.Domain.Entities.Invoice>
{
    public int Id { get; init; }
    public string? InvoiceNumber { get; init; }
    public string? FormNumber { get; init; }
    public string? Serial { get; init; }

    public int OrganizationId { get; init; }
    public int? IssuedByUserId { get; init; }

    // Seller
    public string? SellerName { get; init; }
    public string? SellerTaxId { get; init; }
    public string? SellerAddress { get; init; }
    public string? SellerPhone { get; init; }
    public string? SellerEmail { get; init; }

    // Customer
    public string? CustomerName { get; init; }
    public string? CustomerTaxId { get; init; }
    public string? CustomerAddress { get; init; }
    public string? CustomerPhone { get; init; }
    public string? CustomerEmail { get; init; }

    public InvoiceStatus Status { get; init; }
    public DateTime? IssuedDate { get; init; }

    public decimal? SubTotal { get; init; }
    public decimal? TaxAmount { get; init; }
    public decimal? DiscountAmount { get; init; }
    public decimal? TotalAmount { get; init; }
    public string? Currency { get; init; }
    public string? Note { get; init; }

    public int? BatchId { get; init; }
    public string? ImmutableHash { get; init; }
    public string? Cid { get; init; }
    public string? CidHash { get; init; }
    public string? MerkleProof { get; init; }

    public List<InvoiceLineResponse> Lines { get; init; } = new List<InvoiceLineResponse>();
}
