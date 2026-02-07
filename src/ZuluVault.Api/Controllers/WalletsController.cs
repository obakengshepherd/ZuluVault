using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Wallets.Commands;
using ZuluVault.Application.Features.Wallets.Queries;

namespace ZuluVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WalletsController> _logger;

    public WalletsController(IMediator mediator, ILogger<WalletsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's wallet
    /// </summary>
    [HttpGet("my-wallet")]
    public async Task<ActionResult<WalletDto>> GetMyWallet()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var query = new GetWalletByUserIdQuery { UserId = userId };
        var wallet = await _mediator.Send(query);

        if (wallet == null)
            return NotFound("Wallet not found");

        return Ok(wallet);
    }

    /// <summary>
    /// Get wallet by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WalletDto>> GetWallet(Guid id)
    {
        var query = new GetWalletByIdQuery { WalletId = id };
        var wallet = await _mediator.Send(query);

        if (wallet == null)
            return NotFound("Wallet not found");

        return Ok(wallet);
    }

    /// <summary>
    /// Get all wallets (admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<WalletSummaryDto>>> GetAllWallets()
    {
        var query = new GetAllWalletsQuery();
        var wallets = await _mediator.Send(query);
        return Ok(wallets);
    }

    /// <summary>
    /// Create wallet for user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateWalletResponse>> CreateWallet([FromBody] CreateWalletDto request)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var command = new CreateWalletCommand
        {
            UserId = userId,
            DailyTransferLimit = request.DailyTransferLimit
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetMyWallet), result.Wallet);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;

        return Guid.Empty;
    }
}
