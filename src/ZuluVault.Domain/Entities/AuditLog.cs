namespace ZuluVault.Domain.Entities;

/// <summary>
/// Immutable audit log for compliance and fraud detection
/// </summary>
public class AuditLog
{
    public Guid Id { get; private set; }
    public string Action { get; private set; } // e.g., "WalletCreated", "TransferInitiated", "LoginAttempt"
    public string EntityType { get; private set; } // "Wallet", "Transaction", "User"
    public string EntityId { get; private set; }
    public Guid? UserId { get; private set; }
    public string PerformedBy { get; private set; } // User ID or "System"
    public string? PerformedByIp { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Details { get; private set; } // JSON serialized details
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    public AuditLog() { }

    public AuditLog(string action, string entityType, string entityId, string performedBy)
    {
        Id = Guid.NewGuid();
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        PerformedBy = performedBy;
        Timestamp = DateTime.UtcNow;
        Details = string.Empty;
        IsSuccess = true;
    }

    public static AuditLog Create(
        string action,
        string entityType,
        string entityId,
        string performedBy,
        string? oldValue = null,
        string? newValue = null,
        string? details = null,
        string? ip = null,
        Guid? userId = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            PerformedBy = performedBy,
            OldValue = oldValue,
            NewValue = newValue,
            Details = details ?? string.Empty,
            PerformedByIp = ip,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            IsSuccess = true
        };
    }

    public static AuditLog CreateFailure(
        string action,
        string entityType,
        string entityId,
        string performedBy,
        string errorMessage,
        string? ip = null,
        Guid? userId = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            PerformedBy = performedBy,
            Details = string.Empty,
            PerformedByIp = ip,
            UserId = userId ?? Guid.Empty,
            Timestamp = DateTime.UtcNow,
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}