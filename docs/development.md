# Development Workflow

## Prerequisites

- Docker Desktop with Docker Compose v2.
- Node.js `22.19.0` and npm for the UI (pinned by `../TestPlatform.UI/.nvmrc`).
- .NET SDK `10.0.201` (pinned by `global.json`).
- Both `TestPlatform` and `TestPlatform.UI` repositories checked out as siblings.
- `dotnet-ef` 10.0.5 installed at `/Users/shreyasks/.dotnet/tools/dotnet-ef` when generating EF migrations.

## Run and Test

Create the local environment file and start the API and PostgreSQL from the backend repository root:

```bash
cp -n .env.example .env
docker compose up --build
```

Compose runs `dotnet watch` in Development, waits for PostgreSQL health, and applies pending migrations and development seeds on startup. The API is at `http://localhost:8080`; PostgreSQL is at `localhost:5432`. Data persists in the `database-data` named volume. `docker compose down -v` deletes that volume and its data.

Start the frontend in a second terminal:

```bash
cd ../TestPlatform.UI
npm ci
npm run dev
```

Open the UI at `http://localhost:5173`. Vite proxies `/api` calls to the API at port 8080. Both frontend and API source edits are hot-reloaded.

Use `docker compose logs -f` to view logs and `docker compose ps` to check database health. `docker compose down` stops containers but preserves database data.

For native development:

```bash
dotnet run --project TestPlatform.Api --launch-profile http
dotnet test TestPlatform.slnx --no-restore
```

The HTTP profile serves the API at `http://localhost:5209`. In Development it runs pending EF migrations and seeds data.

For local Docker development, migrations are applied automatically on API startup. For production, run the API with `--migrate-database` as a controlled one-off deployment step; this applies EF migrations and exits without starting the web server or seeding development data. Do not set this argument on regular API replicas.

## Production Deployment

Compose is not used in production. Deploy the Vite `dist` output to Azure Static Web Apps, publish the API with `dotnet publish` to Azure App Service, and configure App Service to connect to managed Azure Database for PostgreSQL. Set production connection strings, JWT signing keys, and the exact frontend CORS origin through Azure settings/Key Vault; never use the local `.env` values in Azure.

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
