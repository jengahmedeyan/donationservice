# Git workflow (branch → PR → human-merge)

Changes reach `main` through review, never directly.

1. **Plan and get explicit approval** for non-trivial work — see the gate below. Do not create
   branches or write/edit files until approval is granted.
2. **Branch, never work on `main`.** Start every task on a feature branch
   (`feature/…`, `fix/…`, `chore/…`). Committing on `main` is blocked by the
   `block-main-commit` hook.
3. Smallest change that satisfies the task; don't refactor unrelated code.
4. Self-verify with `/verify`, and run `/security-scan` + `/review-security` for any change
   touching a handler or endpoint.
5. **Open a PR — do not merge it.** Use `/open-pr`: it pushes the branch and opens a PR against
   `main` with the template filled in. A human reviews and merges. The agent never commits
   directly to `main` and never merges its own PR. CI must be green on the PR.
6. If a requirement is ambiguous or conflicts with the rules, ask — don't guess.

## Plan and approve gate

Planning is a real gate, not a narrated formality — a plan you describe in prose and then act on
without a stop is not an approval, it's an announcement. Use the actual mechanism:

1. Call `EnterPlanMode` before any exploratory reading turns into a plan.
2. Draft the plan: what changes, which files, the steps, the tests, and any open decisions.
3. Call `ExitPlanMode` to hand the plan to the human for explicit approval, a redirect, or
   rejection. **Only after approval** may branches be created or files written/edited.

**Exceptions (may skip the gate):** a one-line fix, a docs-only typo/wording fix, or anything the
human has already explicitly pre-approved in the same turn (e.g. "yes, do exactly that"). Anything
that adds or changes behavior, tests, or the harness itself (`.claude/`, `rules/`, CI, hooks) goes
through the gate — including harness changes proposed by the agent itself.

Delegating the drafting to the `planner` subagent is encouraged when it's loaded (see
`.claude/agents/planner.md`) but is not required — the main agent entering Plan Mode itself and
presenting the plan satisfies the gate. Subagent availability should never block getting a plan
approved.

## Verify gate (must ALL pass before a task is "done")

```shell
dotnet build -warnaserror
dotnet format --verify-no-changes
dotnet test
```

Or run `/verify`. If the build fails with `MSB3101 ... being used by another process`, run
`dotnet build-server shutdown` first — that is a stale build-server lock, not a code error.

## Commit messages

Plain messages describing the change. No `Co-Authored-By` trailer; no AI/assistant attribution.

## Do NOT

- Commit directly to `main`, or merge your own PR. Work on a branch; a human merges.
