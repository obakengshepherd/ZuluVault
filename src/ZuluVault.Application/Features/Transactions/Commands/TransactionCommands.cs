using MediatR;
using ZuluVault.Application.DTOs;

namespace ZuluVault.Application.Features.Transactions.Commands;

public class InitiatePeerToPeerTransferCommand : IRequest<InitiateTransferResponse>
{
    public Guid FromWalletId { get; set; }
    public Guid ToWalletId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty; // For idempotency
    public string PerformedBy { get; set; } = string.Empty;
}

public class InitiateTransferResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TransactionDto? Transaction { get; set; }
}

public class InitiateDepositCommand : IRequest<InitiateDepositResponse>
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}

public class InitiateDepositResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TransactionDto? Transaction { get; set; }
}

public class InitiateWithdrawalCommand : IRequest<InitiateWithdrawalResponse>
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}

public class InitiateWithdrawalResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TransactionDto? Transaction { get; set; }
}
