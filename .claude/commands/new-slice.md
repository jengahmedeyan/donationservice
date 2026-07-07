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
2. **Plan-and-approve gate — do not skip.** `EnterPlanMode`, draft the plan (files to add, the
   ownership/PII handling, the test cases), and `ExitPlanMode` to get explicit approval. Do not
   create a branch or write any file until approved. See
   `rules/common/git-workflow.md#plan-and-approve-gate`.
3. Once approved: create a feature branch off `main` (e.g. `feature/{feature-name}`) — never work
   on `main`.
4. Create the folder under `src/Application/Donations/{Commands|Queries}/{FeatureName}/`:
   - The `record` request implementing `IRequest<TResponse>`.
   - One handler per file, constructor-injecting `IDonationRepository`, async with `CancellationToken`.
   - A FluentValidation validator for every request that carries input.
5. If the slice touches a specific record, enforce **object-level authorization**: not-found → return null/404,
   wrong owner → throw `ForbiddenAccessException` (mirror `GetDonationRequestByIdHandler`).
6. Wire the endpoint into `DonationsController.cs` — thin: send the MediatR request, map the result. No logic in the controller.
7. Add xUnit tests mirroring `GetDonationRequestByIdHandlerTests`: happy path AND failure/edge cases
   (invalid input, unauthorized, not-found). Assert real behavior, not just non-null.
8. Respect PII rules: never log `RequesterName`/`Location`/`RequesterId`; log the `Id`.
9. Run `/verify`. Only proceed when build, format, and tests are all green.
10. Push the branch and open a PR against `main` (fill in the PR template). Do NOT merge it —
    a human reviews and merges. Report the PR link.

Keep the change minimal — do not refactor unrelated code.
