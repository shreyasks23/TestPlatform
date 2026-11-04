using FluentValidation;
using MediatR;
using TestPlatform.Api.Contracts.Tests;
using TestPlatform.Api.Extensions;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Tests.AddQuestionToTest;
using TestPlatform.Application.Features.Tests.CreateTest;
using TestPlatform.Application.Features.Tests.GetTestById;
using TestPlatform.Application.Features.Tests.GetTests;
using TestPlatform.Application.Features.TestSubmissions.SubmitTest;

namespace TestPlatform.Api.Endpoints;

internal static class TestEndpoints
{
    public static IEndpointRouteBuilder MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tests", CreateTestAsync)
            .WithName("CreateTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteApi);

        app.MapGet("/api/tests", GetTestsAsync)
            .WithName("GetTests")
            .WithTags("Tests");

        app.MapGet("/api/tests/{id:guid}", GetTestByIdAsync)
            .WithName("GetTestById")
            .WithTags("Tests");

        app.MapPost("/api/tests/{id:guid}/questions", AddQuestionToTestAsync)
            .WithName("AddQuestionToTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteApi);

        app.MapPost("/api/tests/{id:guid}/submit", SubmitTestAsync)
            .WithName("SubmitTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteApi);

        return app;
    }

    private static async Task<IResult> CreateTestAsync(
        CreateTestRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateTestCommand(request.Title);
            var result = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/tests/{result.Id}", result);
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
    }

    private static async Task<IResult> GetTestsAsync(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTestsQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTestByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new GetTestByIdQuery(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
    }

    private static async Task<IResult> AddQuestionToTestAsync(
        Guid id,
        AddQuestionToTestRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new AddQuestionToTestCommand(
                id,
                request.QuestionId);

            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
        catch (InvalidOperationException exception)
        {
            return exception.ToBadRequestProblem("Invalid test question");
        }
    }

    private static async Task<IResult> SubmitTestAsync(
        Guid id,
        SubmitTestRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new SubmitTestCommand(
                id,
                request.UserId,
                request.Answers
                    .Select(answer => new SubmitTestAnswerDto(answer.QuestionId, answer.SelectedOptionId))
                    .ToArray());

            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
        catch (InvalidOperationException exception)
        {
            return exception.ToBadRequestProblem("Invalid test submission");
        }
    }
}
