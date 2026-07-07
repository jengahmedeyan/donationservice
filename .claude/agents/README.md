# Subagents

Project subagents for delegated, focused work. Claude Code auto-discovers them (each `*.md`
here is a subagent with its own tools + system prompt). Invoke by asking for them, or let the
main agent delegate.

| Agent | Role | When |
| --- | --- | --- |
| `planner` | Read-only planner — turns a ticket into a concrete plan | Before writing code on any non-trivial task |
| `csharp-code-reviewer` | Correctness + Clean Architecture layering review | Before opening/merging a PR |
| `security-reviewer` | Deep authorization + PII pass (same checklist as `/review-security`) | Any change touching a handler or endpoint |
| `tdd-guide` | Specifies/critiques test cases (meaningful, not tautological) | When adding a handler/slice |

## Delegation pattern (the ECC-style loop, right-sized)

For a ticket: **planner** decomposes → implement the slice → **tdd-guide** confirms the test
cases are complete → **csharp-code-reviewer** + **security-reviewer** validate the diff → open a
PR. Reviewers are read-only and independent, so their findings are an honest check on the
implementation, not a rubber stamp.

All four are read-only (no `Edit`/`Write`); they report findings for the main agent or a human to
act on. They inherit the repo rules from `CLAUDE.md`.
