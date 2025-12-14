using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Domain.DTOs.Responses;

public record BlockchainResponse
{
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
}
public record UriResponse
{
    public List<CidDetailResponse> Cids { get; set; }
}
public record CidDetailResponse
{
    public int InvoiceId { get; set; }
    public string Cid { get; set; }

}