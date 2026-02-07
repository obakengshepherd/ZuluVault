using MediatR;
using ZuluVault.Application.DTOs;

namespace ZuluVault.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<AuthResultDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MobilePhone { get; set; }
    public string UserType { get; set; } = "Individual";
}

public class LoginCommand : IRequest<AuthResultDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenCommand : IRequest<AuthResultDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}
