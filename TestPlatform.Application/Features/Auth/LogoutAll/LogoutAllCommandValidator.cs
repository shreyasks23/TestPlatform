using FluentValidation;

namespace TestPlatform.Application.Features.Auth.LogoutAll;

public sealed class LogoutAllCommandValidator : AbstractValidator<LogoutAllCommand>
{
    public LogoutAllCommandValidator() => RuleFor(command => command.UserId).NotEmpty();
}
