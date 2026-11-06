# API and Authentication Guide

## API Conventions

- Base URL: `http://localhost:5209` during local HTTP-profile development.
- Payloads and responses are JSON.
- Validation failures return `400` validation problems; missing entities return `404`; failed ownership checks return `403`; invalid or revoked sessions return `401`.
- Development OpenAPI is mapped by the API startup pipeline. Use `TestPlatform.Api/TestPlatform.Api.http` as the executable request reference.

## Endpoint Groups

| Group | Routes | Access |
| --- | --- | --- |
| Auth | `POST /api/auth/login`, `/refresh` | Anonymous |
| Sessions | `POST /api/auth/logout`, `/logout-all` | Authenticated user |
| Users | `GET/POST /api/users`, `GET/PUT /api/users/{id}` | Admin |
| Questions | `GET/POST /api/questions`, `GET/PUT/DELETE /api/questions/{id}` | Read: Author/Admin. Mutate: owner/Admin. |
| Tests | `GET/POST /api/tests`, `GET/PUT/DELETE /api/tests/{id}` | Read: all roles. Mutate: owner/Admin. |
| Test questions | `POST /api/tests/{id}/questions`, `DELETE /api/tests/{id}/questions/{questionId}` | Test owner/Admin |
| Submission | `POST /api/tests/{id}/submit` | All roles; submitter comes from JWT, never request JSON. |

## Token Contract

Login and refresh return:

```json
{
  "accessToken": "<jwt>",
  "accessTokenExpiresAtUtc": "<timestamp>",
  "refreshToken": "<opaque-token>",
  "refreshTokenExpiresAtUtc": "<timestamp>"
}
```

Access JWTs contain identity (`sub`, `email`), role, permission claims, session ID (`sid`), and user token version (`token_version`). The client does not need to parse these claims to operate correctly.

Refresh tokens are opaque and only their hashes are stored. Rotation revokes the previous refresh token and creates a replacement in the same stable session. Every authenticated request validates that the JWT session is active and that the user token version is current.

## Compatibility Rules

- `/api/auth/revoke` was removed. Clients must call authenticated `/api/auth/logout` instead.
- `POST /api/tests/{id}/submit` no longer accepts `userId`; the API derives it from the access token.
- Creating or updating users requires a valid `role`: `TestTaker`, `TestAuthor`, or `Admin`.
- Existing access tokens created before session/token-version claims are rejected and must be renewed through login.
