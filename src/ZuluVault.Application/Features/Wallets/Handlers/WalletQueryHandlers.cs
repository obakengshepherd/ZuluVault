using MediatR;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Wallets.Queries;
using ZuluVault.Domain.Entities.Wallet;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Application.Features.Wallets.Handlers;

public class GetWalletByUserIdQueryHandler : IRequestHandler<GetWalletByUserIdQuery, WalletDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWalletByUserIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletDto?> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet == null) return null;

        return MapToDto(wallet);
    }

    private WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            WalletNumber = wallet.WalletNumber,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            Status = wallet.Status,
            DailyTransferLimit = wallet.DailyTransferLimit,
            RemainingDailyLimit = wallet.RemainingDailyLimit,
            IsLocked = wallet.IsLocked,
            LockedReason = wallet.LockedReason,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };
    }
}

public class GetWalletByIdQueryHandler : IRequestHandler<GetWalletByIdQuery, WalletDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWalletByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletDto?> Handle(GetWalletByIdQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _unitOfWork.Wallets.GetByIdAsync(request.WalletId, cancellationToken);
        if (wallet == null) return null;

        return MapToDto(wallet);
    }

    private WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            WalletNumber = wallet.WalletNumber,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            Status = wallet.Status,
            DailyTransferLimit = wallet.DailyTransferLimit,
            RemainingDailyLimit = wallet.RemainingDailyLimit,
            IsLocked = wallet.IsLocked,
            LockedReason = wallet.LockedReason,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };
    }
}

public class GetAllWalletsQueryHandler : IRequestHandler<GetAllWalletsQuery, List<WalletSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllWalletsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<WalletSummaryDto>> Handle(GetAllWalletsQuery request, CancellationToken cancellationToken)
    {
        var wallets = await _unitOfWork.Wallets.GetAllAsync(cancellationToken);
        return wallets.Select(MapToSummaryDto).ToList();
    }

    private WalletSummaryDto MapToSummaryDto(Wallet wallet)
    {
        return new WalletSummaryDto
        {
            Id = wallet.Id,
            WalletNumber = wallet.WalletNumber,
            Balance = wallet.Balance,
            Currency = wallet.Currency,
            Status = wallet.Status
        };
    }
}
