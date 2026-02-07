using MediatR;
using ZuluVault.Application.DTOs;

namespace ZuluVault.Application.Features.Wallets.Commands;

public class CreateWalletCommand : IRequest<CreateWalletResponse>
{
    public Guid UserId { get; set; }
    public decimal DailyTransferLimit { get; set; } = 50000m;
}

public class CreateWalletResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public WalletDto? Wallet { get; set; }
}
