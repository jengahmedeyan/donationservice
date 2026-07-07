---
description: Security review focused on authorization and PII (delegates to the security-reviewer subagent)
allowed-tools: Read, Grep, Glob, Bash(git diff:*)
---

Delegate this security review to the **security-reviewer** subagent (`.claude/agents/security-reviewer.md`),
which holds the canonical checklist. Pass it the scope: the current `git diff`, or the files named
in $ARGUMENTS. Relay its verdict (**SAFE TO MERGE** / **BLOCK** + must-fix items) back verbatim.

The checklist lives in one place — the subagent — so this command and the agent never drift.
