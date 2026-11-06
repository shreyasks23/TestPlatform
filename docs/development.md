# Development Workflow

## Prerequisites

- .NET SDK 10
- PostgreSQL on `localhost:5432`
- Database settings matching `TestPlatform.Api/appsettings.json`
- `dotnet-ef` 10.0.5 installed at `/Users/shreyasks/.dotnet/tools/dotnet-ef`

## Run and Test

```bash
dotnet run --project TestPlatform.Api --launch-profile http
dotnet test TestPlatform.slnx --no-restore
```

The HTTP profile serves the API at `http://localhost:5209`. In Development it runs pending EF migrations and seeds data.

## Seed Accounts

All local seed accounts use `P@SSw0rd`.

| Account | Role |
| --- | --- |
| `shreyasks@mail.com` | Admin |
| `ada@example.com` | TestAuthor |
| `grace@example.com` | TestTaker |

Use `TestPlatform.Api/TestPlatform.Api.http` to log in, copy access tokens into request variables, exercise role-specific behavior, and test logout flows.

## Required Checks

```bash
dotnet test TestPlatform.slnx --no-restore
dotnet build TestPlatform.Api/TestPlatform.Api.csproj --no-restore
/Users/shreyasks/.dotnet/tools/dotnet-ef migrations has-pending-model-changes --project TestPlatform.Persistence --startup-project TestPlatform.Api --no-build
git diff --check
```

## Configuration Hygiene

- `appsettings.json` contains development-only database and JWT values. Replace them with secret-managed configuration before deployment.
- Keep `Jwt:SigningKey` out of source control in non-development environments.
- Add frontend origins to `Cors:AllowedOrigins` deliberately; do not use an allow-any-origin policy with bearer authentication.
