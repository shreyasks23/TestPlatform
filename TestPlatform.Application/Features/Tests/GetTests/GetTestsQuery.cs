using MediatR;

namespace TestPlatform.Application.Features.Tests.GetTests;

public sealed record GetTestsQuery : IRequest<IReadOnlyList<TestSummaryDto>>;
