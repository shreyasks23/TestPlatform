using MediatR;

namespace TestPlatform.Application.Features.Tests.GetTestById;

public sealed record GetTestByIdQuery(Guid TestId) : IRequest<TestDetailsDto>;
