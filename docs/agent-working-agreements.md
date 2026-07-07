# Agent working agreements

Team conventions for *how* agents spend context and compute here. `CLAUDE.md` says what to build;
this says how to work efficiently while building it. Maps to guide topics 2 (model switching),
3 (context compression), and 4 (token optimization).

## Context compression (topic 3) — keep the window high-signal

- **Just-in-time retrieval, not preloading.** Don't dump whole directories into context. Use
  `grep`/`glob` and read the specific reference files named in `CLAUDE.md`. Folder and file names
  are signals — the slice you need is at a predictable path.
- **Structured note-taking for long tasks.** For a multi-step task (migration, multi-slice
  feature), keep a scratch `NOTES.md` (git-ignored) with decisions, open questions, and the
  files touched. It survives a context reset; pull it back in after compaction.
- **Clear stale tool output.** Once a build log or file dump is no longer needed, don't keep
  re-quoting it. Summarize the outcome ("build green") and move on.
- **Sub-agent for wide exploration.** For "where is X used across the repo" style questions,
  delegate to a search sub-agent that returns a short summary, so the main window stays clean.

## Token optimization (topic 4) — same result, fewer tokens

- **Scope the task small.** One slice per task. Small, well-specified tasks use fewer tokens and
  fail less than giant vague ones. `/new-slice` is intentionally narrow.
- **Prefer the commands.** `/verify`, `/new-slice`, `/review-security` encode the setup once so
  you don't re-derive it each time.
- **Concise tool output.** Pipe long build/test output through `tail`; don't paste full logs.
- **A few canonical examples, not exhaustive lists.** The reference slice is the one example to
  mirror — don't ask the agent to survey every file.

## Model routing (topic 2) — right model for the job

Keep the harness model-agnostic (all logic lives in `CLAUDE.md`, the commands, `.editorconfig`,
and the evals — nothing model-specific). Then route by task complexity:

| Task | Suggested tier | Why |
| --- | --- | --- |
| Formatting, boilerplate, commit messages, doc tweaks | cheap/fast | Low reasoning; speed and cost win |
| `/new-slice` scaffolding (mirror an existing pattern) | mid | Pattern-following, low ambiguity |
| `/review-security`, authorization/PII logic, tricky debugging | frontier | Where a miss is expensive |
| Architecture decisions, large refactors | frontier | High reasoning, high blast radius |

**When you switch the default model or edit `CLAUDE.md`, re-run `evals/`** (topic 6). Don't
switch on vibes — switch on the eval scores holding.

## Branch & PR workflow (non-negotiable)

Changes reach `main` through review, never directly. The loop:

1. Branch off `main` — `feature/…`, `fix/…`, or `chore/…`. Never commit to `main`.
2. Implement, `/verify`, `/review-security`.
3. Push the branch and open a PR against `main` with the template filled in.
4. A **human** reviews and merges. The agent never merges its own PR.

This is what makes velocity safe: your PR is already green (CI + guardrails ran on it)
before a human looks, so review starts from a known-good state and focuses on judgment,
not mechanics.

## Definition of done (ties to topic 6)

Green tests alone are not "done." A task is done when `/verify` is green, the
`/review-security` checklist passes for any change touching a handler or endpoint,
**and the work is on a branch with an open PR whose CI is green** — awaiting human merge.
