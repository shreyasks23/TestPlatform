using MediatR;

namespace TestPlatform.Application.Features.TestSubmissions.SubmitTest;

public sealed record SubmitTestCommand(
    Guid TestId,
    Guid ActorId,
    IReadOnlyCollection<SubmitTestAnswerDto> Answers) : IRequest<SubmitTestResultDto>;
