# Agent Guide

Read [docs/architecture/README.md](docs/architecture/README.md) before changing application behavior. Treat the API contract, authorization rules, and persistence migrations as one change set.

## Working Rules

- Preserve the dependency direction: `Api -> Application -> Domain`; `Persistence` implements Application interfaces and is composed by Api.
- Keep domain invariants in aggregates/entities, orchestration in Application handlers, transport concerns in Api, and EF mappings/repositories in Persistence.
- Add or update a FluentValidation validator for every command carrying user input.
- Authorization has two layers: API permission policy first, then ownership checks in Application handlers. Do not rely on the UI to enforce permissions.
- Every authenticated API request performs server-side session and token-version validation. New access tokens must include `sid` and `token_version` claims.
- Generate EF migrations with `dotnet-ef`; do not hand-author migration snapshots. Review generated migrations and add only necessary data-backfill SQL.
- Update `TestPlatform.Api/TestPlatform.Api.http`, unit tests, and relevant docs whenever public endpoint, request, response, auth, or role behavior changes.

## Validation

```bash
dotnet test TestPlatform.slnx --no-restore
dotnet build TestPlatform.Api/TestPlatform.Api.csproj --no-restore
/Users/shreyasks/.dotnet/tools/dotnet-ef migrations has-pending-model-changes --project TestPlatform.Persistence --startup-project TestPlatform.Api --no-build
```

## Frontend Integration

The frontend lives in the sibling [`../TestPlatform.UI`](../TestPlatform.UI) repository. Read its `AGENTS.md` and `docs/api-contract.md` when changing a cross-repository contract. Update its DTOs, auth behavior, and tests whenever this API changes public behavior.
