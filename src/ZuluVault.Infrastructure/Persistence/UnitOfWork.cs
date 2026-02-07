using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for managing repository transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public IWalletRepository Wallets => _walletRepository;
    public ITransactionRepository Transactions => _transactionRepository;
    public IAuditLogRepository AuditLogs => _auditLogRepository;

    public UnitOfWork(
        ApplicationDbContext context,
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IAuditLogRepository auditLogRepository)
    {
        _context = context;
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
