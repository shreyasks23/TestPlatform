using FluentValidation;
using MediatR;
using TestPlatform.Api.Contracts.Auth;
using TestPlatform.Api.Extensions;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Features.Auth.Login;
using TestPlatform.Application.Features.Auth.Refresh;
using TestPlatform.Application.Features.Auth.Revoke;

namespace TestPlatform.Api.Endpoints;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", LoginAsync)
            .WithName("Login")
            .WithTags("Auth")
            .AllowAnonymous();

        app.MapPost("/api/auth/refresh", RefreshAsync)
            .WithName("RefreshToken")
            .WithTags("Auth")
            .AllowAnonymous();

        app.MapPost("/api/auth/revoke", RevokeAsync)
            .WithName("RevokeRefreshToken")
            .WithTags("Auth")
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken));
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
        catch (AuthenticationFailedException exception)
        {
            return exception.ToUnauthorizedProblem("Authentication failed");
        }
    }

    private static async Task<IResult> RefreshAsync(
        RefreshTokenRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken));
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
        catch (AuthenticationFailedException exception)
        {
            return exception.ToUnauthorizedProblem("Authentication failed");
        }
    }

    private static async Task<IResult> RevokeAsync(
        RevokeRefreshTokenRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new RevokeRefreshTokenCommand(request.RefreshToken), cancellationToken);
            return Results.NoContent();
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
    }
}
