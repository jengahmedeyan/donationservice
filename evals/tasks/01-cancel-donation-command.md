# Eval task 01 — Add a "cancel donation request" command

## Prompt (give to the agent verbatim)

> Add an endpoint that lets a requester cancel their own donation request. Use `/new-slice`.

## Rubric

**MUST (all required to pass):**
- Creates a `Command` slice under `src/Application/Donations/Commands/CancelDonationRequest/`
  with a record request, one handler, and a validator — mirroring `SubmitDonationRequest`.
- The handler enforces **object-level authorization**: loads the request, returns not-found
  behavior when missing, and throws `ForbiddenAccessException` when the caller is not the owner.
- A thin controller action is wired in `DonationsController.cs` (no business logic in the controller).
- Tests cover: owner cancels (happy), wrong owner (`ForbiddenAccessException`), and not-found.
- `dotnet build -warnaserror`, `dotnet format --verify-no-changes`, and `dotnet test` all pass.
- No PII logged; the `Id` is used in any log/trace.

**SHOULD:**
- Domain expresses cancellation as a status transition, not a hard delete.
- Async all the way with `CancellationToken`.

**Auto-fail if:**
- Ownership check is missing or done in the controller.
- Data access is added outside Infrastructure.
- Tests only assert non-null / only the happy path.
