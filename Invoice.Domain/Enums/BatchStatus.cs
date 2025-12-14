namespace Invoice.Domain.Enums
{
    public enum BatchStatus
    {
        Initial = 1, // initial
        BlockchainConfirmed = 2, // blockchain_confirmed
        BlockchainFailed = 101 // blockchain_failed
    }
}
