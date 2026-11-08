using MediatR;

namespace TestPlatform.Application.Features.TestAttempts.GetAttempts;

public sealed record GetAttemptsQuery(Guid UserId) : IRequest<IReadOnlyList<TestAttemptSummaryDto>>;
