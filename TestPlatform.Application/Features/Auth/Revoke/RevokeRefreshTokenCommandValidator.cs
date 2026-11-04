using FluentValidation;

namespace TestPlatform.Application.Features.Auth.Revoke;

public sealed class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
{
    public RevokeRefreshTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty();
    }
}
