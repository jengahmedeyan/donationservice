---
description: Run the deterministic security/guardrail scan (automated, no model judgment)
allowed-tools: Bash(bash evals/check-guardrails.sh), Read, Grep, Glob
---

Run the repository's **deterministic** security and layering checks and report the result.

This is the automated, model-free layer (like a SAST pass). It complements — does not replace —
`/review-security`, which is the model-based judgment review (the `security-reviewer` subagent).
Use both: this catches mechanical regressions cheaply and fast; `/review-security` catches the
subtle logic/authorization issues a static check can't.

Steps:
1. Run `bash evals/check-guardrails.sh` and report each check ✅/❌:
   - No PII (`RequesterName`/`Location`/`RequesterId`) in logging calls.
   - No data access in the Api layer.
   - Application layer depends only on interfaces (no concrete data store).
   - Every handler is referenced by a test.
2. If any check fails, quote the offending file:line from the script output and stop with a
   **BLOCK** verdict.
3. If all pass, report **PASS** and recommend `/review-security` for the judgment-level pass on
   any change touching a handler or endpoint.

Do not fix anything — just scan and report.
