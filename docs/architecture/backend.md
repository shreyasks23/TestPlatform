# Backend Guide

## Project Layout

| Project | Responsibility |
| --- | --- |
| `TestPlatform.Api` | Minimal API contracts/endpoints, JWT, CORS, OpenAPI, HTTP error mapping, service composition. |
| `TestPlatform.Application` | Commands, queries, handlers, validators, DTOs, repository/security interfaces, authorization helpers. |
| `TestPlatform.Domain` | Aggregates, entities, invariants, domain events, valid role definitions. |
| `TestPlatform.Persistence` | EF Core context/configurations, migrations, repositories, PostgreSQL wiring, development seeds. |
| `TestPlatform.Tests` | Handler-level tests using in-memory repository and security doubles. |

`TestPlatform.Infrastructure` is currently a placeholder. Do not add cross-cutting integrations there until a concrete external concern requires it.

## Feature Pattern

For a new capability:

1. Put invariant-preserving behavior on the relevant Domain aggregate/entity.
2. Define a command/query, validator, handler, and DTO in the Application feature folder.
3. Add or extend a repository interface only when the handler needs persistence access not already modeled.
4. Implement the interface in Persistence and update EF mappings if the model changed.
5. Add an endpoint and request contract in Api. Convert HTTP input to Application DTOs; do not expose EF entities directly.
6. Add focused handler tests and an `.http` request where the behavior is publicly callable.

## Authorization Rules

Roles and permission claims are defined in `AuthConstants` and `UserRoles`.

| Role | Capabilities |
| --- | --- |
| `TestTaker` | Read tests and submit attempts. |
| `TestAuthor` | Test-taker capabilities plus read the answer-key question bank and create/update/delete owned questions and tests. |
| `Admin` | All author capabilities plus user and role management, and ownership bypass. |

Route policies establish broad capability. `OwnershipAuthorization.EnsureOwnerOrAdmin` protects update/delete operations; keep this second check when adding a new mutable owned resource.

## Persistence and Migrations

PostgreSQL is configured by `ConnectionStrings:TestPlatformDatabase`. Development startup calls `Database.MigrateAsync()` and seeds data. The seeder is development-only; do not add production defaults or credentials to it.

The API Docker image listens on port `8080`. `/healthz` is an anonymous database-connectivity probe for the container platform. The API supports a one-off `--migrate-database` command that applies pending migrations and exits without seeding; run it once as a deployment step before updating API replicas, not in every production replica.

After changing an EF model:

```bash
/Users/shreyasks/.dotnet/tools/dotnet-ef migrations add MeaningfulName --project TestPlatform.Persistence --startup-project TestPlatform.Api
dotnet build TestPlatform.Api/TestPlatform.Api.csproj --no-restore
/Users/shreyasks/.dotnet/tools/dotnet-ef migrations has-pending-model-changes --project TestPlatform.Persistence --startup-project TestPlatform.Api --no-build
```

Use generated migration and snapshot files. For data-safe schema changes, edit only the generated migration body to add deterministic backfill SQL; never hand-edit the model snapshot.
