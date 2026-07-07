# Testing requirements

- Every handler: happy path AND failure/edge cases (invalid input, unauthorized, not-found,
  invalid state). See `GetDonationRequestByIdHandlerTests` and `CancelDonationRequestHandlerTests`
  for the shape.
- Assert real behavior, not just non-null. A test that only checks `NotBeNull()` is not enough.
- Verify mutations persisted (`UpdateAsync` called `Times.Once`) — and were NOT called on
  rejection paths (`Times.Never`).
- Mock repositories with Moq; no real DB in unit tests.

The `tdd-guide` subagent enumerates the required cases for a given slice and audits existing tests
for weak/tautological assertions.
