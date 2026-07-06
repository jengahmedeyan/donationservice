<!--
Review focus: CI already proves build/format/tests are green. Human review time goes where
agents are weak — the checklist below. Trust the agent for scaffolding; verify hard here.
-->

## What & why

<!-- One or two lines. Link the task/issue. -->

## Verify gate (CI enforces these — confirm locally too)

- [ ] `dotnet build -warnaserror` clean
- [ ] `dotnet format --verify-no-changes` clean
- [ ] `dotnet test` green — happy path **and** failure/edge cases

## Reviewer checklist — where agents are weak

- [ ] **Authorization:** every record-specific path checks the caller owns the donation
      (not-found → 404, wrong owner → 403). No object-level access gap.
- [ ] **PII:** `RequesterName` / `Location` / `RequesterId` never logged, never in error
      messages, never returned to a caller not entitled to them.
- [ ] **Failure modes:** invalid input, unauthorized, and not-found are all handled and tested.
- [ ] **Architecture fit:** no logic in controllers, no data access outside Infrastructure,
      Domain interfaces not weakened.
- [ ] **Tests are meaningful:** assert real behavior, not just non-null / not just that the
      code the agent wrote runs.
- [ ] No hardcoded secrets; LINQ/parameterized access only.
