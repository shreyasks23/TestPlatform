using FluentValidation;

namespace TestPlatform.Application.Features.Tests.UpdateTest;

public sealed class UpdateTestCommandValidator : AbstractValidator<UpdateTestCommand>
{
    public UpdateTestCommandValidator()
    {
        RuleFor(command => command.TestId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(200);
        RuleFor(command => command.ActorId).NotEmpty();
    }
}
