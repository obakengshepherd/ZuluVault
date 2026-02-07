using ZuluVault.Domain.Entities.Wallet;

namespace ZuluVault.Domain.Interfaces;

/// <summary>
/// Repository interface for Wallet operations
/// </summary>
public interface IWalletRepository : IRepository<Wallet>
{
    Task<Wallet?> GetByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Wallet>> GetByUserIdAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Wallet>> GetLockedWalletsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Wallet>> GetSuspendedWalletsAsync(CancellationToken cancellationToken = default);
}
