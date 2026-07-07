# MCP servers

Project-scoped MCP servers live in `.mcp.json` at the repo root. Claude Code discovers it and
asks each user to approve project servers before they load, so committing the config is safe —
no server runs without explicit approval.

## GitHub

`.mcp.json` declares the hosted GitHub MCP server for reading issues and PRs from inside a
session. It authenticates with a Personal Access Token supplied via an environment variable —
**no token is committed**:

```shell
# set once in your shell profile (a fine-grained PAT with repo scope)
export GITHUB_MCP_PAT=ghp_your_token_here
```

Then approve the `github` server when Claude Code prompts on first use.

## Note: `gh` is still the primary PR tool

PR creation goes through the `gh` CLI via the `/open-pr` command — it's already authenticated and
needs no extra setup. The GitHub MCP server is additive: it lets the agent *read* issue/PR context
during a task. Keep PR *creation* on `/open-pr` so the branch → PR → human-merge workflow stays
consistent and scriptable.
