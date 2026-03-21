using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;

namespace TestPlatform.Application.Features.Tests.GetTests;

public sealed class GetTestsQueryHandler(ITestRepository testRepository)
    : IRequestHandler<GetTestsQuery, IReadOnlyList<TestSummaryDto>>
{
    public async Task<IReadOnlyList<TestSummaryDto>> Handle(GetTestsQuery request, CancellationToken cancellationToken)
    {
        var tests = await testRepository.GetAllAsync(cancellationToken);

        return tests
            .Select(test => new TestSummaryDto(test.Id, test.Title, test.QuestionIds.Count))
            .ToArray();
    }
}
