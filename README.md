# DonationService

## Layers
- `src/Domain` — entities + interfaces (no dependencies)
- `src/Application` — MediatR handlers, DTOs, validators (business logic)
- `src/Infrastructure` — repository implementation (in-memory; swap for EF Core)
- `src/Api` — controllers + composition root
- `tests/Application.Tests` — xUnit unit tests

## Prerequisites
- .NET 8 SDK
- Internet access on first run (to restore NuGet packages)

## Create the solution file (optional but handy)
```
cd DonationService
dotnet new sln -n DonationService
dotnet sln add src/Domain/Domain.csproj src/Application/Application.csproj \
               src/Infrastructure/Infrastructure.csproj src/Api/Api.csproj \
               tests/Application.Tests/Application.Tests.csproj
```

## Build, test, run (the "verify" commands from AGENTS.md)
```
dotnet restore
dotnet build -warnaserror
dotnet test
dotnet run --project src/Api      # then open the Swagger UI it prints
```

## Try it
POST a donation request:
```
curl -s -X POST http://localhost:5000/api/donations \
  -H "Content-Type: application/json" \
  -d '{"requesterId":"user-1","requesterName":"Ada Lovelace","itemNeeded":"Winter coat","location":"Toronto"}'
```
(Port may differ; use the one printed on startup.)

## Agentic development harness
This repo is set up for AI-assisted development. The setup lives in the repo, not in
people's heads:
- **[CLAUDE.md](./CLAUDE.md)** — canonical steering file (architecture, conventions, security
  non-negotiables). `AGENTS.md` points to it so Codex/Claude Code stay in sync.
- **`.editorconfig` + `Directory.Build.props`** — analyzers + format rules enforced on every build.
- **`.claude/settings.json`** — permission policy (auto-allow read/build/test; approval for push/network).
- **`.claude/commands/`** — reusable commands: `/verify`, `/new-slice`, `/review-security`.
- **`.github/workflows/ci.yml`** — CI gate: build `-warnaserror`, format check, tests, guardrail checks.
- **`evals/`** — regression tests for the agent workflow; re-run when switching models/prompts.
- **[docs/agent-working-agreements.md](./docs/agent-working-agreements.md)** — context/token/routing conventions.

Verified green on .NET SDK 10.x (targets net8.0): build `-warnaserror`, `dotnet format`, and
`dotnet test` (4 tests) all pass. Run the commands above to confirm on your machine.
