using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;

namespace TestPlatform.Application.Features.TestAttempts.GetAttempts;

public sealed class GetAttemptsQueryHandler(ITestAttemptRepository testAttemptRepository)
    : IRequestHandler<GetAttemptsQuery, IReadOnlyList<TestAttemptSummaryDto>>
{
    public async Task<IReadOnlyList<TestAttemptSummaryDto>> Handle(
        GetAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        var attempts = await testAttemptRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return attempts
            .Select(attempt => new TestAttemptSummaryDto(
                attempt.Id,
                attempt.TestId,
                attempt.Score,
                attempt.Answers.Count,
                attempt.IsEvaluated))
            .ToArray();
    }
}
