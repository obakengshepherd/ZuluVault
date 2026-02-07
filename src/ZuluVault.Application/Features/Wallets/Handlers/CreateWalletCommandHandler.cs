using MediatR;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Wallets.Commands;
using ZuluVault.Domain.Common;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Entities.Wallet;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Application.Features.Wallets.Handlers;

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, CreateWalletResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateWalletCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateWalletResponse> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate unique wallet number
            var walletNumber = GenerateWalletNumber();

            // Check if wallet already exists for user
            var existingWallet = await _unitOfWork.Wallets.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingWallet != null)
            {
                return new CreateWalletResponse
                {
                    Success = false,
                    Message = "User already has a wallet"
                };
            }

            // Create wallet
            var wallet = new Wallet(request.UserId, walletNumber, request.DailyTransferLimit);
            wallet.CreatedBy = request.UserId.ToString();

            await _unitOfWork.Wallets.AddAsync(wallet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Log audit
            var auditLog = AuditLog.Create(
                "WalletCreated",
                "Wallet",
                wallet.Id.ToString(),
                request.UserId.ToString(),
                details: $"Wallet number: {walletNumber}",
                userId: request.UserId
            );

            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateWalletResponse
            {
                Success = true,
                Message = "Wallet created successfully",
                Wallet = MapToDto(wallet)
            };
        }
        catch (Exception ex)
        {
            return new CreateWalletResponse
            {
                Success = false,
                Message = $"Error creating wallet: {ex.Message}"
            };
        }
    }

    private string GenerateWalletNumber()
    {
        return $"ZV{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N").Substring(0, 8)}".ToUpper();
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
