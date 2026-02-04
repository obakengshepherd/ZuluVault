// Full CreateTransferCommand code with comments
// This is a MediatR command for P2P transfer.

using MediatR;
using ZuluVault.Application.DTOs;  // Assume DTOs exist

namespace ZuluVault.Application.Features.Transfers.Commands;

public class CreateTransferCommand : IRequest<TransferDto>
{
    public string DestinationWalletNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}

// Handler
public class CreateTransferCommandHandler : IRequestHandler<CreateTransferCommand, TransferDto>
{
    // Inject repos, etc.
    private readonly IUnitOfWork _unitOfWork;  // Assume interface

    public CreateTransferCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TransferDto> Handle(CreateTransferCommand request, CancellationToken cancellationToken)
    {
        // Logic: Check idempotency, debit/credit, commit
        // Placeholder
        await Task.CompletedTask;
        return new TransferDto();  // Map and return
    }
}