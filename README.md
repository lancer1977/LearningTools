# LearningTools / Polyhydra Trivia

LearningTools is being modernized from the legacy SpellingTest-era application into the Polyhydra Trivia platform.

[![Polyhydra Trivia](https://github.com/lancer1977/LearningTools/actions/workflows/polyhydra-trivia.yml/badge.svg)](https://github.com/lancer1977/LearningTools/actions/workflows/polyhydra-trivia.yml)

## Tags

- learning-tools
- dotnet
- docs
- learning
- ui
- mod

## Overview
This repository contains the LearningTools project. It is designed to provide robust functionality and seamless integration within its ecosystem.

## 🚀 Key Features
- General Purpose Utility
- Core Application Logic
- Sub Module Spellingtest Blazor
- Standardized Project Layout
- Core Capabilities
- Sub Module Spellingtest Core
- Sub Module Spellingtest Wasm
- Polyhydra Software Dashboard
- Polyhydra Trivia Core Domain
- Polyhydra Trivia API
- Polyhydra Trivia Web
- [Feature 3 (Beyond the App capability)]

## 🛠 Technology Stack
- C# / .NET 10

## 📖 Documentation
Detailed documentation can be found in the following sections:
- [Docs Index](./docs/README.md)
- [Feature Index](./docs/features/README.md)
- [Core Capabilities](./docs/features/core-capabilities.md)
- [Roadmap Index](./docs/roadmaps/README.md)

## 🚦 Getting Started

The validated modernization surface is the new `Polyhydra.Trivia.slnx` solution. It uses only public NuGet packages and does not require the legacy Azure Artifacts feed.

```bash
dotnet restore Polyhydra.Trivia.slnx --configfile NuGet.Public.config
dotnet build Polyhydra.Trivia.slnx --configuration Release --no-restore
dotnet test Polyhydra.Trivia.slnx --configuration Release --no-build
dotnet run --project src/Polyhydra.Trivia.Api/Polyhydra.Trivia.Api.csproj
dotnet run --project src/Polyhydra.Trivia.Web/Polyhydra.Trivia.Web.csproj
```

Run the API and web app together for local UI validation. The web app reads `TriviaApi:BaseUrl` from configuration and defaults to `http://localhost:5147`.

Initial web routes:

- `/`, `/trivia`, `/topics`, `/topics/{slug}`, and `/play`
- `/admin/topics` and `/admin/questions`
- `/stream/poll`, `/stream/results`, `/overlay/trivia`, and `/overlay/poll-results`

The original `SpellingTest.*` projects remain in the repository for migration continuity. They still depend on private Polyhydra packages from the Azure Artifacts feed in `NuGet.Config`, so they are not part of the public-feed CI path yet.

## Artifacts

The `Polyhydra Trivia` GitHub Actions workflow restores, builds, tests, packs `Polyhydra.Trivia.Core`, publishes the API output, and uploads test/package/API artifacts as workflow artifacts. The separate `Publish Packages` workflow publishes tagged releases of `Polyhydra.Trivia.Core` to GitHub Packages and NuGet.org.

Release versions follow semantic tags such as `v1.2.3`; the workflow strips the leading `v` when packing.

To restore a package from GitHub Packages locally, add a user-level NuGet source that has `read:packages` access. One example is:

```bash
dotnet nuget add source https://nuget.pkg.github.com/lancer1977/index.json \
  --name github \
  --username lancer1977 \
  --password <your-read-packages-token> \
  --store-password-in-clear-text
```

## Persistence

The API uses SQLite through `src/Polyhydra.Trivia.Infrastructure`. Local development defaults to `polyhydra-trivia-dev.db`; non-development runs default to `polyhydra-trivia.db`. Override the path with `ConnectionStrings:Trivia`.

On startup the API creates the SQLite schema with EF Core `EnsureCreated` and imports reviewed seed files from `seed/trivia/*.json` when the database is empty or seed rows need to be updated. The first curated seed file is `seed/trivia/modern-dotnet.json`.

The persistence approach is documented in [Decision Record 0001](./docs/decisions/0001-trivia-persistence-and-seed-content.md). Production schema upgrades should move from `EnsureCreated` to explicit EF Core migrations before the database contains data that cannot be recreated.
