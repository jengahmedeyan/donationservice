#!/usr/bin/env bash
# PreToolUse(Bash) hook: refuse commits made directly on `main`.
# Enforces the branch -> PR -> human-merge workflow (see CLAUDE.md) in code, not just docs.
# Reads the tool-call JSON on stdin; exit 2 blocks the tool call and shows the message to the agent.
set -uo pipefail

input=$(cat)

# Only care about git commit calls. Match on the raw payload to stay dependency-free (no jq).
case "$input" in
  *"git commit"*)
    branch=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo "")
    if [ "$branch" = "main" ]; then
      echo "Blocked: do not commit directly to 'main'. Create a feature branch (feature/…, fix/…, chore/…) and open a PR — a human merges. See CLAUDE.md > Workflow." >&2
      exit 2
    fi
    ;;
esac

exit 0
