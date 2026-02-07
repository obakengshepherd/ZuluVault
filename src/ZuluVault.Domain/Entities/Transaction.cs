using System;
using ZuluVault.Domain.Common;
using ZuluVault.Domain.Entities.Wallet;

namespace ZuluVault.Domain.Entities;

/// <summary>
/// Transaction entity representing a monetary transaction
/// </summary>
public class Transaction : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid WalletId { get; private set; }
    public decimal Amount { get; private set; }
    public string Type { get; private set; } // "Deposit", "Withdrawal", "P2PTransfer", "Reversal"
    public string Description { get; private set; }
    public string Reference { get; private set; } // Unique reference for idempotency
    public string Status { get; private set; } // "Pending", "Completed", "Failed", "Reversed"
    public string? FailureReason { get; private set; }
    public Guid? RelatedTransactionId { get; private set; } // For linking transfer pairs
    public DateTime TransactionTime { get; private set; }
    public bool IsIdempotent { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;

    // For peer-to-peer transfers
    public Guid? RecipientWalletId { get; private set; }

    // Navigation properties
    public virtual Wallet.Wallet? Wallet { get; set; }
    public virtual Wallet.Wallet? RecipientWallet { get; set; }

    public Transaction() { }

    public Transaction(
        Guid walletId,
        decimal amount,
        string type,
        string description,
        string reference,
        string idempotencyKey)
    {
        Id = Guid.NewGuid();
        WalletId = walletId;
        Amount = amount;
        Type = type;
        Description = description;
        Reference = reference;
        Status = "Pending";
        TransactionTime = DateTime.UtcNow;
        IsIdempotent = !string.IsNullOrEmpty(idempotencyKey);
        IdempotencyKey = idempotencyKey ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark transaction as completed
    /// </summary>
    public void MarkComplete(string completedBy)
    {
        Status = "Completed";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = completedBy;
    }

    /// <summary>
    /// Mark transaction as failed
    /// </summary>
    public void MarkFailed(string failureReason, string failedBy)
    {
        Status = "Failed";
        FailureReason = failureReason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = failedBy;
    }

    /// <summary>
    /// Reverse transaction (for refunds)
    /// </summary>
    public void Reverse(Guid reversalTransactionId, string reversedBy)
    {
        Status = "Reversed";
        RelatedTransactionId = reversalTransactionId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = reversedBy;
    }

    /// <summary>
    /// Set recipient wallet for P2P transfers
    /// </summary>
    public void SetRecipientWallet(Guid recipientWalletId)
    {
        if (Type != "P2PTransfer")
            throw new InvalidWalletOperationException("Recipient can only be set for P2P transfers");

        RecipientWalletId = recipientWalletId;
    }
}