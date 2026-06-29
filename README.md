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
```

The original `SpellingTest.*` projects remain in the repository for migration continuity. They still depend on private Polyhydra packages from the Azure Artifacts feed in `NuGet.Config`, so they are not part of the public-feed CI path yet.

## Artifacts

The `Polyhydra Trivia` GitHub Actions workflow restores, builds, tests, packs `Polyhydra.Trivia.Core`, publishes the API output, and uploads test/package/API artifacts as workflow artifacts. Publishing to GitHub Packages or NuGet.org is intentionally deferred until package ownership, release versioning, and required secrets are finalized.
