# TestPlatform

TestPlatform is the .NET 10 assessment API. It manages users, question banks, tests, submissions, role-based authorization, and refresh-token-backed JWT sessions. The React/Vite client lives in the sibling [`../TestPlatform.UI`](../TestPlatform.UI) repository.

## Start Here

- [Architecture index](docs/architecture/README.md)
- [Backend guide](docs/architecture/backend.md)
- [Frontend integration guide](docs/architecture/frontend.md)
- [API and authentication guide](docs/architecture/api-and-auth.md)
- [Development workflow](docs/development.md)
- [Agent guidance](AGENTS.md)

## Local Quick Start

### Prerequisites

- Docker Desktop with Docker Compose v2.
- Node.js `22.19.0` and npm for the Vite frontend (`../TestPlatform.UI/.nvmrc` is used by nvm and CI).
- .NET SDK `10.0.201` (`global.json` pins the SDK and the API Dockerfiles use the same SDK).
- Both repositories checked out as siblings.

### Configure and start the backend/database

```bash
cp -n .env.example .env
docker compose up --build
```

This starts the .NET API with `dotnet watch` and PostgreSQL. The API waits for PostgreSQL, applies pending EF migrations, and seeds development accounts at startup. The database data is stored in the `database-data` named volume.

- API: `http://localhost:8080`
- PostgreSQL: `localhost:5432`

### Start the frontend

In another terminal:

```bash
cd ../TestPlatform.UI
npm ci
npm run dev
```

Open `http://localhost:5173`. Vite proxies `/api` requests to the Docker API at `http://localhost:8080`. Both Vite and `dotnet watch` support source hot reload.

### Stop and restart

Stop containers with `docker compose down`; the named database volume is preserved. Restart using `docker compose up --build`. To delete the database and all local data, run `docker compose down -v`.

View logs with `docker compose logs -f`; check service health with `docker compose ps`.

### Migrations

In Development, the API automatically applies all pending EF Core migrations whenever it starts. To apply a newly generated migration, restart the API:

```bash
docker compose restart api
```

Generate migrations using the repository's usual `dotnet-ef` command. The source mount makes the new migration available to the running watcher.

### Troubleshooting

- If Docker reports that ports `8080` or `5432` are already in use, stop the process using the port before starting Compose.
- If the API cannot connect to PostgreSQL, run `docker compose ps` and wait for `database` to show `healthy`; inspect `docker compose logs database api`.
- If database configuration changes after the database volume was initialized, the existing volume retains its original credentials. To recreate it and erase its data, run `docker compose down -v`, then `docker compose up --build`.
- If UI API requests fail, ensure the API is reachable at `http://localhost:8080` and restart Vite after changing `API_PROXY_TARGET`.

For native API development, use .NET SDK 10 and PostgreSQL:

```bash
dotnet run --project TestPlatform.Api --launch-profile http
dotnet test TestPlatform.slnx
```

The API runs at `http://localhost:5209`. Use `TestPlatform.Api/TestPlatform.Api.http` for end-to-end API requests.

### Production deployment is separate

Docker Compose is only for local development. Production remains:

- **Frontend:** GitHub Actions builds the Vite app and deploys `dist` to Azure Static Web Apps.
- **Backend:** GitHub Actions runs `dotnet publish` and deploys the API to Azure App Service.
- **Database:** Azure App Service connects to managed Azure Database for PostgreSQL.

Configure production connection strings, JWT signing keys, and the explicit frontend CORS origin in Azure App Service settings/Key Vault. Do not use `.env`, Compose credentials, or development seed accounts in Azure. The existing App Service workflow remains independently controlled.
