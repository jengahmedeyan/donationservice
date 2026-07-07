#!/usr/bin/env bash
# Stop hook: two jobs when the agent finishes a turn.
#   1. Keep the working tree formatted (auto `dotnet format`), so handoff is always clean.
#   2. Append a short session note to a local (git-ignored) log — lightweight agentic memory.
#
# Why format on Stop rather than on every edit: `dotnet format` loads the whole project (multiple
# seconds), so running it per-edit would cripple velocity. Once-per-turn keeps the tree formatted
# without that tax. CI's `dotnet format --verify-no-changes` remains the hard backstop.
set -uo pipefail

repo_root=$(git rev-parse --show-toplevel 2>/dev/null || pwd)
cd "$repo_root" || exit 0

# 1. Best-effort format. Never fail the hook (|| true) — a formatting hiccup must not block the agent.
if command -v dotnet >/dev/null 2>&1; then
  dotnet format >/dev/null 2>&1 || true
fi

# 2. Session note (git-ignored; see .gitignore).
branch=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo "?")
note_file="$repo_root/.claude/session-notes.local.md"
{
  echo "- $(date '+%Y-%m-%d %H:%M') · branch \`$branch\` · $(git rev-parse --short HEAD 2>/dev/null || echo '-')"
} >> "$note_file" 2>/dev/null || true

exit 0
