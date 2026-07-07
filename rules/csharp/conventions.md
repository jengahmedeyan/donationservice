# C# — conventions, validation & errors

## Conventions

- Async all the way; I/O methods end in `Async` and take a `CancellationToken`.
- Records for DTOs/value objects. Nullable reference types stay enabled.
- One handler per file. Small, single-purpose methods. File-scoped namespaces.
- No magic strings for config; bind to typed options.

## Validation & errors

- Every request DTO has a FluentValidation validator in `Application/`. The `ValidationBehavior`
  pipeline runs it before the handler.
- Validation failures return 400 problem-details, never 500. Ownership failures throw
  `ForbiddenAccessException` → 403. Invalid state transitions throw `InvalidDonationStateException`
  → 409. Mapping lives in `Program.cs`.
- Never return raw exception messages or stack traces to callers.

## Do NOT

- Disable nullable/analyzers/tests to make a build pass.
