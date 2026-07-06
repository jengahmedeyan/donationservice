# Agent evals

Regression tests for the **agent workflow itself** — not for the app. Run these when you change
the model, the steering file (`CLAUDE.md`), or a command, so quality doesn't silently drift.

This is what makes model switching (guide topic 2) safe: swap the model, re-run the evals, and
you have evidence quality held instead of a vibe.

## Two layers

1. **`check-guardrails.sh`** — automated, deterministic structural checks (PII-in-logs,
   controllers staying thin, tests exist). Runs in seconds; wired into CI. No model needed.
2. **`tasks/*.md`** — golden agent tasks. Each has a prompt you give the agent and a rubric you
   grade the result against. These catch behavioral regressions the static checks can't.

## How to run

Automated layer:

```shell
bash evals/check-guardrails.sh
```

Golden-task layer (manual / semi-automated):

1. Check out a clean tree.
2. Give the agent the task prompt from a `tasks/*.md` file verbatim.
3. Grade the resulting diff against that file's rubric. Every "MUST" item must pass.
4. Record the score in a table (model, date, task, pass/fail). Compare across model changes.

## Suggested cadence

- Before adopting a new default model or a `CLAUDE.md` change: run all tasks.
- In CI on every PR: `check-guardrails.sh` only (fast, deterministic).
