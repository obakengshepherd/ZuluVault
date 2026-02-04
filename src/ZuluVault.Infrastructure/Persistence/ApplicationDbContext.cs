// Full ApplicationDbContext code with comments
// This is the EF Core DbContext for PostgreSQL.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZuluVault.Domain.Entities;

namespace ZuluVault.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }  // Assume Transaction entity exists
    public DbSet<AuditLog> AuditLogs { get; set; }  // Assume AuditLog entity exists

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure entities
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DailyTransferLimit).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RemainingDailyLimit).HasColumnType("decimal(18,2)");
            // Add indexes, relationships, etc.
        });
        
        // Similar for other entities
    }

    // Unit of Work methods
    public async Task BeginTransactionAsync() => await Database.BeginTransactionAsync();
    public async Task CommitTransactionAsync() => await Database.CommitTransactionAsync();
    public async Task RollbackTransactionAsync() => await Database.RollbackTransactionAsync();
}