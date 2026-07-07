# Git workflow (branch → PR → human-merge)

Changes reach `main` through review, never directly.

1. Plan first for non-trivial tasks; wait for approval before coding.
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
