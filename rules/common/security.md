# Security & data handling (non-negotiable)

These are the rules AI-generated code most often gets wrong. They are non-negotiable.

- **Authorization, not just authentication:** a user may only read/modify their OWN donation
  requests. Every record-specific endpoint/handler checks ownership (see
  `GetDonationRequestByIdHandler`: not-found → null, wrong owner → `ForbiddenAccessException`).
  Enforce ownership **before** any state-specific behavior, so record state never leaks to
  non-owners.
- **PII:** `RequesterName`, `Location`, and `RequesterId` are sensitive. Never log them (log the
  donation `Id`), never expose fields the caller isn't entitled to, never put them in error
  messages. Responses use `DonationRequestDto`, which omits `RequesterId`.
- Parameterized queries / LINQ only. No secrets hardcoded.
- Identity comes from the request context, never from a spoofable request-body field.

## Enforcement

- `/security-scan` — deterministic checks (`evals/check-guardrails.sh`): PII-in-logs, layering,
  handler test coverage.
- `/review-security` — the `security-reviewer` subagent's judgment pass.

Run both on any change touching a handler or endpoint before opening/merging a PR.

## Do NOT

- Commit secrets or real customer data.
