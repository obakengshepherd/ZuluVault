using MediatR;
using ZuluVault.Application.DTOs;

namespace ZuluVault.Application.Features.Wallets.Queries;

public class GetWalletByUserIdQuery : IRequest<WalletDto?>
{
    public Guid UserId { get; set; }
}

public class GetWalletByIdQuery : IRequest<WalletDto?>
{
    public Guid WalletId { get; set; }
}

public class GetAllWalletsQuery : IRequest<List<WalletSummaryDto>>
{
}
