# Polyhydra Trivia Platform

## Purpose

Polyhydra Trivia is the new .NET 10 platform surface for modernizing LearningTools beyond the legacy SpellingTest application. The first validated slice creates a public-feed build path for the core trivia domain, API endpoints, and tests.

## Current Solution

- `Polyhydra.Trivia.slnx`
- `src/Polyhydra.Trivia.Core`
- `src/Polyhydra.Trivia.Api`
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

The API currently provides an in-memory implementation for the first happy path:

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

Persistence, authentication, admin UI, Twitch, and Channel Cheevos adapters remain separate follow-up issues.

## Validation

```bash
dotnet restore Polyhydra.Trivia.slnx --configfile NuGet.Public.config
dotnet build Polyhydra.Trivia.slnx --configuration Release --no-restore
dotnet test Polyhydra.Trivia.slnx --configuration Release --no-build
dotnet pack src/Polyhydra.Trivia.Core/Polyhydra.Trivia.Core.csproj --configuration Release --no-build --output artifacts/packages
dotnet publish src/Polyhydra.Trivia.Api/Polyhydra.Trivia.Api.csproj --configuration Release --no-build --output artifacts/api
```
