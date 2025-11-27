using Invoice.Domain.Enums;

namespace Invoice.Domain.DTOs.Requests;

public record CreateInvoiceReportRequest
{
    public int InvoiceId { get; init; }
    public InvoiceReportReason Reason { get; init; }
    public string? Description { get; init; }
}

public record UpdateInvoiceReportRequest
{
    public int Id { get; init; }
    public InvoiceReportReason? Reason { get; init; }
    public string? Description { get; init; }
    public InvoiceReportStatus? Status { get; init; }
}

public record GetInvoiceReportWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? InvoiceId { get; set; }
    public int? ReportedByUserId { get; set; }
    public InvoiceReportStatus? Status { get; set; }
    public InvoiceReportReason? Reason { get; set; }
}

public record GetInvoiceReportByUserWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int UserId { get; set; }
    public InvoiceReportStatus? Status { get; set; }
}
