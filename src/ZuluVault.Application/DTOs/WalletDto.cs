namespace ZuluVault.Application.DTOs;

public class WalletDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string WalletNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string Status { get; set; } = string.Empty;
    public decimal DailyTransferLimit { get; set; }
    public decimal RemainingDailyLimit { get; set; }
    public bool IsLocked { get; set; }
    public string? LockedReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWalletDto
{
    public Guid UserId { get; set; }
    public decimal DailyTransferLimit { get; set; } = 50000m;
}

public class WalletSummaryDto
{
    public Guid Id { get; set; }
    public string WalletNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string Status { get; set; } = string.Empty;
}
