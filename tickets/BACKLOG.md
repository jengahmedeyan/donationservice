# DonationService — ticket backlog

Sample work items for exercising the agentic-dev harness end to end
(`/new-slice` → `/verify` → `/review-security` → CI → `evals/`). Each ticket is
written so an agent can pick it up cold. Ordered roughly easiest → hardest.

Legend: **Type** = Feature / Bug / Chore · **Slice** = the vertical slice it maps to ·
**Eval** = matching golden task in `evals/tasks/` if one exists.

---

## DON-1 — Cancel my donation request
- **Type:** Feature · **Slice:** Command · **Eval:** `01-cancel-donation-command.md`
- **Why:** A requester whose need is met should be able to cancel their request.
- **Description:** Add `POST /api/donations/{id}/cancel` that lets the authenticated
  requester cancel a request they own. Cancellation is a status transition
  (`Open → Cancelled`), not a delete.
- **Acceptance criteria:**
  - New `CancelDonationRequest` command slice mirroring `SubmitDonationRequest`.
  - Owner can cancel → returns the updated DTO with `Status = Cancelled`.
  - Non-owner → `ForbiddenAccessException` (403). Unknown id → 404.
  - Cancelling an already-cancelled request is rejected cleanly (not a 500).
  - Tests: happy path, wrong owner, not-found, already-cancelled.
- **Security notes:** Object-level ownership check required. Don't log PII (log the `Id`).
- **Good first ticket** — smallest slice, and it validates the `/new-slice` command
  *and* the eval rubric together.

---

## DON-2 — List my donation requests
- **Type:** Feature · **Slice:** Query · **Eval:** `02-list-my-donations-query.md`
- **Why:** Requesters need to see everything they've submitted.
- **Description:** Add `GET /api/donations` returning all requests belonging to the
  calling requester.
- **Acceptance criteria:**
  - New query slice mirroring `GetDonationRequestById`.
  - Returns **only** the caller's own requests — never anyone else's.
  - Empty list is a valid 200 result, not a 404.
  - Requester identity comes from the request context the same way `GetById` gets it
    — not from a spoofable body/query field.
  - Tests: returns own, excludes others', empty result.
- **Security notes:** The trap here is an authorization bypass that returns all rows.
  This is the highest-value `/review-security` exercise.

---

## DON-3 — Mark a donation request as fulfilled
- **Type:** Feature · **Slice:** Command
- **Why:** Track when a need has been met.
- **Description:** Add `POST /api/donations/{id}/fulfill` transitioning `Open → Fulfilled`.
- **Acceptance criteria:**
  - Owner-only transition; 403 for others, 404 for unknown id.
  - Only an `Open` request can be fulfilled; other states rejected with a 400-style
    problem, never a 500.
  - Tests cover each transition guard.
- **Security notes:** Ownership check; consider whether "fulfilled" should ever be
  reversible (document the decision).

---

## DON-4 — Validate and reject empty/oversized fields on submit
- **Type:** Bug · **Slice:** Command (existing)
- **Why:** Confirm the validation pipeline actually blocks bad input at the edge.
- **Description:** Audit `SubmitDonationRequestValidator` against the acceptance
  criteria below and add any missing rules + tests.
- **Acceptance criteria:**
  - `RequesterId`, `RequesterName`, `ItemNeeded`, `Location` all required and length-bounded.
  - Whitespace-only values are rejected (not just empty strings).
  - Validation failures return 400 problem-details with field-level errors, never 500,
    and never echo PII back in a way a log would capture.
  - Add validator tests asserting the specific failures.
- **Security notes:** Error responses must not leak internal detail or unentitled PII.

---

## DON-5 — Paginate the "list my requests" endpoint
- **Type:** Feature · **Slice:** Query (depends on DON-2)
- **Why:** A prolific requester's list shouldn't return unbounded rows.
- **Description:** Add `page` and `pageSize` query params with sane defaults and a max cap.
- **Acceptance criteria:**
  - Defaults (e.g. page 1, size 20) and an enforced max page size.
  - Still filtered to the caller's own requests (DON-2's rule holds under pagination).
  - Invalid paging params → 400, not a 500 or silent clamp-without-note.
  - Tests: default paging, custom paging, out-of-range params, ownership still enforced.
- **Security notes:** Pagination must not become an authorization bypass.

---

## How to run a ticket through the harness
1. Pick a ticket. For a new slice: `/new-slice Command CancelDonationRequest` (or Query).
2. Let the agent plan, then implement following the reference slice.
3. `/verify` — build (`-warnaserror`), format, tests must all pass.
4. `/review-security` — confirm authorization + PII before handing back.
5. Commit and push; confirm CI is green.
6. If the ticket has a matching eval, grade the result against `evals/tasks/*` to
   check the harness itself, not just the code.
