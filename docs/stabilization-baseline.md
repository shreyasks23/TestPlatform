# Stabilization Baseline

## Required Local Configuration

The API reads sensitive local and deployment configuration from standard ASP.NET Core configuration providers. Do not commit connection strings or JWT signing material.

Required names:

- `ConnectionStrings__TestPlatformDatabase`
- `Jwt__SigningKey`

For local development, set these with .NET user secrets or environment variables. `appsettings.Development.json` is ignored and may hold local-only values.

## Migration Policy

The API applies EF Core migrations and development seed data only when `ASPNETCORE_ENVIRONMENT` is `Development`. Production migrations must run in the deployment process before application startup.

## Validation Baseline

```bash
dotnet test TestPlatform.slnx
dotnet build TestPlatform.Api/TestPlatform.Api.csproj --no-restore
```

```bash
npm ci
npm run lint
npm run test
npm run build
```

## Remaining Test Priorities

- API integration coverage for CORS preflight, authorization policies, ownership checks, token refresh, and logout.
- Browser-to-API contract coverage with a disposable PostgreSQL database.
- Deployment verification that migrations run before production startup.
