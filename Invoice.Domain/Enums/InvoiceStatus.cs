namespace Invoice.Domain.Enums
{
    public enum InvoiceStatus
    {
        Draft = 0,              // Bản nháp
        Uploaded = 1,           // Đã upload
        IpfsStored = 2,         // Đã lưu trên IPFS
        Batched = 3,            // Đã tạo batch
        BlockchainConfirmed = 4,// Đã xác nhận trên blockchain
        Finalized = 5,          // Hoàn tất
        Canceled = 6,           // Đã hủy
        IpfsFailed = 101,       // Upload IPFS thất bại
        BlockchainFailed = 102  // Ghi blockchain thất bại
    }
}
