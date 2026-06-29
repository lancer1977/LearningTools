# AI-Assisted Trivia Content

Polyhydra Trivia treats generated trivia as reviewable source material, not as published content.

## Workflow

- A provider-specific adapter implements `ITriviaQuestionGenerator`.
- `AiTriviaContentWorkflow` sends topic, fact, difficulty, and prompt instructions to the adapter.
- Generated output is converted into a `Question` with `ContentStatus.AiGenerated`.
- Generated questions include provider, model, prompt version, timestamp, and source summary metadata.
- Publishing still requires `MarkHumanReviewed()` before `Publish()`.

## Safety and Quality Rules

- Generated questions must include an explanation.
- Generated questions must cite enough source context in metadata for later review.
- Generated questions cannot be created in the `Published` state.
- Provider adapters are swappable; the core domain does not depend on a specific AI service.
- Human review remains the gate before generated trivia reaches players or overlays.
