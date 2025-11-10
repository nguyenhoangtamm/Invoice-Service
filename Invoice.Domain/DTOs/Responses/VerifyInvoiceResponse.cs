namespace Invoice.Domain.DTOs.Responses;

using Invoice.Domain.Entities;

public class VerifyInvoiceResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public Invoice OffChainInvoice { get; set; }
    public Invoice OnChainInvoice { get; set; }
}