namespace Invoice.Domain.Enums
{
    public enum InvoiceReportStatus
    {
        Pending = 1,        // Đang chờ xử lý
        Reviewing = 2,      // Đang xem xét
        Resolved = 3,       // Đã giải quyết
        Rejected = 4,       // Bị từ chối
        Closed = 5          // Đã đóng
    }
}
