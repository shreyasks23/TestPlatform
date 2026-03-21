using MediatR;

namespace TestPlatform.Application.Features.TestSubmissions.SubmitTest;

public sealed record SubmitTestCommand(
    Guid TestId,
    Guid UserId,
    IReadOnlyCollection<SubmitTestAnswerDto> Answers) : IRequest<SubmitTestResultDto>;
