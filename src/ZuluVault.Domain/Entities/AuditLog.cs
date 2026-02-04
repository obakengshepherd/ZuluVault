using System;

namespace ZuluVault.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Action { get; set; } = string.Empty;          // "WalletCreated", "TransferInitiated", "LoginFailed", etc.
    public string EntityType { get; set; } = string.Empty;      // "Wallet", "Transaction", "User", etc.
    public string EntityId { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;     // User ID or "System"
    public string? PerformedByIp { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Details { get; set; } = string.Empty;         // JSON or free text
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}