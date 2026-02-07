using FluentValidation;
using ZuluVault.Application.Features.Wallets.Commands;

namespace ZuluVault.Application.Features.Wallets.Validators;

public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.DailyTransferLimit)
            .GreaterThan(0).WithMessage("Daily transfer limit must be greater than zero")
            .LessThanOrEqualTo(decimal.MaxValue).WithMessage("Daily transfer limit is too large");
    }
}
