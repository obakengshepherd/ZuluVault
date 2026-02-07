using MediatR;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Transactions.Commands;
using ZuluVault.Domain.Common;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Entities.Wallet;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Application.Features.Transactions.Handlers;

public class InitiatePeerToPeerTransferCommandHandler : IRequestHandler<InitiatePeerToPeerTransferCommand, InitiateTransferResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public InitiatePeerToPeerTransferCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InitiateTransferResponse> Handle(InitiatePeerToPeerTransferCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Idempotency check
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
            {
                var existingTransaction = await _unitOfWork.Transactions.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
                if (existingTransaction != null)
                {
                    return new InitiateTransferResponse
                    {
                        Success = true,
                        Message = "Transfer already processed",
                        Transaction = MapToDto(existingTransaction)
                    };
                }
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get wallets
            var fromWallet = await _unitOfWork.Wallets.GetByIdAsync(request.FromWalletId, cancellationToken);
            var toWallet = await _unitOfWork.Wallets.GetByIdAsync(request.ToWalletId, cancellationToken);

            if (fromWallet == null || toWallet == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return new InitiateTransferResponse
                {
                    Success = false,
                    Message = "One or both wallets not found"
                };
            }

            if (request.Amount <= 0)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return new InitiateTransferResponse
                {
                    Success = false,
                    Message = "Amount must be greater than zero"
                };
            }

            try
            {
                // Debit from source wallet
                fromWallet.Debit(request.Amount, request.PerformedBy);
                
                // Create transaction record
                var transaction = new Transaction(
                    request.FromWalletId,
                    request.Amount,
                    "P2PTransfer",
                    request.Description,
                    Guid.NewGuid().ToString("N"),
                    request.IdempotencyKey
                );
                transaction.SetRecipientWallet(request.ToWalletId);
                transaction.CreatedBy = request.PerformedBy;

                await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
                await _unitOfWork.Wallets.UpdateAsync(fromWallet, cancellationToken);

                // Credit to recipient wallet
                toWallet.Credit(request.Amount, request.PerformedBy);
                await _unitOfWork.Wallets.UpdateAsync(toWallet, cancellationToken);

                // Mark transaction as complete
                transaction.MarkComplete(request.PerformedBy);
                await _unitOfWork.Transactions.UpdateAsync(transaction, cancellationToken);

                // Audit log
                var auditLog = AuditLog.Create(
                    "P2PTransferCompleted",
                    "Transaction",
                    transaction.Id.ToString(),
                    request.PerformedBy,
                    details: $"From: {fromWallet.WalletNumber} To: {toWallet.WalletNumber} Amount: {request.Amount}",
                    userId: fromWallet.UserId
                );
                await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new InitiateTransferResponse
                {
                    Success = true,
                    Message = "Transfer completed successfully",
                    Transaction = MapToDto(transaction)
                };
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                // Log failure
                var auditLog = AuditLog.CreateFailure(
                    "P2PTransferFailed",
                    "Transaction",
                    request.FromWalletId.ToString(),
                    request.PerformedBy,
                    ex.Message,
                    userId: fromWallet?.UserId
                );
                await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new InitiateTransferResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        catch (Exception ex)
        {
            return new InitiateTransferResponse
            {
                Success = false,
                Message = $"Transfer failed: {ex.Message}"
            };
        }
    }

    private TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            WalletId = transaction.WalletId,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Reference = transaction.Reference,
            Status = transaction.Status,
            FailureReason = transaction.FailureReason,
            RecipientWalletId = transaction.RecipientWalletId,
            TransactionTime = transaction.TransactionTime,
            CreatedAt = transaction.CreatedAt
        };
    }
}

