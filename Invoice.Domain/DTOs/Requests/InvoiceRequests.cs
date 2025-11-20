using System;
using System.Collections.Generic;
using Invoice.Domain.Enums;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Requests;

public record CreateInvoiceRequest
{
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

    public List<CreateInvoiceLineRequest> Lines { get; init; }
}

public record UpdateInvoiceRequest
{
    public int Id { get; init; }
    public string? InvoiceNumber { get; init; }
    public string? FormNumber { get; init; }
    public string? Serial { get; init; }

    // Public lookup code
    public string? LookupCode { get; init; }

    public int? OrganizationId { get; init; }
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

    public InvoiceStatus? Status { get; init; }
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
}

public record DeleteInvoiceRequest
{
    public int Id { get; init; }
}

public record GetInvoiceWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int? OrganizationId { get; set; }
}
public record GetInvoiceLookUpWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public string Code { get; set; }
}

public record GetInvoiceByUserWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public int UserId { get; set; }
    public int? OrganizationId { get; set; }
}

// --- Merged action requests ---

public record StoreToIpfsRequest
{
    public int InvoiceId { get; init; }
    public string ImmutableHash { get; init; } = string.Empty;
    public string Cid { get; init; } = string.Empty;
    public string CidHash { get; init; } = string.Empty;
    public string? MerkleProof { get; init; }
}

public record CreateBatchFromInvoicesRequest
{
    public string? ExternalBatchId { get; init; }
    public List<int> InvoiceIds { get; init; } = new();
    public string? MerkleRoot { get; init; }
}

public record ConfirmBlockchainRequest
{
    public int BatchId { get; init; }
    public string TxHash { get; init; } = string.Empty;
    public long BlockNumber { get; init; }
    public bool Success { get; init; } = true; // if false -> mark failed statuses
}
