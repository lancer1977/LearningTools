# Decision Record: Trivia persistence and seed content

## Status

Accepted for the first durable persistence slice.

## Context

The first `Polyhydra.Trivia.Api` slice used an in-memory store so the domain, API, and web UI could be validated without private infrastructure. That was useful for modernization, but it did not satisfy durable topic, definition, fact, question, or session storage.

## Decision

Use SQLite plus EF Core for local development and first production-sized persistence work. Keep the core domain free of EF Core attributes and put database-specific mapping in `src/Polyhydra.Trivia.Infrastructure`.

Seed content should live as versioned JSON fixtures that can be imported into the database during local setup or test seeding. This keeps curated trivia content reviewable in pull requests while allowing the API to persist runtime edits.

## Initial shape

- `src/Polyhydra.Trivia.Infrastructure` owns EF Core mappings, startup schema creation, and seed import.
- `src/Polyhydra.Trivia.Api` depends on `ITriviaStore` rather than concrete in-memory lists.
- `seed/trivia/*.json` holds reviewed topics, definitions, facts, and questions.
- Tests cover schema creation, seed import, repository round trips, and API persistence beyond process memory.
- EF Core `EnsureCreated` is the initial schema-management process; replace it with explicit migrations before production data needs in-place upgrades.

## Validation plan

1. Add infrastructure tests that create a temporary SQLite database.
2. Create the SQLite schema against the temporary database.
3. Import seed JSON.
4. Restart the repository/service scope and verify topics and questions survive.
5. Keep the current in-memory API tests as fast endpoint contract coverage.

## Follow-up work

- Replace `EnsureCreated` with explicit EF Core migrations before production data is long-lived.
- Add more curated trivia seed files as content review catches up.
- Add operational backup/restore notes once a production deployment target exists.
