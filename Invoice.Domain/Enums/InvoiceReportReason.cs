namespace Invoice.Domain.Enums
{
    public enum InvoiceReportReason
    {
        IncorrectDetails = 1,           // Thông tin sai lệch
        MissingInformation = 2,         // Thiếu thông tin
        FraudulentActivity = 3,         // Hoạt động gian lận
        Other = 4                       // Khác
    }
}
