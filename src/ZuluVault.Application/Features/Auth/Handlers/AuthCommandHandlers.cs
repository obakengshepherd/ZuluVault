using MediatR;
using Microsoft.AspNetCore.Identity;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Auth.Commands;
using ZuluVault.Domain.Common;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Entities.Wallet;
using ZuluVault.Domain.Interfaces;

namespace ZuluVault.Application.Features.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(UserManager<User> userManager, IJwtTokenService jwtService, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Email already registered"
                };
            }

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = false,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MobilePhone = request.MobilePhone,
                UserType = request.UserType,
                IsActive = true,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResultDto
                {
                    Success = false,
                    Message = $"Registration failed: {errors}"
                };
            }

            // Assign User role
            await _userManager.AddToRoleAsync(user, "User");

            // Create wallet for user
            var walletNumber = GenerateWalletNumber();
            var wallet = new Wallet(user.Id, walletNumber, 50000m);
            wallet.CreatedBy = user.Id.ToString();

            await _unitOfWork.Wallets.AddAsync(wallet, cancellationToken);

            // Log audit
            var auditLog = AuditLog.Create(
                "UserRegistered",
                "User",
                user.Id.ToString(),
                "System",
                details: $"Email: {user.Email}, UserType: {user.UserType}",
                userId: user.Id
            );
            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new AuthResultDto
            {
                Success = true,
                Message = "Registration successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = $"Registration failed: {ex.Message}"
            };
        }
    }

    private string GenerateWalletNumber()
    {
        return $"ZV{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N").Substring(0, 8)}".ToUpper();
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserType = user.UserType,
            IsKycVerified = user.IsKycVerified,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(UserManager<User> userManager, IJwtTokenService jwtService, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Log failed attempt
                var failureLog = AuditLog.CreateFailure(
                    "LoginFailed",
                    "User",
                    request.Email,
                    "System",
                    "User not found"
                );
                await _unitOfWork.AuditLogs.AddAsync(failureLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Check password
            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                // Log failed attempt and increment failed attempts
                var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(user.Id, cancellationToken);
                if (wallet != null)
                {
                    wallet.IncrementFailedAttempts();
                    await _unitOfWork.Wallets.UpdateAsync(wallet, cancellationToken);
                }

                var failureLog = AuditLog.CreateFailure(
                    "LoginFailed",
                    "User",
                    user.Id.ToString(),
                    user.Id.ToString(),
                    "Invalid password",
                    userId: user.Id
                );
                await _unitOfWork.AuditLogs.AddAsync(failureLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Check if user is active
            if (!user.IsActive || user.Status != "Active")
            {
                var failureLog = AuditLog.CreateFailure(
                    "LoginFailed",
                    "User",
                    user.Id.ToString(),
                    user.Id.ToString(),
                    "User account is inactive",
                    userId: user.Id
                );
                await _unitOfWork.AuditLogs.AddAsync(failureLog, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new AuthResultDto
                {
                    Success = false,
                    Message = "User account is inactive"
                };
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Reset failed attempts
            var wallet2 = await _unitOfWork.Wallets.GetByUserIdAsync(user.Id, cancellationToken);
            if (wallet2 != null)
            {
                wallet2.ResetFailedAttempts(user.Id.ToString());
                await _unitOfWork.Wallets.UpdateAsync(wallet2, cancellationToken);
            }

            // Log successful login
            var auditLog = AuditLog.Create(
                "LoginSuccessful",
                "User",
                user.Id.ToString(),
                user.Id.ToString(),
                userId: user.Id
            );
            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get user role
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new AuthResultDto
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            return new AuthResultDto
            {
                Success = false,
                Message = $"Login failed: {ex.Message}"
            };
        }
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserType = user.UserType,
            IsKycVerified = user.IsKycVerified,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
    }
}
