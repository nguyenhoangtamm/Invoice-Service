namespace Invoice.Domain.Configurations;

public class BlockchainConfiguration
{
    public const string SectionName = "Blockchain";
    public string RpcUrl { get; set; } = string.Empty;
    public string ChainId { get; set; } = "11155111"; // Sepolia
    public string PrivateKey { get; set; } = string.Empty;
    public string? KmsEndpoint { get; set; }
    public string ContractAddress { get; set; } = string.Empty;
    public string ContractAbi { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 2000;
    public int ConfirmationBlocks { get; set; } = 3;
    public long GasLimit { get; set; } = 200000;
    public long MaxGasPrice { get; set; } = 50000000000; // 50 Gwei
    public int TimeoutMs { get; set; } = 120000;
}