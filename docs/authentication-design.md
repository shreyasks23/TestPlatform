# Authentication Design

## Overview

This document describes the initial authentication and authorization design for the TestPlatform backend.

The design goals for v1 are:

- support multiple API clients with a standard bearer-token model
- keep the first implementation narrow and operationally simple
- protect mutating endpoints with a single permission
- leave room to expand roles, permissions, and session management later

The chosen model is:

- email/password authentication
- JWT bearer access tokens
- refresh tokens with rotation
- one role: `Admin`
- one permission: `api.write`

## Auth flows

### Login

Clients authenticate with `POST /api/auth/login` using email and password.

On successful authentication, the backend:

- verifies the submitted password against the stored password hash
- issues a short-lived JWT access token
- issues a refresh token
- persists refresh-token state for later validation, rotation, and revocation

The login response returns both tokens and their expiration timestamps.

### Access token usage

Clients send the access token in the `Authorization` header using the `Bearer` scheme.

The API validates:

- signature
- issuer
- audience
- expiration
- claims required for authorization

The access token is the only token used to authorize normal API requests.

### Refresh

Clients use `POST /api/auth/refresh` when the access token expires.

The backend:

- validates the submitted refresh token against persisted server-side state
- checks token expiry and revocation status
- rotates the refresh token on success
- returns a new access token and refresh token pair

Refresh-token rotation is part of the default security posture for v1.

### Revoke

Clients use `POST /api/auth/revoke` to invalidate a refresh token.

After revocation:

- the revoked refresh token can no longer be used to obtain a new access token
- the client must perform a full login again after the current access token expires

## Token and claim model

### Access token

The access token is a JWT intended for bearer authentication across multiple clients.

Required claims for v1:

- `sub`: the user identifier
- `email`: the authenticated user email
- `role`: `Admin`
- `permission`: `api.write`

Authorization should rely on permission claims instead of hard-coded role checks. The role remains useful for identity context and future expansion, but policy enforcement should be permission-driven.

### Refresh token

The refresh token is an opaque long-lived credential stored and validated server-side.

Design expectations:

- store a hashed or otherwise safely persisted form rather than the raw token when practical
- record expiry metadata
- record revocation state
- support rotation so each successful refresh invalidates the prior token

The exact storage shape may start simple in v1 and evolve if multi-device or multi-session requirements grow.

## Authorization policy

### Role-to-permission mapping

The initial model contains one role and one permission:

- role: `Admin`
- permission: `api.write`

In v1, an admin user receives the `api.write` claim in the access token.

### Policy

All mutating endpoints should require a single authorization policy named `CanWriteApi`.

`CanWriteApi` should validate the presence of the `api.write` permission claim.

### Protected endpoints

The following endpoints are protected in v1:

- `POST /api/users`
- `PUT /api/users/{id}`
- `POST /api/questions`
- `PUT /api/questions/{id}`
- `DELETE /api/questions/{id}`
- `POST /api/tests`
- `POST /api/tests/{id}/questions`
- `POST /api/tests/{id}/submit`

### Public endpoints

The following endpoint categories remain anonymous in v1:

- `GET /api/users`
- `GET /api/users/{id}`
- `GET /api/questions`
- `GET /api/questions/{id}`
- `GET /api/tests`
- `GET /api/tests/{id}`

This split keeps the first rollout simple while protecting state-changing behavior.

## API contracts

### Login

Request body:

```json
{
  "email": "admin@example.com",
  "password": "example-password"
}
```

Response body:

```json
{
  "accessToken": "<jwt>",
  "accessTokenExpiresAtUtc": "2026-06-04T12:00:00Z",
  "refreshToken": "<opaque-token>",
  "refreshTokenExpiresAtUtc": "2026-06-11T12:00:00Z"
}
```

### Refresh

Request body:

```json
{
  "refreshToken": "<opaque-token>"
}
```

Response body:

```json
{
  "accessToken": "<jwt>",
  "accessTokenExpiresAtUtc": "2026-06-04T13:00:00Z",
  "refreshToken": "<new-opaque-token>",
  "refreshTokenExpiresAtUtc": "2026-06-11T13:00:00Z"
}
```

### Revoke

Request body:

```json
{
  "refreshToken": "<opaque-token>"
}
```

A successful revoke response may be `204 No Content` or another minimal success response, depending on the eventual endpoint style chosen during implementation.

### User creation

Because provisioning is admin-managed in v1, the create-user contract should be extended to include an initial password:

```json
{
  "email": "user@example.com",
  "firstName": "Ada",
  "lastName": "Lovelace",
  "password": "initial-password"
}
```

## Data model impact

### User entity

The existing `User` entity remains the primary identity record and will be extended with authentication data.

Expected additions:

- password hash
- role

This keeps v1 implementation small and avoids a separate auth-account abstraction until there is a clear need for one.

### Refresh-token persistence

The backend must persist refresh-token state. The exact shape can be one of two patterns:

- a simple implementation tied directly to the user for one active refresh token per user
- a dedicated refresh-token store for richer multi-session support

The initial implementation may choose the simpler path as long as rotation and revocation are supported and future evolution remains possible.

## Configuration

The API should define a dedicated JWT configuration section.

Required settings:

- issuer
- audience
- signing key
- access token lifetime
- refresh token lifetime

Implementation guidance:

- keep the signing key out of source control outside local development defaults
- validate issuer and audience consistently across environments
- prefer short access-token lifetimes and longer refresh-token lifetimes

## Testing considerations

The auth layer should be covered at both the application and API boundary.

Recommended test scenarios:

- login succeeds with valid credentials
- login fails for unknown email
- login fails for invalid password
- passwords are never persisted in plaintext
- refresh succeeds for a valid active refresh token
- refresh fails for expired, revoked, or rotated tokens
- revoke prevents future refresh with the same token
- anonymous callers receive `401` on protected write endpoints
- callers without the required permission receive `403`
- anonymous callers can still access public read endpoints

Because the current test suite is handler-focused, API-level integration tests will likely need to be introduced as part of the auth implementation.

## Out of scope

The following are explicitly excluded from v1:

- self-service registration
- password reset
- email verification
- MFA
- lockout and brute-force protections
- advanced audit history
- multiple roles per user
- multiple permissions per role
- fine-grained per-resource authorization
- external identity providers
