using Microsoft.EntityFrameworkCore;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Transaction> AddAsync(Transaction entity, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<Transaction> UpdateAsync(Transaction entity, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Update(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await GetByIdAsync(id, cancellationToken);
        if (transaction == null) return false;

        _context.Transactions.Remove(transaction);
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Transaction?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Reference == reference, cancellationToken);
    }

    public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking()
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.TransactionTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetWalletTransactionsAsync(Guid walletId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking()
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.TransactionTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking()
            .Where(t => t.Status == "Pending")
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetFailedTransactionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions.AsNoTracking()
            .Where(t => t.Status == "Failed")
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetDailyTransferAmountAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Transactions.AsNoTracking()
            .Where(t => t.WalletId == walletId 
                && t.Status == "Completed" 
                && (t.Type == "P2PTransfer" || t.Type == "Withdrawal")
                && t.TransactionTime.Date == today)
            .SumAsync(t => t.Amount, cancellationToken);
    }
}
