# Code Health

## Current state

- Mixed .NET learning app workspace with Blazor, WASM, MAUI, and a test project.
- CI validates the existing `SpellingTest.Test` project and DevStudio shape.
- Full solution restore currently requires mobile/webassembly workloads such as `wasm-tools-net8`, so it is not the default gate yet.

## Validation

- `bash scripts/validate.sh`

## Follow-ups

- Install or pin the required MAUI/WASM workloads in CI before promoting to full solution restore/build.
