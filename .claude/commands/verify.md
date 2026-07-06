---
description: Run the full verify gate (build, format, test) and report pass/fail
allowed-tools: Bash(dotnet build:*), Bash(dotnet test:*), Bash(dotnet format:*), Bash(dotnet build-server shutdown)
---

Run the project's definition-of-done gate and report the result concisely.

1. `dotnet build -warnaserror`
2. `dotnet format --verify-no-changes`
3. `dotnet test`

If step 1 fails with `MSB3101 ... being used by another process`, run
`dotnet build-server shutdown` once and retry — that is a stale build-server lock, not a code
error.

Report each step as ✅/❌ with the key line of output. If anything failed, stop and summarize
what broke; do not attempt fixes unless asked.
