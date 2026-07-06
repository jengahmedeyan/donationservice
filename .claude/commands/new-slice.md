---
description: Scaffold a new MediatR vertical slice (command or query) following the reference pattern
argument-hint: <Command|Query> <FeatureName> e.g. "Command CancelDonationRequest"
---

Scaffold a new vertical slice for: **$ARGUMENTS**

Follow the repository's reference slice **exactly** — do not invent a new structure. Use
just-in-time retrieval: read only the reference files you need, mirror them, don't inline them
into your reasoning wholesale.

## Reference to mirror

- Command example: `src/Application/Donations/Commands/SubmitDonationRequest/` (Command + Handler + Validator)
- Query example (with ownership check): `src/Application/Donations/Queries/GetDonationRequestById/`
- Controller wiring: `src/Api/Controllers/DonationsController.cs`
- Tests to mirror: `tests/Application.Tests/Donations/*`

## Steps

1. Confirm the slice type (Command vs Query) and feature name from the arguments. If ambiguous, ask.
2. Create the folder under `src/Application/Donations/{Commands|Queries}/{FeatureName}/`:
   - The `record` request implementing `IRequest<TResponse>`.
   - One handler per file, constructor-injecting `IDonationRepository`, async with `CancellationToken`.
   - A FluentValidation validator for every request that carries input.
3. If the slice touches a specific record, enforce **object-level authorization**: not-found → return null/404,
   wrong owner → throw `ForbiddenAccessException` (mirror `GetDonationRequestByIdHandler`).
4. Wire the endpoint into `DonationsController.cs` — thin: send the MediatR request, map the result. No logic in the controller.
5. Add xUnit tests mirroring `GetDonationRequestByIdHandlerTests`: happy path AND failure/edge cases
   (invalid input, unauthorized, not-found). Assert real behavior, not just non-null.
6. Respect PII rules: never log `RequesterName`/`Location`/`RequesterId`; log the `Id`.
7. Run `/verify`. Only report done when build, format, and tests are all green.

Keep the change minimal — do not refactor unrelated code.
