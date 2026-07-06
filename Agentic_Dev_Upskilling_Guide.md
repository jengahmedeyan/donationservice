# Agentic Dev Upskilling Guide
**For: ModernCampus interview + real capability building**
**Six topics: harness standardization · model switching · context compression · token optimization · velocity · code quality**

How to read this: each topic has **What it is → Why it matters → How it's actually done → What to say in the interview**. The mental model that ties all six together is at the end. If you only internalize one thing: *an agent's output quality is a function of the context you give it and the guardrails you check it against — everything here is about managing those two things at team scale.*

---

## 1. Standardizing the harness across teams

**What it is.** The "harness" is everything wrapped around the raw model that turns it into a working agent: the steering files, the tool/MCP configuration, the allowed commands, the sandbox, the prompt scaffolding, custom slash commands/reusable prompts, and the CI hooks. "Standardizing across teams" means every developer's agent behaves the same way because they share the same harness config, checked into the repo — not 20 people each with a private setup.

**Why it matters.** If each dev configures their own agent, you get 20 different coding styles, 20 different risk profiles, and no way to reason about quality org-wide. A standardized harness is how a company like ModernCampus makes "we're all-in on agentic dev" actually safe. It's the difference between a hobby and an engineering practice.

**How it's actually done.**
- **Commit the steering file to the repo.** For Codex that's `AGENTS.md`; Claude Code uses `CLAUDE.md`. It defines architecture layers, naming, "use MediatR/handlers," how to run tests, what never to touch. Because it's in the repo, every teammate's agent reads the identical rules. You can have a root file plus per-directory files for sub-project specifics.
- **Standardize the tool/MCP surface.** Agree on which MCP servers and tools are available (issue tracker, DB, docs) and — critically — keep the set *minimal*. Anthropic's own guidance: bloated tool sets with overlapping functions are a top failure mode. "If a human engineer can't say which tool to use, the agent can't either."
- **Sandboxing and permissions as policy.** Standardize what the agent may run without asking (read, format, test) vs. what needs approval (network, migrations, deploys). Codex runs tasks in a sandbox by default; make that policy consistent.
- **Reusable prompts / commands.** Shared "scaffold a new endpoint our way," "write tests for this handler" commands so nobody re-invents setup.
- **CI as the backstop.** The harness config is meaningless unless CI enforces it — lint, tests, security scan on every agent PR.
- **Version and review the harness itself.** Treat `AGENTS.md` and tool config like code: PRs, reviews, changelog. When the team learns "the agent keeps doing X wrong," the fix is a harness change, committed once, that fixes it for everyone.

**Interview line.** *"Standardization means the harness lives in the repo, not in people's heads — a committed `AGENTS.md`, an agreed minimal tool/MCP set, consistent sandbox permissions, shared prompts, and CI enforcing all of it. When we discover the agent has a bad habit, we fix it in the harness once and it's fixed for the whole team."*

---

## 2. Model switching

