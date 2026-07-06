---
description: Security review focused where agents are weak — authorization and PII
allowed-tools: Read, Grep, Glob, Bash(git diff:*)
---

Review the current changes (or the files named in $ARGUMENTS) against this repo's non-negotiable
security rules. This is a focused review, not a general code review — spend attention only on the
areas below, where AI-generated code most often goes wrong.

## Scope
If there is a git diff, review the diff. Otherwise review the files in $ARGUMENTS, or ask which
files to review.

## Checklist — report each as ✅ / ❌ / ⚠️ with file:line evidence

1. **Object-level authorization.** Does every handler/endpoint that reads or mutates a specific
   donation verify the caller owns it? Not-found → null/404, wrong owner → `ForbiddenAccessException`
   (→ 403). Flag any record-specific path that skips the ownership check. This is the #1 risk.
2. **PII exposure.** `RequesterName`, `Location`, `RequesterId` must never appear in:
   logs (grep for logging calls), exception/error messages, or response DTOs the caller isn't
   entitled to. Log the donation `Id` instead. Flag any leak.
3. **Error leakage.** No raw exception messages or stack traces returned to callers. Failures map
   to problem-details (400 validation / 403 forbidden) via `Program.cs`.
4. **Input validation.** Every request DTO carrying input has a FluentValidation validator.
5. **Secrets & queries.** No hardcoded secrets; LINQ/parameterized access only; data access stays
   in Infrastructure.

End with a one-line verdict: **SAFE TO MERGE** or **BLOCK** + the must-fix items. Do not fix
anything unless asked — just report.
