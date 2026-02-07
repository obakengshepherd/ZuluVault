using MediatR;
using ZuluVault.Application.DTOs;

namespace ZuluVault.Application.Features.Transactions.Queries;

public class GetTransactionHistoryQuery : IRequest<List<TransactionHistoryDto>>
{
    public Guid WalletId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetTransactionByIdQuery : IRequest<TransactionDto?>
{
    public Guid TransactionId { get; set; }
}
