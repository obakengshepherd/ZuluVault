using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Entities.Wallet;

namespace ZuluVault.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>
{
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Wallet entity
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.Property(w => w.WalletNumber).HasMaxLength(50).IsRequired();
            entity.Property(w => w.Balance).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            entity.Property(w => w.Currency).HasMaxLength(3).HasDefaultValue("ZAR");
            entity.Property(w => w.Status).HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(w => w.DailyTransferLimit).HasColumnType("decimal(18,2)").HasDefaultValue(50000m);
            entity.Property(w => w.RemainingDailyLimit).HasColumnType("decimal(18,2)").HasDefaultValue(50000m);
            entity.Property(w => w.IsLocked).HasDefaultValue(false);
            entity.Property(w => w.LockedReason).HasMaxLength(500);
            entity.Property(w => w.FailedAuthAttempts).HasDefaultValue(0);

            // Indexes
            entity.HasIndex(w => w.UserId);
            entity.HasIndex(w => w.WalletNumber).IsUnique();
            entity.HasIndex(w => w.Status);

            // Relationships
            entity.HasOne(w => w.User)
                .WithMany(u => u.Wallets)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(t => t.Type).HasMaxLength(20).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.Property(t => t.Reference).HasMaxLength(100).IsRequired();
            entity.Property(t => t.Status).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(t => t.FailureReason).HasMaxLength(500);
            entity.Property(t => t.IdempotencyKey).HasMaxLength(100);
            entity.Property(t => t.IsIdempotent).HasDefaultValue(false);

            // Indexes
            entity.HasIndex(t => t.WalletId);
            entity.HasIndex(t => t.Reference).IsUnique();
            entity.HasIndex(t => t.IdempotencyKey).IsUnique(false);
            entity.HasIndex(t => t.Status);
            entity.HasIndex(t => t.TransactionTime);

            // Relationships
            entity.HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.RecipientWallet)
                .WithMany()
                .HasForeignKey(t => t.RecipientWalletId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Action).HasMaxLength(100).IsRequired();
            entity.Property(a => a.EntityType).HasMaxLength(50).IsRequired();
            entity.Property(a => a.EntityId).HasMaxLength(100).IsRequired();
            entity.Property(a => a.PerformedBy).HasMaxLength(100).IsRequired();
            entity.Property(a => a.PerformedByIp).HasMaxLength(45); // IPv6 max length
            entity.Property(a => a.Details).HasMaxLength(2000);
            entity.Property(a => a.OldValue).HasMaxLength(2000);
            entity.Property(a => a.NewValue).HasMaxLength(2000);
            entity.Property(a => a.ErrorMessage).HasMaxLength(1000);

            // Indexes for compliance queries
            entity.HasIndex(a => a.UserId);
            entity.HasIndex(a => a.EntityId);
            entity.HasIndex(a => a.Action);
            entity.HasIndex(a => a.Timestamp);
            entity.HasIndex(a => new { a.PerformedBy, a.Timestamp });
        });

        // Seed data for admin user and initial wallets
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed admin role
        var adminRoleId = Guid.NewGuid();
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>().HasData(
            new Microsoft.AspNetCore.Identity.IdentityRole<Guid> 
            { 
                Id = adminRoleId,
                Name = "Admin", 
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new Microsoft.AspNetCore.Identity.IdentityRole<Guid> 
            { 
                Id = Guid.NewGuid(),
                Name = "User", 
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<Domain.Common.AuditableEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}