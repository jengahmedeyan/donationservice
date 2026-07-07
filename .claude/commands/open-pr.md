---
description: Verify, push the current branch, and open a PR against main (never merges)
allowed-tools: Bash(git:*), Bash(dotnet build:*), Bash(dotnet test:*), Bash(dotnet format:*), Bash(gh pr create:*), Bash(gh pr view:*), Bash(bash evals/check-guardrails.sh)
---

Open a pull request for the current branch. Follow the branch → PR → human-merge workflow — you
NEVER merge the PR; a human does.

Steps:
1. **Refuse if on `main`.** Run `git rev-parse --abbrev-ref HEAD`. If it's `main`, stop and tell
   the user to create a feature branch first — do not commit or push to `main`.
2. **Verify green.** Run `/verify` (build `-warnaserror`, format, tests) and
   `bash evals/check-guardrails.sh`. If anything fails, stop and report; do not open a PR on red.
3. **Push** the branch: `git push -u origin <branch>`.
4. **Open the PR** with `gh pr create --base main --head <branch>`, filling in the repo's PR
   template (`.github/pull_request_template.md`): what/why, the verify-gate checkboxes, and the
   reviewer checklist. Title should summarize the change.
5. **Report the PR URL.** Do not merge it. If the change touched a handler or endpoint, remind the
   user that `/review-security` (the security-reviewer subagent) should pass before merge.

Keep the PR body concise and specific — no filler.
