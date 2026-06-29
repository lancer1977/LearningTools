# Decision Record: Trivia persistence and seed content

## Status

Proposed.

## Context

The current `Polyhydra.Trivia.Api` uses an in-memory store so the domain, API, and web UI can be validated without private infrastructure. That is useful for modernization, but it does not satisfy durable topic, definition, fact, question, or session storage.

## Decision

Use SQLite plus EF Core for local development and first production-sized persistence work. Keep the core domain free of EF Core attributes and put database-specific mapping in a future infrastructure project.

Seed content should live as versioned JSON fixtures that can be imported into the database during local setup or test seeding. This keeps curated trivia content reviewable in pull requests while allowing the API to persist runtime edits.

## Initial shape

- `src/Polyhydra.Trivia.Infrastructure` will own EF Core mappings, migrations, and repository implementations.
- `src/Polyhydra.Trivia.Api` will depend on abstractions rather than concrete in-memory lists.
- `seed/trivia/*.json` will hold reviewed topics, definitions, facts, and questions.
- Tests should cover migration creation, seed import, repository round trips, and API persistence beyond process memory.

## Validation plan

1. Add infrastructure tests that create a temporary SQLite database.
2. Run migrations against the temporary database.
3. Import seed JSON.
4. Restart the repository/service scope and verify topics and questions survive.
5. Keep the current in-memory API tests as fast endpoint contract coverage.

## Follow-up work

- Implement the infrastructure project and repository abstractions.
- Add the seed JSON schema and first curated seed file.
- Document local database setup in the README once the implementation lands.
