# Architecture Guide

This directory is the source of truth for humans and coding agents working on TestPlatform.

| Document | Purpose |
| --- | --- |
| [System overview](system-overview.md) | Components, boundaries, request flow, and data ownership. |
| [Backend guide](backend.md) | Layer responsibilities, feature conventions, persistence, and extension points. |
| [Frontend guide](frontend.md) | Backend integration requirements for the sibling browser client. |
| [API and auth guide](api-and-auth.md) | Endpoint groups, role matrix, JWT/session semantics, and errors. |
| [Development workflow](../development.md) | Setup, commands, migrations, seeds, and validation. |

## Repository Boundaries

This repository contains the backend API and tests. The browser client is maintained separately in the sibling [`../TestPlatform.UI`](../../../TestPlatform.UI) repository. The API allows local Vite development (`http://localhost:5173`) and preview (`http://localhost:4173`) origins through CORS. Read the UI's `docs/architecture.md` and `docs/api-contract.md` when changing shared behavior.
