using ZuluVault.Domain.Entities;

namespace ZuluVault.Domain.Interfaces;

/// <summary>
/// Repository interface for Transaction operations
/// </summary>
public interface ITransactionRepository : IRepository<Transaction>
{
    Task<Transaction?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetWalletTransactionsAsync(Guid walletId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetFailedTransactionsAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetDailyTransferAmountAsync(Guid walletId, CancellationToken cancellationToken = default);
}
