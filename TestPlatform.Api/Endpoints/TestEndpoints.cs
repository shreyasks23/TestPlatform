using FluentValidation;
using MediatR;
using System.Security.Claims;
using TestPlatform.Api.Contracts.Tests;
using TestPlatform.Api.Extensions;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Tests.AddQuestionToTest;
using TestPlatform.Application.Features.Tests.CreateTest;
using TestPlatform.Application.Features.Tests.GetTestById;
using TestPlatform.Application.Features.Tests.GetTests;
using TestPlatform.Application.Features.Tests.UpdateTest;
using TestPlatform.Application.Features.Tests.DeleteTest;
using TestPlatform.Application.Features.Tests.RemoveQuestionFromTest;
using TestPlatform.Application.Features.TestAttempts.GetAttempts;
using TestPlatform.Application.Features.TestSubmissions.SubmitTest;

namespace TestPlatform.Api.Endpoints;

internal static class TestEndpoints
{
    public static IEndpointRouteBuilder MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tests", CreateTestAsync)
            .WithName("CreateTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteContent);

        app.MapGet("/api/tests", GetTestsAsync)
            .WithName("GetTests")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanReadTests);

        app.MapGet("/api/attempts", GetAttemptsAsync)
            .WithName("GetAttempts")
            .WithTags("Attempts")
            .RequireAuthorization(AuthConstants.Policies.CanReadTests);

        app.MapGet("/api/tests/{id:guid}", GetTestByIdAsync)
            .WithName("GetTestById")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanReadTests);

        app.MapPost("/api/tests/{id:guid}/questions", AddQuestionToTestAsync)
            .WithName("AddQuestionToTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteContent);

        app.MapPut("/api/tests/{id:guid}", UpdateTestAsync)
            .WithName("UpdateTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteContent);

        app.MapDelete("/api/tests/{id:guid}", DeleteTestAsync)
            .WithName("DeleteTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteContent);

        app.MapDelete("/api/tests/{id:guid}/questions/{questionId:guid}", RemoveQuestionFromTestAsync)
            .WithName("RemoveQuestionFromTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanWriteContent);

        app.MapPost("/api/tests/{id:guid}/submit", SubmitTestAsync)
            .WithName("SubmitTest")
            .WithTags("Tests")
            .RequireAuthorization(AuthConstants.Policies.CanSubmitTests);

        return app;
    }

    private static async Task<IResult> CreateTestAsync(
        CreateTestRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateTestCommand(request.Title, user.GetUserId());
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

    private static async Task<IResult> GetAttemptsAsync(
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAttemptsQuery(user.GetUserId()), cancellationToken);
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
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new AddQuestionToTestCommand(
                id,
                request.QuestionId,
                user.GetUserId(),
                user.IsAdmin());

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
        catch (AuthorizationDeniedException exception)
        {
            return exception.ToForbiddenProblem();
        }
    }

    private static async Task<IResult> UpdateTestAsync(
        Guid id,
        UpdateTestRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new UpdateTestCommand(id, request.Title, user.GetUserId(), user.IsAdmin()), cancellationToken);
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
        catch (AuthorizationDeniedException exception)
        {
            return exception.ToForbiddenProblem();
        }
    }

    private static async Task<IResult> DeleteTestAsync(
        Guid id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new DeleteTestCommand(id, user.GetUserId(), user.IsAdmin()), cancellationToken);
            return Results.NoContent();
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
        catch (AuthorizationDeniedException exception)
        {
            return exception.ToForbiddenProblem();
        }
    }

    private static async Task<IResult> RemoveQuestionFromTestAsync(
        Guid id,
        Guid questionId,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new RemoveQuestionFromTestCommand(id, questionId, user.GetUserId(), user.IsAdmin()), cancellationToken);
            return Results.NoContent();
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
        catch (InvalidOperationException exception)
        {
            return exception.ToBadRequestProblem("Invalid test question");
        }
        catch (AuthorizationDeniedException exception)
        {
            return exception.ToForbiddenProblem();
        }
    }

    private static async Task<IResult> SubmitTestAsync(
        Guid id,
        SubmitTestRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new SubmitTestCommand(
                id,
                user.GetUserId(),
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
