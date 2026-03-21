using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Tests;

internal sealed class TestRepositoryStub(params IEnumerable<Test> seededTests) : ITestRepository
{
    private readonly List<Test> _tests = [.. seededTests];

    public Test? AddedTest { get; private set; }

    public Task AddAsync(Test test, CancellationToken cancellationToken)
    {
        AddedTest = test;
        _tests.Add(test);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Test>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Test>>(_tests.AsReadOnly());

    public Task<Test?> GetByIdAsync(Guid testId, CancellationToken cancellationToken)
        => Task.FromResult(_tests.SingleOrDefault(test => test.Id == testId));
}
