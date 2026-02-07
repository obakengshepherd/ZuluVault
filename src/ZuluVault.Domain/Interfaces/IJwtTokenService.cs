namespace ZuluVault.Domain.Interfaces;

/// <summary>
/// JWT token service interface
/// </summary>
public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, string? role = null);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Task<string> GetUserIdFromTokenAsync(string token);
}
