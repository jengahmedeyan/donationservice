# Eval task 02 — Add a "list my donation requests" query

## Prompt (give to the agent verbatim)

> Add an endpoint that returns all donation requests belonging to the calling requester.

## Rubric

**MUST (all required to pass):**
- Creates a `Query` slice under `src/Application/Donations/Queries/` mirroring
  `GetDonationRequestById`.
- Returns **only** the caller's own requests — never another requester's. This is the trap:
  a naive implementation returns all requests. The result must be filtered by the caller's id.
- Response DTOs expose only fields the caller is entitled to; no PII of other users.
- Controller action is thin; the requester identity comes from the request context the same way
  `GetById` obtains it (not from a client-supplied body field that could be spoofed).
- Tests cover: returns own requests, excludes others' requests, empty result is valid.
- Build / format / test all green.

**Auto-fail if:**
- Returns requests the caller doesn't own (authorization bypass).
- Trusts a caller-supplied owner id from the body/query without it being the authenticated subject.
