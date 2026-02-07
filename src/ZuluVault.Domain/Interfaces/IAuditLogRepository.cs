using ZuluVault.Domain.Entities;

namespace ZuluVault.Domain.Interfaces;

/// <summary>
/// Repository interface for Audit Log operations
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByEntityIdAsync(string entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetFailedAttemptsAsync(Guid userId, int hours = 24, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAuditTrailAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
