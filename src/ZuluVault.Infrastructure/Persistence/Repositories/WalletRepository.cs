using Microsoft.EntityFrameworkCore;
using ZuluVault.Domain.Entities.Wallet;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Infrastructure.Persistence.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly ApplicationDbContext _context;

    public WalletRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Wallet>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Wallet> AddAsync(Wallet entity, CancellationToken cancellationToken = default)
    {
        await _context.Wallets.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<Wallet> UpdateAsync(Wallet entity, CancellationToken cancellationToken = default)
    {
        _context.Wallets.Update(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var wallet = await GetByIdAsync(id, cancellationToken);
        if (wallet == null) return false;

        _context.Wallets.Remove(wallet);
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Wallet?> GetByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking()
            .FirstOrDefaultAsync(w => w.WalletNumber == walletNumber, cancellationToken);
    }

    public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Wallet>> GetByUserIdAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking()
            .Where(w => w.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Wallet>> GetLockedWalletsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking()
            .Where(w => w.IsLocked)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Wallet>> GetSuspendedWalletsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking()
            .Where(w => w.Status == "Suspended")
            .ToListAsync(cancellationToken);
    }
}
