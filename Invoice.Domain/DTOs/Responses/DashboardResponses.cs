namespace Invoice.Domain.DTOs.Responses;

public class RevenueChartDataDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public string Period { get; set; } = string.Empty;
}

public class TopCustomerDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int InvoiceCount { get; set; }
    public string? LastPurchase { get; set; }
}

public class RecentActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // invoice_created, payment_received, customer_added, invoice_updated
    public string Description { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? EntityId { get; set; }
}
