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
# Set POSTGRES_PASSWORD and JWT_SIGNING_KEY in .env before starting Compose.
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
dotnet user-secrets set "ConnectionStrings:TestPlatformDatabase" "Host=localhost;Port=5432;Database=TestPlatformDb;Username=postgres;Password=<local-password>" --project TestPlatform.Api
dotnet user-secrets set "Jwt:SigningKey" "<random-local-signing-key>" --project TestPlatform.Api
dotnet run --project TestPlatform.Api --launch-profile http
dotnet test TestPlatform.slnx --no-restore
```

The HTTP profile serves the API at `http://localhost:5209`. In Development it runs pending EF migrations and seeds data.

For local Docker development, migrations are applied automatically on API startup. For production, run the API with `--migrate-database` as a controlled one-off deployment step; this applies EF migrations and exits without starting the web server or seeding development data. Do not set this argument on regular API replicas.

## Production Deployment

Compose is not used in production. Deploy the Vite `dist` output to Azure Static Web Apps, publish the API with `dotnet publish` to Azure App Service, and configure App Service to connect to managed Azure Database for PostgreSQL. Set production connection strings, JWT signing keys, and the exact frontend CORS origin through Azure settings/Key Vault; never use the local `.env` values in Azure.

Set these App Service application settings for the API:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__TestPlatformDatabase`
- `Jwt__Issuer` and `Jwt__Audience`, matching the values used to issue and validate tokens
- `Jwt__SigningKey`, supplied as a Key Vault reference
- `Cors__AllowedOrigins__0`, set to the exact HTTPS Static Web Apps origin (add indexed entries for any additional origins)
- `APPLICATIONINSIGHTS_CONNECTION_STRING`

Keep the Static Web Apps API URL configurable with the `VITE_API_BASE_URL` GitHub Actions variable in the UI repository. It should be the App Service API origin, without a trailing slash; the corresponding CORS value is the UI origin, not the API URL. The UI workflow passes that variable to its build. If the variable is unset, the client retains its same-origin `/api` behavior.

The API emits JSON console logs and sends OpenTelemetry telemetry to Application Insights when `APPLICATIONINSIGHTS_CONNECTION_STRING` is configured. App Service settings can use Key Vault references for secrets; do not copy local `.env` values into Azure. PostgreSQL data is managed by Azure Database for PostgreSQL, not the Compose named volume.

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

- `appsettings.json` contains non-secret JWT issuer/audience defaults only. Development database credentials and signing keys belong in .NET user-secrets or the ignored local `.env`, not checked-in settings.
- Development uses only the local Vite origins; non-development environments must configure an explicit `Cors:AllowedOrigins` list.
- `/health` and the backwards-compatible `/healthz` return `503` when PostgreSQL cannot be reached.
- Add frontend origins to `Cors:AllowedOrigins` deliberately; do not use an allow-any-origin policy with bearer authentication.
