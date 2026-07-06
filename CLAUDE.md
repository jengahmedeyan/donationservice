# CLAUDE.md

Canonical steering file for AI coding agents working in this repository.
This file is the source of truth for **how agents write code here**. It is committed and
reviewed like code. `AGENTS.md` is a pointer to this file so Codex and Claude Code stay in sync.

## Project overview

`DonationService` — a .NET 8 Web API where community members submit and track donation
requests. Clean Architecture with an in-memory repository (no external DB required); swap in
EF Core/Postgres inside Infrastructure without touching the inner layers.

## Tech stack (do not introduce alternatives without asking)

- .NET 8 / C# 12, ASP.NET Core Web API
- MediatR (command/query handlers), FluentValidation
- xUnit + FluentAssertions + Moq for tests

## Architecture — respect the layers (dependencies point inward)

- **`src/Api/`** — Controllers only: routing, model binding, status codes. Thin: send a MediatR
  request, return the result. No business logic, no data access.
- **`src/Application/`** — Business logic: MediatR handlers, DTOs, FluentValidation validators,
  mapping. Depends on Domain interfaces, never on concrete data types.
- **`src/Domain/`** — Entities, value objects, interfaces (e.g. `IDonationRepository`). No
  framework dependencies.
- **`src/Infrastructure/`** — Implementations of Domain interfaces (repositories). The ONLY
  layer that touches a data store.

**Reference slice — mimic it exactly.** Follow `SubmitDonationRequest` (command) and
`GetDonationRequestById` (query, with ownership check) end to end:

- `src/Api/Controllers/DonationsController.cs`
- `src/Application/Donations/Commands/SubmitDonationRequest/*`
- `src/Application/Donations/Queries/GetDonationRequestById/*`
- `src/Infrastructure/Repositories/InMemoryDonationRepository.cs`
- Cross-cutting: `src/Application/Common/Behaviors/ValidationBehavior.cs`,
  `src/Application/Common/Exceptions/ForbiddenAccessException.cs`, and the failure→status
  mapping in `src/Api/Program.cs`.

To add a feature, use the `/new-slice` command — it scaffolds the slice the repo's way.

## Build, test, lint (must ALL pass before a task is "done")

```shell
dotnet build -warnaserror
dotnet format --verify-no-changes
dotnet test
```

Or run the `/verify` command, which runs all three and reports. If the build fails with
`MSB3101 ... being used by another process`, run `dotnet build-server shutdown` first — that is
a stale build-server lock, not a code error.

## Conventions

- Async all the way; I/O methods end in `Async` and take a `CancellationToken`.
- Records for DTOs/value objects. Nullable reference types stay enabled.
- One handler per file. Small, single-purpose methods. File-scoped namespaces.
- No magic strings for config; bind to typed options.

## Validation & errors

- Every request DTO has a FluentValidation validator in `Application/`. The `ValidationBehavior`
  pipeline runs it before the handler.
- Validation failures return 400 problem-details, never 500. Ownership failures throw
  `ForbiddenAccessException` → 403. Mapping lives in `Program.cs`.
- Never return raw exception messages or stack traces to callers.

## Security & data handling (non-negotiable)

- **Authorization, not just authentication:** a user may only read/modify their OWN donation
  requests. Every record-specific endpoint/handler checks ownership (see
  `GetDonationRequestByIdHandler`: not-found → null, wrong owner → `ForbiddenAccessException`).
- **PII:** `RequesterName`, `Location`, and `RequesterId` are sensitive. Never log them (log the
  donation `Id`), never expose fields the caller isn't entitled to, never put them in error
  messages.
- Parameterized queries / LINQ only. No secrets hardcoded.

Before handing back any change that touches a handler or endpoint, run `/review-security`.

## Testing requirements

- Every handler: happy path AND failure/edge cases (invalid input, unauthorized, not-found).
  See `GetDonationRequestByIdHandlerTests` for the shape (owned / not-found / wrong-owner).
- Assert real behavior, not just non-null. A test that only checks `NotBeNull()` is not enough.
- Mock repositories with Moq; no real DB in unit tests.

## Workflow for agents

1. Plan first for non-trivial tasks; wait for approval before coding.
2. Smallest change that satisfies the task; don't refactor unrelated code.
3. Self-verify with `/verify` and confirm the security/PII rules before handing back.
4. If a requirement is ambiguous or conflicts with this file, ask — don't guess.

See `docs/agent-working-agreements.md` for context/token conventions and the model-routing table.

## Do NOT

- Add business logic to controllers, or data access outside Infrastructure.
- Weaken Domain interfaces for an Infrastructure shortcut.
- Disable nullable/analyzers/tests to make a build pass.
- Commit secrets or real customer data.
