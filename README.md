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

With the sibling `../TestPlatform.UI` repository checked out, start the full UI, API, and PostgreSQL stack from this repository root:

```bash
cp -n .env.example .env
docker compose up --build
```

Open the UI at `http://localhost:5173`; the API is available at `http://localhost:5209`. Development startup applies pending migrations and seeds role-specific development accounts. PostgreSQL data persists in a named volume; `docker compose down -v` deletes it.

For native API development, use .NET SDK 10 and PostgreSQL:

```bash
dotnet run --project TestPlatform.Api --launch-profile http
dotnet test TestPlatform.slnx
```

The API runs at `http://localhost:5209`. Use `TestPlatform.Api/TestPlatform.Api.http` for end-to-end API requests.
