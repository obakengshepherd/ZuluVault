// Full Wallet entity code with comments
// This defines the Wallet domain entity with business logic.

using System;
using ZuluVault.Domain.Common;  // Assume common base entity

namespace ZuluVault.Domain.Entities;

public class Wallet : AuditableEntity  // Inherits from base for audit fields
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string WalletNumber { get; private set; } = string.Empty;
    public decimal Balance { get; private set; } = 0.00m;
    public string Currency { get; private set; } = "ZAR";
    public string Status { get; private set; } = "Active";
    public decimal DailyTransferLimit { get; private set; } = 50000.00m;
    public decimal RemainingDailyLimit { get; private set; } = 50000.00m;
    public bool IsLocked { get; private set; } = false;
    public string? LockedReason { get; private set; }

    // Domain logic methods
    public void Debit(decimal amount, string description, string performedBy)
    {
        if (amount <= 0) throw new ArgumentException("Debit amount must be greater than zero.");
        if (IsLocked) throw new InvalidOperationException($"Wallet is locked: {LockedReason}");
        if (Balance < amount) throw new InvalidOperationException("Insufficient funds.");
        
        Balance -= amount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    public void Credit(decimal amount, string description, string performedBy)
    {
        if (amount <= 0) throw new ArgumentException("Credit amount must be greater than zero.");
        
        Balance += amount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = performedBy;
    }

    // Additional entities would be added similarly (e.g., Transaction, AuditLog)
    // For brevity, only Wallet is shown; extend as needed.
}