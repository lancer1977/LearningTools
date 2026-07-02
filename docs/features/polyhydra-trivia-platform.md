# Polyhydra Trivia Platform

## Purpose

Polyhydra Trivia is the new .NET 10 platform surface for modernizing LearningTools beyond the legacy SpellingTest application. The first validated slice creates a public-feed build path for the core trivia domain, API endpoints, and tests.

## Current Solution

- `Polyhydra.Trivia.slnx`
- `src/Polyhydra.Trivia.Core`
- `src/Polyhydra.Trivia.Api`
- `src/Polyhydra.Trivia.Infrastructure`
- `tests/Polyhydra.Trivia.Tests`
- `.github/workflows/polyhydra-trivia.yml`

The new solution uses `NuGet.Public.config` so restore, build, test, pack, and publish-artifact validation can run without Azure Artifacts credentials.

## Domain Model

The core library currently models:

- topics with normalized slugs and tags
- facts tied to topics with difficulty and confidence
- definitions tied to topics with examples
- multiple-choice questions with explanations, source facts, difficulty, and content lifecycle status
- game sessions for solo, chat poll, streamer-vs-chat, and trivia-night modes
- poll vote collection, duplicate-voter replacement, majority calculation, hidden results before reveal, and correct-answer reveal

Generated or draft questions cannot be published directly. Questions must move through human review before publication.

## API Surface

The API currently provides a SQLite-backed implementation for:

- `GET /openapi/v1.json` in development
- `GET /api/topics`
- `GET /api/topics/{slug}`
- `POST /api/topics`
- `PUT /api/topics/{id}`
- `GET /api/topics/{id}/facts`
- `POST /api/topics/{id}/facts`
- `GET /api/topics/{id}/definitions`
- `POST /api/topics/{id}/definitions`
- `GET /api/topics/{id}/questions`
- `POST /api/topics/{id}/questions`
- `POST /api/game-sessions`
- `GET /api/game-sessions/{id}`
- `POST /api/game-sessions/{id}/answer`
- `POST /api/game-sessions/{id}/reveal`
- `GET /api/game-sessions/{id}/results`

Authentication, full admin UI protection, Twitch, and Channel Cheevos adapters remain separate follow-up issues.

## Persistence

The API depends on an `ITriviaStore` abstraction backed by EF Core SQLite. `Polyhydra.Trivia.Infrastructure` owns the DbContext, entity mappings, startup schema creation, and JSON seed import.

- Local development defaults to `polyhydra-trivia-dev.db`.
- Non-development runs default to `polyhydra-trivia.db`.
- `ConnectionStrings:Trivia` overrides the database location.
- Reviewed seed content lives in `seed/trivia/*.json`.
- Startup schema management uses EF Core `EnsureCreated`; production upgrade work should replace this with explicit migrations before long-lived data is introduced.

## Packaging

`Polyhydra.Trivia.Core` is the current packable library. Tagged releases use semantic version tags such as `v1.2.3`; the release workflow strips the leading `v` before publishing.

- GitHub Packages publishes use the repository `GITHUB_TOKEN`.
- NuGet.org publishes use the `NUGET_API_KEY` secret.
- Local GitHub Packages restore requires a user-level NuGet source with `read:packages` access.
- The public-feed CI path restores with `NuGet.Public.config` so development does not need Azure Artifacts credentials.

## Validation

```bash
dotnet restore Polyhydra.Trivia.slnx --configfile NuGet.Public.config
dotnet build Polyhydra.Trivia.slnx --configuration Release --no-restore
dotnet test Polyhydra.Trivia.slnx --configuration Release --no-build
dotnet pack src/Polyhydra.Trivia.Core/Polyhydra.Trivia.Core.csproj --configuration Release --no-build --output artifacts/packages
dotnet publish src/Polyhydra.Trivia.Api/Polyhydra.Trivia.Api.csproj --configuration Release --no-build --output artifacts/api
```
