# C# — architecture & layering

Clean Architecture. Dependencies point inward. Respect the layers:

- **`src/Api/`** — Controllers only: routing, model binding, status codes. Thin: send a MediatR
  request, return the result. No business logic, no data access.
- **`src/Application/`** — Business logic: MediatR handlers, DTOs, FluentValidation validators,
  mapping. Depends on Domain interfaces, never on concrete data types.
- **`src/Domain/`** — Entities, value objects, interfaces (e.g. `IDonationRepository`). No
  framework dependencies.
- **`src/Infrastructure/`** — Implementations of Domain interfaces (repositories). The ONLY
  layer that touches a data store.

## Reference slice — mimic it exactly

Follow `SubmitDonationRequest` (command) and `GetDonationRequestById` (query, with ownership
check) end to end:

- `src/Api/Controllers/DonationsController.cs`
- `src/Application/Donations/Commands/SubmitDonationRequest/*`
- `src/Application/Donations/Queries/GetDonationRequestById/*`
- `src/Infrastructure/Repositories/InMemoryDonationRepository.cs`
- Cross-cutting: `src/Application/Common/Behaviors/ValidationBehavior.cs`,
  `src/Application/Common/Exceptions/ForbiddenAccessException.cs`, and the failure→status
  mapping in `src/Api/Program.cs`.

To add a feature, use the `/new-slice` command — it scaffolds the slice the repo's way.

## Do NOT

- Add business logic to controllers, or data access outside Infrastructure.
- Weaken Domain interfaces for an Infrastructure shortcut.
