using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

    // Public lookup code
    public string? LookupCode { get; init; }

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

// IPFS Response Models
public record IpfsSellerInfoResponse
{
    [JsonPropertyName("sellerName")]
    public string? SellerName { get; init; }

    [JsonPropertyName("sellerTaxId")]
    public string? SellerTaxId { get; init; }

    [JsonPropertyName("sellerAddress")]
    public string? SellerAddress { get; init; }

    [JsonPropertyName("sellerPhone")]
    public string? SellerPhone { get; init; }

    [JsonPropertyName("sellerEmail")]
    public string? SellerEmail { get; init; }
}

public record IpfsCustomerInfoResponse
{
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; init; }

    [JsonPropertyName("customerTaxId")]
    public string? CustomerTaxId { get; init; }

    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; init; }

    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; init; }

    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; init; }
}

public record IpfsInvoiceDetailsResponse
{
    [JsonPropertyName("issueDate")]
    public DateTime IssueDate { get; init; }

    [JsonPropertyName("subTotal")]
    public decimal SubTotal { get; init; }

    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; init; }

    [JsonPropertyName("discountAmount")]
    public decimal DiscountAmount { get; init; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("note")]
    public string? Note { get; init; }
}

public record IpfsInvoiceLineResponse
{
    [JsonPropertyName("lineNumber")]
    public int LineNumber { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("unit")]
    public string? Unit { get; init; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; init; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; init; }

    [JsonPropertyName("discount")]
    public decimal Discount { get; init; }

    [JsonPropertyName("taxRate")]
    public decimal TaxRate { get; init; }

    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; init; }

    [JsonPropertyName("lineTotal")]
    public decimal LineTotal { get; init; }
}

public record IpfsMetadataResponse
{
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("version")]
    public string? Version { get; init; }
}

public record IpfsInvoiceResponse
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("invoiceNumber")]
    public string? InvoiceNumber { get; init; }

    [JsonPropertyName("formNumber")]
    public string? FormNumber { get; init; }

    [JsonPropertyName("serial")]
    public string? Serial { get; init; }

    [JsonPropertyName("tenantOrganizationId")]
    public int TenantOrganizationId { get; init; }

    [JsonPropertyName("issuedByUserId")]
    public int? IssuedByUserId { get; init; }

    [JsonPropertyName("sellerInfo")]
    public IpfsSellerInfoResponse? SellerInfo { get; init; }

    [JsonPropertyName("customerInfo")]
    public IpfsCustomerInfoResponse? CustomerInfo { get; init; }

    [JsonPropertyName("invoiceDetails")]
    public IpfsInvoiceDetailsResponse? InvoiceDetails { get; init; }

    [JsonPropertyName("lines")]
    public List<IpfsInvoiceLineResponse>? Lines { get; init; }

    [JsonPropertyName("metadata")]
    public IpfsMetadataResponse? Metadata { get; init; }
}
