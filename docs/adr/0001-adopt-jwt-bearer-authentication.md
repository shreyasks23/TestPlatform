# ADR 0001: Adopt JWT bearer authentication for multi-client backend access

## Context

The backend is expected to be called by multiple clients rather than a single server-rendered web application. The current codebase exposes anonymous minimal API endpoints and has a `User` entity that models profile data only. There is no existing authentication, authorization, credential storage, or session management layer.

The first iteration needs to stay intentionally small while leaving room for future evolution. The agreed initial scope is:

- email and password based login
- JWT bearer access tokens
- refresh tokens for longer-lived client sessions
- three roles: `TestTaker`, `TestAuthor`, and `Admin`
- permissions derived from roles and enforced through policies
- authenticated read access, with question-bank answer keys limited to authors and admins
- admin-managed user provisioning with an initial password

## Decision

Adopt JWT bearer authentication as the v1 authentication mechanism for the backend.

This decision includes the following design choices:

- Use `POST /api/auth/login` for email/password authentication.
- Issue a short-lived JWT access token and a refresh token after successful login.
- Use `POST /api/auth/refresh` to rotate refresh tokens and issue a new token pair.
- Use authenticated `POST /api/auth/logout` for the current session and `POST /api/auth/logout-all` for all sessions.
- Validate JWT session and token-version claims against persisted state on each authenticated request so logout takes effect immediately.
- Extend the existing `User` entity with authentication data instead of introducing a separate auth-account aggregate in v1.
- Store one validated role on the user: `TestTaker`, `TestAuthor`, or `Admin`.
- Encode the permissions granted by the role as JWT claims.
- Enforce policies for user management, question-bank access, content writes, test reads, and test submissions.
- Enforce question and test ownership for authors; admins can manage all content.
- Require authentication for all application reads and writes.
- Allow only privileged callers to provision users, including setting the initial password during user creation.

## Alternatives considered

### Cookie-based session authentication

Cookie sessions would be a strong choice for a single trusted web client, especially when the backend and frontend share the same web boundary. They were not selected because this backend is intended for multiple clients and benefits from a stateless bearer-token model that works consistently across browser, mobile, and service consumers.

### Opaque server-stored access tokens

Opaque tokens simplify revocation and reduce some JWT-specific claim and validation risks. They were not selected because JWT bearer authentication is a better fit for multi-client API interoperability and aligns with the package and hosting model already present in the API project.

### API keys

API keys are suitable for service-to-service integrations and automation but are a poor fit for user login, user-specific claims, and future role or permission growth. They do not meet the expected end-user authentication requirements for this backend.

### External identity provider or OIDC

An external identity provider would reduce the need to build password handling and account security in-house. It was not selected for v1 because the initial scope is deliberately narrow and the team wants a simple first-party authentication model before introducing third-party identity infrastructure.

## Consequences

### Positive

- Supports multiple independent clients with a standard bearer-token model.
- Keeps the API stateless at the access-token validation boundary.
- Leaves room to grow authorization without changing the external authentication mechanism.
- Uses permission-based policy checks from the start, which will scale better than hard-coding role checks everywhere.

### Costs and tradeoffs

- The backend must now own password hashing, token issuance, refresh-token lifecycle management, and revocation behavior.
- Refresh tokens add persistence and rotation complexity compared with access-token-only auth.
- Extending `User` keeps v1 simple but couples profile and authentication concerns in the same entity.
- Anonymous reads mean the API will still mix protected and unprotected surfaces, which requires clear policy application and test coverage.

### Security defaults for v1

- Passwords must be stored as hashes, never plaintext.
- Access tokens should be short-lived.
- Refresh tokens should be rotated on successful refresh.
- Authorization policies should validate permissions rather than role names directly.

## Deferred follow-ups

The following items are intentionally out of scope for v1 and may be introduced later:

- self-service registration
- password reset and recovery
- email verification
- MFA
- account lockout and abuse controls
- multi-role or multi-permission storage
- richer session management across multiple devices
- migration to a dedicated auth-account model if separation becomes necessary
- migration to an external identity provider if operational needs outgrow the first-party model
