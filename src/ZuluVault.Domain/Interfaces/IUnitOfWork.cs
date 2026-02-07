using ZuluVault.Domain.Entities;

namespace ZuluVault.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern for managing transactions across repositories
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    IWalletRepository Wallets { get; }
    ITransactionRepository Transactions { get; }
    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
