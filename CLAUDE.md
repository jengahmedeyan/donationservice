# CLAUDE.md

Canonical entry point for AI coding agents in this repository. This file is the index; the
detailed always-follow rules live in `rules/` and are loaded just-in-time when relevant.
`AGENTS.md` points here so Codex and Claude Code stay in sync. Committed and reviewed like code.

## Project overview

`DonationService` — a .NET 8 Web API where community members submit and track donation
requests. Clean Architecture with an in-memory repository (no external DB required); swap in
EF Core/Postgres inside Infrastructure without touching the inner layers.

## Tech stack (do not introduce alternatives without asking)

- .NET 8 / C# 12, ASP.NET Core Web API
- MediatR (command/query handlers), FluentValidation
- xUnit + FluentAssertions + Moq for tests

## Security non-negotiables (read every time)

Kept inline because these are what AI code most often gets wrong — full detail in
[rules/common/security.md](rules/common/security.md):

- **Authorization, not just authentication:** a user may only read/modify their OWN donation
  requests. Check ownership on every record-specific path (not-found → 404, wrong owner → 403),
  before any state-specific behavior.
- **PII:** never log or leak `RequesterName` / `Location` / `RequesterId` — log the `Id`.
  Responses use `DonationRequestDto` (omits `RequesterId`).

## Rules (read the relevant file before working)

- [rules/common/git-workflow.md](rules/common/git-workflow.md) — branch → PR → human-merge; verify gate; commit style
- [rules/common/security.md](rules/common/security.md) — authorization + PII (non-negotiable)
- [rules/common/testing.md](rules/common/testing.md) — required test cases; assert real behavior
- [rules/csharp/layering.md](rules/csharp/layering.md) — Clean Architecture layers + the reference slice
- [rules/csharp/conventions.md](rules/csharp/conventions.md) — async, records, validation & error mapping

## Workflow (summary — see git-workflow.md)

**Plan and get explicit approval** (Plan Mode — see below) → **branch** (never `main`) →
implement the smallest change → `/verify` → `/security-scan` + `/review-security` for
handler/endpoint changes → **`/open-pr`** → a human merges. The agent never commits to `main`
or merges its own PR. To add a feature, use `/new-slice`.

**The planning step is a real gate, not a formality.** For any non-trivial task, enter Plan Mode
(`EnterPlanMode`), draft the plan, and call `ExitPlanMode` to request explicit approval —
do not create branches or write/edit files until that approval is granted. A one-line fix or a
docs-only typo can skip it; anything that adds/changes behavior, tests, or the harness itself
cannot. Full detail: [rules/common/git-workflow.md](rules/common/git-workflow.md#plan-and-approve-gate).

## Harness surface

- **Commands** (`.claude/commands/`): `/verify`, `/new-slice`, `/open-pr`, `/security-scan`,
  `/review-security`.
- **Subagents** (`.claude/agents/`): `planner`, `csharp-code-reviewer`, `security-reviewer`,
  `tdd-guide` — see [.claude/agents/README.md](.claude/agents/README.md).
- **Hooks** (`.claude/hooks/`): block commits to `main`; format + session note on stop.
- **Evals** (`evals/`): deterministic guardrail checks + golden tasks; re-run when switching models.
- Context/token conventions and model routing: [docs/agent-working-agreements.md](docs/agent-working-agreements.md).

## Reference slice — mimic it exactly

`SubmitDonationRequest` (command) and `GetDonationRequestById` (query, with ownership check).
Details in [rules/csharp/layering.md](rules/csharp/layering.md).

If a requirement is ambiguous or conflicts with these rules, ask — don't guess.
