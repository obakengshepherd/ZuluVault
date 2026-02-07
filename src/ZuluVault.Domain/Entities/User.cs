using Microsoft.AspNetCore.Identity;
using WalletEntity = ZuluVault.Domain.Entities.Wallet.Wallet;

namespace ZuluVault.Domain.Entities;

/// <summary>
/// User entity extending ASP.NET Identity IdentityUser for ZuluVault
/// </summary>
public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MobilePhone { get; set; }
    public string? IdNumber { get; set; } // South African ID
    public string UserType { get; set; } = "Individual"; // Individual, Business
    public bool IsKycVerified { get; set; }
    public DateTime? KycVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Status { get; set; } = "Active"; // Active, Suspended, Deactivated

    // Navigation properties
    public virtual ICollection<WalletEntity> Wallets { get; set; } = new List<WalletEntity>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public string GetFullName() => $"{FirstName} {LastName}".Trim();
}
