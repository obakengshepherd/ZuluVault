using ZuluVault.Domain.Common;

namespace ZuluVault.Domain.Entities.Wallet;

/// <summary>
/// Wallet entity representing a user's digital wallet account
/// </summary>
public class Wallet : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string WalletNumber { get; private set; }
    public decimal Balance { get; private set; }
    public string Currency { get; private set; }
    public string Status { get; private set; }
    public decimal DailyTransferLimit { get; private set; }
    public decimal RemainingDailyLimit { get; private set; }
    public DateTime? LastLimitResetAt { get; private set; }
    public bool IsLocked { get; private set; }
    public string? LockedReason { get; private set; }
    public int FailedAuthAttempts { get; private set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public Wallet() { }

    public Wallet(Guid userId, string walletNumber, decimal dailyTransferLimit = 50000m)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        WalletNumber = walletNumber;
        Balance = 0m;
        Currency = "ZAR";
        Status = "Active";
        DailyTransferLimit = dailyTransferLimit;
        RemainingDailyLimit = dailyTransferLimit;
        IsLocked = false;
        FailedAuthAttempts = 0;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Debit amount from wallet (withdrawal/transfer)
    /// </summary>
    public void Debit(decimal amount, string performedBy)
    {
        if (amount <= 0)
            throw new InvalidWalletOperationException("Debit amount must be greater than zero.");
        
        if (IsLocked)
            throw new WalletLockedException($"Wallet is locked: {LockedReason}");
        
        if (Balance < amount)
            throw new InsufficientFundsException($"Insufficient funds. Available: {Balance}, Requested: {amount}");
        
        if (RemainingDailyLimit < amount)
            throw new DailyLimitExceededException($"Daily transfer limit exceeded. Remaining: {RemainingDailyLimit}, Requested: {amount}");

        Balance -= amount;
        RemainingDailyLimit -= amount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    /// <summary>
    /// Credit amount to wallet (deposit/transfer in)
    /// </summary>
    public void Credit(decimal amount, string performedBy)
    {
        if (amount <= 0)
            throw new InvalidWalletOperationException("Credit amount must be greater than zero.");
        
        if (Status != "Active")
            throw new InvalidWalletOperationException("Wallet is not in active status");

        Balance += amount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    /// <summary>
    /// Lock wallet for suspicious activity
    /// </summary>
    public void Lock(string reason, string performedBy)
    {
        IsLocked = true;
        LockedReason = reason;
        Status = "Locked";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    /// <summary>
    /// Unlock wallet
    /// </summary>
    public void Unlock(string performedBy)
    {
        IsLocked = false;
        LockedReason = null;
        Status = "Active";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    /// <summary>
    /// Reset daily transfer limit
    /// </summary>
    public void ResetDailyLimit(string performedBy)
    {
        RemainingDailyLimit = DailyTransferLimit;
        LastLimitResetAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    /// <summary>
    /// Increment failed auth attempts
    /// </summary>
    public void IncrementFailedAttempts()
    {
        FailedAuthAttempts++;
        if (FailedAuthAttempts >= 5)
            Lock("Too many failed authentication attempts", "System");
    }

    /// <summary>
    /// Reset failed auth attempts
    /// </summary>
    public void ResetFailedAttempts(string performedBy)
    {
        FailedAuthAttempts = 0;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    /// <summary>
    /// Suspend wallet
    /// </summary>
    public void Suspend(string reason, string performedBy)
    {
        Status = "Suspended";
        Lock(reason, performedBy);
    }

    /// <summary>
    /// Reactivate wallet
    /// </summary>
    public void Reactivate(string performedBy)
    {
        Status = "Active";
        Unlock(performedBy);
        ResetFailedAttempts(performedBy);
    }
}
