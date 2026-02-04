using System;
using ZuluVault.Domain.Common;

namespace ZuluVault.Domain.Entities;

public class Transaction : AuditableEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid WalletId { get; set; }
    public Wallet? Wallet { get; set; }   // Navigation property (optional for now)

    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;           // "Credit", "Debit", "TransferIn", "TransferOut"
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;      // e.g. transfer ID, payment reference
    public string Status { get; set; } = "Completed";          // Completed, Failed, Pending, Reversed
    public string? FailureReason { get; set; }

    public Guid? RelatedTransactionId { get; set; }            // for linking transfer pairs
}