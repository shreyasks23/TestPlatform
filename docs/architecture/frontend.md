# Frontend Integration Guide

## Repository Boundary

The frontend is implemented in the sibling [`../TestPlatform.UI`](../../../TestPlatform.UI) repository as a React, TypeScript, and Vite application. This document records backend integration requirements; implementation conventions and client architecture live in the UI repository's `docs/architecture.md` and `docs/api-contract.md`.

## Required Client Boundaries

- Keep API transport in one client module. Feature UI should call typed feature services, not build ad hoc URLs or headers.
- Store the access token only in memory. Persisting bearer access tokens in local storage increases XSS exposure.
- Store a refresh token only in a deliberately chosen secure mechanism. The current API returns it in JSON; a future production hardening step should move it to an HTTP-only, secure, same-site cookie.
- Attach `Authorization: Bearer <accessToken>` to protected requests.
- On a `401`, attempt one refresh with `POST /api/auth/refresh`; if that fails, clear local auth state and show the login view.
- Do not infer permissions from UI visibility. The API is authoritative; UI guards are for usability only.

## Authentication Lifecycle

```mermaid
sequenceDiagram
    participant UI as Browser client
    participant API as TestPlatform API
    UI->>API: POST /api/auth/login
    API-->>UI: access token + refresh token
    UI->>API: Protected request with Bearer access token
    API-->>UI: Response or 401
    UI->>API: POST /api/auth/refresh when access expires
    API-->>UI: Rotated token pair
    UI->>API: POST /api/auth/logout or /logout-all
    API-->>UI: 204; current tokens immediately invalid
```

`POST /api/auth/logout` invalidates the current session. `POST /api/auth/logout-all` invalidates every session for the signed-in user. After either call, clear local tokens even if the network response is interrupted; subsequent protected calls will return `401`.

## Role-Aware UX

| User experience | TestTaker | TestAuthor | Admin |
| --- | --- | --- | --- |
| Browse and take tests | Yes | Yes | Yes |
| Read answer-key question bank | No | Yes | Yes |
| Create or edit own content | No | Yes | Yes |
| Edit another author's content | No | No | Yes |
| Manage users and roles | No | No | Yes |

Suggested feature boundaries: `auth`, `tests`, `questions`, `users`, and a shared `api` transport module. Keep response DTOs in sync with [API and auth guide](api-and-auth.md); generate or hand-maintain client types from the OpenAPI document when a frontend is introduced.

## Local Integration

- API: `http://localhost:8080` when started through Docker Compose; `http://localhost:5209` for the native HTTP launch profile.
- Vite development origin: `http://localhost:5173`
- Vite preview origin: `http://localhost:4173`
- Vite proxies browser `/api` requests to the local API; direct cross-origin clients must be added to `Cors:AllowedOrigins`.
- Run Vite natively at port `5173`; it proxies `/api` to the Docker API at port `8080` by default.
- Use seeded development accounts documented in `docs/development.md` only for local work.
