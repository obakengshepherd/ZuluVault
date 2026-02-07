using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Transactions.Commands;
using ZuluVault.Application.Features.Transactions.Queries;

namespace ZuluVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(IMediator mediator, ILogger<TransactionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get transaction history for wallet
    /// </summary>
    [HttpGet("history/{walletId}")]
    public async Task<ActionResult<List<TransactionHistoryDto>>> GetTransactionHistory(
        Guid walletId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetTransactionHistoryQuery
        {
            WalletId = walletId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var transactions = await _mediator.Send(query);
        return Ok(transactions);
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(Guid id)
    {
        var query = new GetTransactionByIdQuery { TransactionId = id };
        var transaction = await _mediator.Send(query);

        if (transaction == null)
            return NotFound("Transaction not found");

        return Ok(transaction);
    }

    /// <summary>
    /// Initiate peer-to-peer transfer
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<InitiateTransferResponse>> Transfer([FromBody] InitiateTransferDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var command = new InitiatePeerToPeerTransferCommand
        {
            FromWalletId = request.FromWalletId,
            ToWalletId = request.ToWalletId,
            Amount = request.Amount,
            Description = request.Description,
            IdempotencyKey = request.IdempotencyKey,
            PerformedBy = userId.ToString()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Initiate deposit
    /// </summary>
    [HttpPost("deposit")]
    public async Task<ActionResult<InitiateDepositResponse>> Deposit([FromBody] InitiateDepositDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var command = new InitiateDepositCommand
        {
            WalletId = request.WalletId,
            Amount = request.Amount,
            Reference = request.Reference,
            IdempotencyKey = request.IdempotencyKey,
            PerformedBy = userId.ToString()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Initiate withdrawal
    /// </summary>
    [HttpPost("withdraw")]
    public async Task<ActionResult<InitiateWithdrawalResponse>> Withdraw([FromBody] InitiateDepositDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var command = new InitiateWithdrawalCommand
        {
            WalletId = request.WalletId,
            Amount = request.Amount,
            Reference = request.Reference,
            IdempotencyKey = request.IdempotencyKey,
            PerformedBy = userId.ToString()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;

        return Guid.Empty;
    }
}
