namespace ZuluVault.Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? FailureReason { get; set; }
    public Guid? RecipientWalletId { get; set; }
    public DateTime TransactionTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionHistoryDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime TransactionTime { get; set; }
}

public class InitiateTransferDto
{
    public Guid FromWalletId { get; set; }
    public Guid ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}

public class InitiateDepositDto
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}
