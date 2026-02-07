using Microsoft.EntityFrameworkCore;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<AuditLog> AddAsync(AuditLog entity, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<AuditLog> UpdateAsync(AuditLog entity, CancellationToken cancellationToken = default)
    {
        _context.AuditLogs.Update(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var auditLog = await GetByIdAsync(id, cancellationToken);
        if (auditLog == null) return false;

        _context.AuditLogs.Remove(auditLog);
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityIdAsync(string entityId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.AsNoTracking()
            .Where(a => a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.AsNoTracking()
            .Where(a => a.Action == action)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetFailedAttemptsAsync(Guid userId, int hours = 24, CancellationToken cancellationToken = default)
    {
        var timeThreshold = DateTime.UtcNow.AddHours(-hours);
        return await _context.AuditLogs.AsNoTracking()
            .Where(a => a.UserId == userId 
                && !a.IsSuccess 
                && a.Timestamp >= timeThreshold)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetAuditTrailAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs.AsNoTracking()
            .Where(a => a.Timestamp >= from && a.Timestamp <= to)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