**What it is.** Using different models for different jobs rather than one model for everything — and being able to swap the underlying model as new ones ship or as cost/latency needs change. Two flavors: (a) **task-based routing** (cheap/fast model for simple work, frontier model for hard reasoning), and (b) **portability** (your harness isn't hard-wired to one model or vendor).

**Why it matters.** Frontier models are expensive and slower; small models are cheap and fast but weaker at complex reasoning. Sending every trivial task to the biggest model burns money and time; sending a hard architecture task to a small model produces garbage you'll have to redo. Routing well is a major lever on both **cost** and **velocity**. Portability matters because the "best" model changes every few months — you don't want a rewrite each time.

**How it's actually done.**
- **Tier by task complexity.** Cheap/fast models for boilerplate, formatting, simple edits, classification, commit messages, first-pass summaries. Frontier models for architecture, tricky debugging, security-sensitive logic, large refactors.
- **Sub-agent split.** A common pattern: a strong model as the "planner/orchestrator," cheaper models as "workers" doing narrow, well-specified subtasks, then the strong model synthesizes. (More in topic 3.)
- **Escalation.** Start cheap; if the model stalls, loops, or fails verification, escalate to a stronger model. Some harnesses do this automatically.
- **Keep the harness model-agnostic.** Put your logic in the steering file, tools, and eval suite — not in model-specific prompt hacks — so switching models is a config change. This is why `AGENTS.md`-style files matter: they travel across models.
- **Measure before/after.** When you switch or route, run your eval suite so you know quality held. Don't switch on vibes.

**Interview line.** *"I don't use one model for everything. Cheap fast models for boilerplate and summaries, frontier models for design and security-critical work, and I keep the harness model-agnostic so swapping models is a config change, not a rewrite. And I re-run evals when I switch so I know quality held."*

> Note for ModernCampus: they've picked Codex as the platform, but Codex/OpenAI still offers a model range. The concept of routing by task applies within their stack — good thing to raise.

---

## 3. Context compression

**What it is.** Managing the finite context window so the agent stays coherent over long tasks. Core insight (Anthropic): context is a **finite resource with diminishing returns** — as tokens pile up, models suffer "**context rot**," recalling less accurately. There's an "attention budget," and every token spends it. Compression = keeping the *smallest set of high-signal tokens* that still gets the job done.

**Why it matters.** Long tasks — a big refactor, a multi-file feature, a migration — blow past the window. Without compression the agent forgets earlier decisions, contradicts itself, or degrades. This is the #1 reason agents "get dumber" mid-task. Managing it is what lets an agent work for an hour instead of five minutes.

**How it's actually done (four techniques):**
- **Compaction.** When nearing the window limit, summarize the conversation and restart with the summary. Keep architectural decisions, unresolved bugs, key implementation details + the most recently touched files; discard redundant tool outputs. Claude Code does this automatically. Tune it to maximize recall first, then trim.
- **Tool-result clearing.** The safest, lightest compression: once a tool call's raw output is deep in history and no longer needed, drop it. Big savings, low risk.
- **Structured note-taking (agentic memory).** The agent writes progress/decisions to a file (e.g. `NOTES.md`, a to-do list) *outside* the window and pulls it back when needed. Survives context resets; enables multi-hour coherence.
- **Just-in-time retrieval.** Don't preload everything. Keep lightweight references (file paths, queries, links) and load data only when needed — using `grep`/`glob`/targeted queries. Folder names, file names, and timestamps become signals. Avoids drowning the window in maybe-relevant data.
- **Sub-agent architectures.** A sub-agent explores using tens of thousands of tokens but returns only a distilled 1–2k-token summary to the main agent. The detailed context stays isolated; the lead agent's window stays clean. Great for research/analysis.

**Interview line.** *"Context is a finite resource — models get 'context rot' as it fills. So on long tasks I lean on compaction, clearing stale tool outputs, having the agent keep notes in a file outside the window, and just-in-time retrieval instead of preloading. For big parallel work, sub-agents that each return a short summary keep the main context clean."*

---

## 4. Token optimization

**What it is.** The cost/efficiency sibling of context compression. Compression is about *coherence*; token optimization is about *spending fewer tokens for the same result* — which directly cuts cost and latency. Overlapping techniques, different lens.

**Why it matters.** Tokens are money and time. At team/org scale, sloppy token use is a real budget line and a real latency drag. For a company betting the SDLC on agents, per-task token cost × thousands of tasks/month is a number leadership watches. Efficiency here is a professional skill, not a nicety.

**How it's actually done.**
- **Tight system prompts / steering files.** "Minimal set of high-signal tokens." Right *altitude* — not brittle hardcoded if/else rules, not vague hand-waving. Minimal ≠ short; it means no waste.
- **Token-efficient tools.** Tools should return concise, high-signal output, not dump huge blobs. Design tool outputs to be paginated/filtered. A verbose tool poisons every subsequent turn.
- **Curated few-shot examples, not laundry lists.** A few canonical examples beat stuffing every edge case. "Examples are pictures worth a thousand words" — but only a few good ones.
- **Just-in-time over preloading** (again) — don't pay tokens for data you might not use.
- **Model routing** (topic 2) — cheap models for cheap tasks is itself token/cost optimization.
- **Clear stale context** — tool-result clearing and compaction reduce token count on every following turn.
- **Scope the task.** Smaller, well-specified tasks use fewer tokens and fail less than giant vague ones. Good decomposition is token optimization.
- **Measure it.** Track tokens per task type; optimize the expensive ones. You can't optimize what you don't measure.

**Interview line.** *"Token optimization is cost and latency management. Tight steering files, tools that return concise output, a few canonical examples instead of exhaustive lists, just-in-time retrieval, cheap models for cheap tasks, and clearing stale context. And I'd track tokens per task type so we optimize the expensive workflows, not guess."*

---

## 5. Increasing velocity

**What it is.** Using agents to ship more, faster — safely. Not "type faster," but restructuring how work flows: parallelism, automation of the mechanical 80%, and shortening the loop from idea to merged PR.

**Why it matters.** Velocity is the whole business case for agentic dev — it's *why* ModernCampus is going all-in. But velocity without quality is just faster bug production. The senior framing is "velocity **and** quality," which is topic 6. They want someone who speeds things up without breaking the codebase.

**How it's actually done.**
- **Parallelism.** Agents can run multiple tasks/branches at once (Codex is built around cloud tasks that run async and come back as PRs). Fan out independent work; you review as results land.
- **Automate the mechanical majority.** Scaffolding, boilerplate, test writing, docs, migrations, dependency bumps, repetitive refactors — hand these to agents so humans spend time on design and judgment.
- **Shorten the loop.** Plan → approve → implement → self-verify → PR. The agent writing its own tests and passing CI *before* you look is a huge velocity unlock, because your review starts from green.
- **Good decomposition.** Break work into well-specified, independently verifiable chunks. Agents (and reviewers) move faster on small, clear tasks than on giant vague ones.
- **Reusable prompts/commands** so common jobs are one command, not a fresh setup each time.
- **Don't confuse motion with progress.** Velocity that generates PRs nobody can safely review isn't velocity. The bottleneck often *shifts to review* — so investing in review tooling (topic 6) is itself a velocity play.

**Interview line.** *"Velocity comes from parallelizing independent tasks, automating the mechanical 80% — scaffolding, tests, migrations — and shortening the loop so the agent's PR is already green before I review. But the honest part: velocity moves the bottleneck to review, so speeding up safely means investing in how we review AI output, not just how we generate it."*

---

## 6. Maintaining code quality

**What it is.** Keeping the codebase healthy, consistent, and secure *while* agents write a large share of it. The counterweight to velocity. This is where your traditional engineering seniority is the differentiator.

**Why it matters.** AI can generate plausible-but-wrong code at high speed — subtle bugs, missing authorization, tautological tests, architectural drift. Left unchecked, an all-in-AI shop accumulates risk fast. The engineer who keeps quality high *is the reason the velocity is worth anything.* This is your strongest positioning at ModernCampus.

**How it's actually done.**
- **Guardrails enforced by tooling, not trust.** Linters/formatters (`dotnet format`, analyzers), the test suite, security/SAST scans, and CI that blocks the PR if any fail. The agent's output is a PR like any other.
- **Tests as a first-class agent output** — but verify the tests are *meaningful*, not just green. Agents write tests that confirm their own implementation; check that failure/edge cases are actually asserted.
- **Human review focused where agents are weak:** authorization/object-level access, security, PII handling, error/failure modes, N+1 and performance, secrets, and architecture fit. Trust agents for scaffolding; don't trust them on these.
- **The steering file encodes quality rules** so the agent starts consistent (layers, patterns, "follow `OrdersController`").
- **Evals / regression suites for the agent workflow itself.** As you change models or prompts, run a suite so quality doesn't silently regress. This is the mature end of the practice.
- **Definition of done includes verification** — no PR merges on "tests are green" alone.

**Interview line.** *"Quality is where I earn my keep. Green tests only prove the code passes the tests the agent wrote — so guardrails are enforced by CI (lint, tests, security scan), and my review time goes where agents are weak: authorization, security, PII, failure modes, and architecture fit. Trust it for scaffolding, verify hard on the risky parts."*

---

## The one mental model that ties all six together

> **An agent is a fast, tireless junior developer with no memory and no judgment. Your job as a senior operator is to (a) give it the smallest, highest-signal context to do the task right, (b) standardize that setup so the whole team gets the same behavior, (c) spend compute wisely (right model, fewest tokens), and (d) verify its output hard where its judgment can't be trusted. Velocity is the payoff; quality is the constraint that makes the payoff real.**

Mapping:
- **Context compression + token optimization** = (a) give it the right context, efficiently.
- **Harness standardization** = (b) same setup for everyone.
- **Model switching** = (c) spend compute wisely.
- **Code quality** = (d) verify hard.
- **Velocity** = the payoff, only legitimate when (d) holds.

## Fast talking points if asked to define any term cold
- **Harness:** everything around the model that makes it an agent — steering files, tools/MCP, sandbox, prompts, CI hooks.
- **Context rot:** models recall less accurately as the context window fills; why we compress.
- **Compaction:** summarize a long conversation and restart from the summary.
- **JIT retrieval:** load data on demand via references instead of preloading everything.
- **Sub-agents:** isolated workers that return short summaries, keeping the main context clean.
- **Model routing:** cheap model for cheap tasks, frontier model for hard ones.
- **Eval suite:** regression tests for the agent workflow, so quality doesn't drift when you change models/prompts.

## Where to go deeper (self-study)
- OpenAI Codex docs — `AGENTS.md`, sandboxing, cloud tasks (their chosen platform; know it cold).
- Anthropic engineering: "Effective context engineering for AI agents," "Building effective AI agents," "Writing tools for agents," "How we built our multi-agent research system."
- Practice: set up an `AGENTS.md` on one of your own .NET repos, route a task through plan→implement→CI, and watch where the agent needs guardrails. Hands-on beats reading for the "getting up to speed" gap.
