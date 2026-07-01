# LearningTools portfolio roadmap

This local planning file has been migrated. GitHub Issues are the canonical tracker.

Canonical GitHub issue: https://github.com/lancer1977/LearningTools/issues/22
Original source kind: roadmap

## Current repo posture
- Stack: .NET
- Docs folder: yes
- Roadmap folder: no
- Features docs: yes
- Tests indexed: no

## Discovery
- [x] Capture and timestamp recent change signal
- [x] Capture top-level area concentration
- [x] Document owner and intent for area: docs(9)
- [x] Add explicit release gates for next validation steps

## V1 (stability)
- [x] Close gaps in docs and feature notes for recently touched areas
- [x] Add or update smoke checks for changed source paths
- [x] Validate packaging and deploy assumptions where infra/config changed

## Release gates

- The validated modernization path is `Polyhydra.Trivia.slnx`.
- CI restores with `NuGet.Public.config` so public PR validation does not require private Azure Artifacts credentials.
- Local validation must run restore, build, test, pack, and API publish before closing implementation issues.
- The legacy `SpellingTest.*` projects remain migration inputs until the private package/feed dependency is removed or documented with credentials.

## V2 (confidence)
- [ ] Add deeper tests on highest-churn areas
- [ ] Expand runbooks for recurring operator or publishing workflows
- [ ] Standardize naming and checklist structure for future items

## V10 (scale)
- [ ] Move to a stable platform pattern with cross-repo checklist templates
- [ ] Split roadmap into discrete feature-level and initiative-level folders
- [ ] Define long-range acceptance criteria with operational and product owners

## Top touched files (90-day top 10)
- .gitattributes
- 00_agile/backlog/.gitkeep
- 00_agile/doing/.gitkeep
- 00_agile/done/.gitkeep
- 00_agile/epics/.gitkeep
- ... and 5 more

## Follow-up ideas
- [ ] Convert area signals into one short feature roadmap within docs/features
- [ ] Add changelog notes in docs for behavior-impacting updates
- [ ] Add simple owner checklist for release readiness
