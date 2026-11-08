# Development Workflow

## Prerequisites

- .NET SDK 10
- PostgreSQL on `localhost:5432`
- Database settings matching `TestPlatform.Api/appsettings.json`
- `dotnet-ef` 10.0.5 installed at `/Users/shreyasks/.dotnet/tools/dotnet-ef`

## Run and Test

To run the UI, API, and PostgreSQL together in Docker, check out both repositories as siblings and run from the backend repository root:

```bash
cp -n .env.example .env
docker compose up --build
```

Open the UI at `http://localhost:5173`; the API is also available at `http://localhost:5209`. The Compose stack uses Development configuration, applies migrations, and seeds development accounts. PostgreSQL data persists in a named volume; `docker compose down -v` deletes it.

For native development:

```bash
dotnet run --project TestPlatform.Api --launch-profile http
dotnet test TestPlatform.slnx --no-restore
```

The HTTP profile serves the API at `http://localhost:5209`. In Development it runs pending EF migrations and seeds data.

For a controlled non-development migration step, run the API image with `--migrate-database`; it applies EF migrations and exits without starting the web server or seeding development data. Do not set this argument on the regular API replicas.

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
