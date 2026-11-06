# TestPlatform Copilot Instructions

Before proposing changes, read `AGENTS.md` and `docs/architecture/README.md`.

- This is a .NET 10 Clean Architecture backend. Preserve `Api -> Application -> Domain`; Persistence implements Application repository interfaces.
- Keep endpoint mapping and HTTP contracts in `TestPlatform.Api`; keep commands, queries, validators, and handlers in `TestPlatform.Application`; keep persistence-free business rules in `TestPlatform.Domain`.
- Enforce authorization in both endpoint policies and Application ownership checks. Never trust a user ID, role, or ownership value supplied in request JSON.
- JWTs are immediately revocable through persisted refresh-token sessions and user token versions. Do not remove session validation, `sid`, or `token_version` claims.
- Use EF Core migrations generated through `dotnet-ef`, then verify no pending model changes.
- Update tests, `TestPlatform.Api/TestPlatform.Api.http`, and docs when API behavior changes.
- The browser client is the sibling `../TestPlatform.UI` repository. Update its API contract guidance and DTOs for cross-repository changes.