public class InitiateDepositCommandHandler : IRequestHandler<InitiateDepositCommand, InitiateDepositResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public InitiateDepositCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InitiateDepositResponse> Handle(InitiateDepositCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
            {
                var existingTransaction = await _unitOfWork.Transactions.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
                if (existingTransaction != null && existingTransaction.Status == "Completed")
                {
                    return new InitiateDepositResponse
                    {
                        Success = true,
                        Message = "Deposit already processed",
                        Transaction = MapToDto(existingTransaction)
                    };
                }
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var wallet = await _unitOfWork.Wallets.GetByIdAsync(request.WalletId, cancellationToken);
            if (wallet == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return new InitiateDepositResponse
                {
                    Success = false,
                    Message = "Wallet not found"
                };
            }

            wallet.Credit(request.Amount, request.PerformedBy);
            await _unitOfWork.Wallets.UpdateAsync(wallet, cancellationToken);

            var transaction = new Transaction(
                request.WalletId,
                request.Amount,
                "Deposit",
                "Deposit",
                request.Reference,
                request.IdempotencyKey
            );
            transaction.CreatedBy = request.PerformedBy;
            transaction.MarkComplete(request.PerformedBy);

            await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);

            var auditLog = AuditLog.Create(
                "DepositCompleted",
                "Wallet",
                wallet.Id.ToString(),
                request.PerformedBy,
                details: $"Amount: {request.Amount}",
                userId: wallet.UserId
            );
            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new InitiateDepositResponse
            {
                Success = true,
                Message = "Deposit completed successfully",
                Transaction = MapToDto(transaction)
            };
        }
        catch (Exception ex)
        {
            return new InitiateDepositResponse
            {
                Success = false,
                Message = $"Deposit failed: {ex.Message}"
            };
        }
    }

    private TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            WalletId = transaction.WalletId,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Reference = transaction.Reference,
            Status = transaction.Status,
            FailureReason = transaction.FailureReason,
            TransactionTime = transaction.TransactionTime,
            CreatedAt = transaction.CreatedAt
        };
    }
}

public class InitiateWithdrawalCommandHandler : IRequestHandler<InitiateWithdrawalCommand, InitiateWithdrawalResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public InitiateWithdrawalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InitiateWithdrawalResponse> Handle(InitiateWithdrawalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
            {
                var existingTransaction = await _unitOfWork.Transactions.GetByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);
                if (existingTransaction != null && existingTransaction.Status == "Completed")
                {
                    return new InitiateWithdrawalResponse
                    {
                        Success = true,
                        Message = "Withdrawal already processed",
                        Transaction = MapToDto(existingTransaction)
                    };
                }
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var wallet = await _unitOfWork.Wallets.GetByIdAsync(request.WalletId, cancellationToken);
            if (wallet == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return new InitiateWithdrawalResponse
                {
                    Success = false,
                    Message = "Wallet not found"
                };
            }

            try
            {
                wallet.Debit(request.Amount, request.PerformedBy);
                await _unitOfWork.Wallets.UpdateAsync(wallet, cancellationToken);

                var transaction = new Transaction(
                    request.WalletId,
                    request.Amount,
                    "Withdrawal",
                    "Withdrawal",
                    request.Reference,
                    request.IdempotencyKey
                );
                transaction.CreatedBy = request.PerformedBy;
                transaction.MarkComplete(request.PerformedBy);

                await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);

                var auditLog = AuditLog.Create(
                    "WithdrawalCompleted",
                    "Wallet",
                    wallet.Id.ToString(),
                    request.PerformedBy,
                    details: $"Amount: {request.Amount}",
                    userId: wallet.UserId
                );
                await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new InitiateWithdrawalResponse
                {
                    Success = true,
                    Message = "Withdrawal completed successfully",
                    Transaction = MapToDto(transaction)
                };
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                var auditLog = AuditLog.CreateFailure(
                    "WithdrawalFailed",
                    "Wallet",
                    wallet.Id.ToString(),
                    request.PerformedBy,
                    ex.Message,
                    userId: wallet.UserId
                );
                await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new InitiateWithdrawalResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        catch (Exception ex)
        {
            return new InitiateWithdrawalResponse
            {
                Success = false,
                Message = $"Withdrawal failed: {ex.Message}"
            };
        }
    }

    private TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            WalletId = transaction.WalletId,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Reference = transaction.Reference,
            Status = transaction.Status,
            FailureReason = transaction.FailureReason,
            TransactionTime = transaction.TransactionTime,
            CreatedAt = transaction.CreatedAt
        };
    }
}
