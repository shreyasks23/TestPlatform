# Authentication Design

## Overview

This document describes the initial authentication and authorization design for the TestPlatform backend.

The design goals for v1 are:

- support multiple API clients with a standard bearer-token model
- keep the first implementation narrow and operationally simple
- enforce role-based capabilities through permission claims and resource ownership
- keep a single role per user while leaving room for future multi-role support

The chosen model is:

- email/password authentication
- JWT bearer access tokens
- refresh tokens with rotation
- roles: `TestTaker`, `TestAuthor`, and `Admin`
- permission claims derived from the assigned role

## Auth flows

### Login

Clients authenticate with `POST /api/auth/login` using email and password.

On successful authentication, the backend:

- verifies the submitted password against the stored password hash
- issues a short-lived JWT access token
- issues a refresh token
- creates a stable server-side session for later validation, rotation, and logout

The login response returns both tokens and their expiration timestamps.

### Access token usage

Clients send the access token in the `Authorization` header using the `Bearer` scheme.

The API validates:

- signature
- issuer
- audience
- expiration
- claims required for authorization
- the user token version and active server-side session

The access token is the only token used to authorize normal API requests.

### Refresh

Clients use `POST /api/auth/refresh` when the access token expires.

The backend:

- validates the submitted refresh token against persisted server-side state
- checks token expiry and revocation status
- rotates the refresh token on success
- returns a new access token and refresh token pair

Refresh-token rotation is part of the default security posture for v1.

### Logout

Clients use authenticated `POST /api/auth/logout` to end the current session, or `POST /api/auth/logout-all` to end every session for the authenticated user. Both endpoints immediately invalidate access tokens as well as refresh tokens. Logout-all increments the user's token version, invalidating every previously issued access token.

## Token and claim model

### Access token

The access token is a JWT intended for bearer authentication across multiple clients.

Required claims for v1:

- `sub`: the user identifier
- `email`: the authenticated user email
- `role`: the user's assigned role
- `permission`: one or more capabilities derived from the role
- `sid`: the stable server-side session identifier
- `token_version`: the user security version required for immediate global logout

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

Each user has exactly one role. The access token includes the capabilities granted by that role:

- `TestTaker`: `tests.read`, `tests.submit`
- `TestAuthor`: `questions.read`, `content.write`, `tests.read`, `tests.submit`
- `Admin`: all author capabilities plus `users.manage`

### Policy

Policies validate the permission claims for user administration, question-bank reads, content writes, test reads, and test submission. Content writes also enforce ownership in the application layer: authors may update or delete only their own questions and tests, while admins may manage all content.

### Protected endpoints

All application endpoints require a bearer token. Login and refresh remain anonymous.

- User management is admin-only.
- The full question bank, including answer keys and explanations, is visible only to authors and admins.
- All roles can browse tests and submit attempts. The submit API derives the attempt owner from the JWT subject rather than accepting a user ID from the client.

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

### Logout

`POST /api/auth/logout` and `POST /api/auth/logout-all` require a bearer token, accept no request body, and return `204 No Content`.

### User creation

Because provisioning is admin-managed in v1, the create-user contract should be extended to include an initial password:

```json
{
  "email": "user@example.com",
  "firstName": "Ada",
  "lastName": "Lovelace",
  "password": "initial-password",
  "role": "TestTaker"
}
```

## Data model impact

### User entity

The existing `User` entity remains the primary identity record and will be extended with authentication data.

The user record stores a password hash and one validated role.

Development data includes `shreyasks@mail.com` as Admin, `ada@example.com` as TestAuthor, and `grace@example.com` as TestTaker. Each uses the development password `P@SSw0rd`.

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
- logout invalidates the active session immediately
- logout-all invalidates every session and access token for the user immediately
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
- test assignment and publication workflows
- external identity providers
