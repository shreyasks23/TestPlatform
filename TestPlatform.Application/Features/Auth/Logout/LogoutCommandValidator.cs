using FluentValidation;

namespace TestPlatform.Application.Features.Auth.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.SessionId).NotEmpty();
    }
}
