using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Invoice.Domain.DTOs.Responses;

public record InvoiceLineResponse : IMapFrom<Invoice.Domain.Entities.InvoiceLine>
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int LineNumber { get; set; }
    public string? Name { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

public record InvoiceResponse : IMapFrom<Invoice.Domain.Entities.Invoice>
{
    public int Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? FormNumber { get; set; }
    public string? Serial { get; set; }

    // Public lookup code
    public string? LookupCode { get; set; }

    public int OrganizationId { get; set; }
    public int? IssuedByUserId { get; set; }

    // Seller
    public string? SellerName { get; set; }
    public string? SellerTaxId { get; set; }
    public string? SellerAddress { get; set; }
    public string? SellerPhone { get; set; }
    public string? SellerEmail { get; set; }

    // Customer
    public string? CustomerName { get; set; }
    public string? CustomerTaxId { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }

    public InvoiceStatus Status { get; set; }
    public DateTime? IssuedDate { get; set; }

    public decimal? SubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? Note { get; set; }

    public int? BatchId { get; set; }
    public string? ImmutableHash { get; set; }
    public string? Cid { get; set; }
    public string? CidHash { get; set; }
    public string? MerkleProof { get; set; }

    public List<InvoiceLineResponse> Lines { get; set; } = new List<InvoiceLineResponse>();
}

// IPFS Response Models
public record IpfsSellerInfoResponse
{
    [JsonPropertyName("sellerName")]
    public string? SellerName { get; set; }

    [JsonPropertyName("sellerTaxId")]
    public string? SellerTaxId { get; set; }

    [JsonPropertyName("sellerAddress")]
    public string? SellerAddress { get; set; }

    [JsonPropertyName("sellerPhone")]
    public string? SellerPhone { get; set; }

    [JsonPropertyName("sellerEmail")]
    public string? SellerEmail { get; set; }
}

public record IpfsCustomerInfoResponse
{
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    [JsonPropertyName("customerTaxId")]
    public string? CustomerTaxId { get; set; }

    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; set; }

    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }

    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }
}

public record IpfsInvoiceDetailsResponse
{
    [JsonPropertyName("issueDate")]
    public DateTime IssueDate { get; set; }

    [JsonPropertyName("subTotal")]
    public decimal SubTotal { get; set; }

    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; set; }

    [JsonPropertyName("discountAmount")]
    public decimal DiscountAmount { get; set; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }
}

public record IpfsInvoiceLineResponse
{
    [JsonPropertyName("lineNumber")]
    public int LineNumber { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    [JsonPropertyName("taxRate")]
    public decimal TaxRate { get; set; }

    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; set; }

    [JsonPropertyName("lineTotal")]
    public decimal LineTotal { get; set; }
}

public record IpfsMetadataResponse
{
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

public record IpfsInvoiceResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public string? InvoiceNumber { get; set; }

    [JsonPropertyName("formNumber")]
    public string? FormNumber { get; set; }

    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    [JsonPropertyName("tenantOrganizationId")]
    public int TenantOrganizationId { get; set; }

    [JsonPropertyName("issuedByUserId")]
    public int? IssuedByUserId { get; set; }

    [JsonPropertyName("sellerInfo")]
    public IpfsSellerInfoResponse? SellerInfo { get; set; }

    [JsonPropertyName("customerInfo")]
    public IpfsCustomerInfoResponse? CustomerInfo { get; set; }

    [JsonPropertyName("invoiceDetails")]
    public IpfsInvoiceDetailsResponse? InvoiceDetails { get; set; }

    [JsonPropertyName("lines")]
    public List<IpfsInvoiceLineResponse>? Lines { get; set; }

    [JsonPropertyName("metadata")]
    public IpfsMetadataResponse? Metadata { get; set; }
}

public record InvoiceLookUpResponse : IMapFrom<Invoice.Domain.Entities.Invoice>
{
    public int Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? FormNumber { get; set; }
    public string? Serial { get; set; }

    // Public lookup code
    public string? LookupCode { get; set; }

    public int OrganizationId { get; set; }
    public int? IssuedByUserId { get; set; }

    // Seller
    public string? SellerName { get; set; }
    public string? SellerTaxId { get; set; }
    public string? SellerAddress { get; set; }
    public string? SellerPhone { get; set; }
    public string? SellerEmail { get; set; }

    // Customer
    public string? CustomerName { get; set; }
    public string? CustomerTaxId { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }

    public InvoiceStatus Status { get; set; }
    public DateTime? IssuedDate { get; set; }

    public decimal? SubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? Note { get; set; }

    public int? BatchId { get; set; }
    public string? ImmutableHash { get; set; }
    public string? Cid { get; set; }
    public string? CidHash { get; set; }
    public string? MerkleProof { get; set; }

    public bool IsExactMatch { get; set; }
    public List<InvoiceLineResponse> Lines { get; set; } = new List<InvoiceLineResponse>();
}