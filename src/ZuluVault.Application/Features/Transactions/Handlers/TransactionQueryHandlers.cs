using MediatR;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Transactions.Queries;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Application.Features.Transactions.Handlers;

public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, List<TransactionHistoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTransactionHistoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TransactionHistoryDto>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _unitOfWork.Transactions.GetWalletTransactionsAsync(
            request.WalletId,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        return transactions.Select(MapToDto).ToList();
    }

    private TransactionHistoryDto MapToDto(Transaction transaction)
    {
        return new TransactionHistoryDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            Description = transaction.Description,
            Status = transaction.Status,
            TransactionTime = transaction.TransactionTime
        };
    }
}

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTransactionByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(request.TransactionId, cancellationToken);
        if (transaction == null) return null;

        return MapToDto(transaction);
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
