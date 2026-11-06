using FluentValidation;
using MediatR;
using System.Security.Claims;
using TestPlatform.Api.Contracts.Auth;
using TestPlatform.Api.Extensions;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Features.Auth.Login;
using TestPlatform.Application.Features.Auth.Logout;
using TestPlatform.Application.Features.Auth.LogoutAll;
using TestPlatform.Application.Features.Auth.Refresh;

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

        app.MapPost("/api/auth/logout", LogoutAsync)
            .WithName("Logout")
            .WithTags("Auth")
            .RequireAuthorization();

        app.MapPost("/api/auth/logout-all", LogoutAllAsync)
            .WithName("LogoutAll")
            .WithTags("Auth")
            .RequireAuthorization();

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

    private static async Task<IResult> LogoutAsync(
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new LogoutCommand(user.GetUserId(), user.GetSessionId()), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> LogoutAllAsync(
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new LogoutAllCommand(user.GetUserId()), cancellationToken);
            return Results.NoContent();
        }
        catch (AuthenticationFailedException exception)
        {
            return exception.ToUnauthorizedProblem("Authentication failed");
        }
    }
}
