#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

required_paths=(
  "README.md"
  ".env.example"
  "code_health.md"
  "docs/project-atlas/README.md"
  "templates/README.md"
  ".devstudio/project.yaml"
  "SpellingTest.sln"
  "SpellingTest.Test/SpellingTest.Test.csproj"
)

for path in "${required_paths[@]}"; do
  if [[ ! -e "$path" ]]; then
    echo "Missing required path: $path" >&2
    exit 1
  fi
done

if command -v dotnet >/dev/null 2>&1; then
  dotnet restore SpellingTest.Test/SpellingTest.Test.csproj
  dotnet test SpellingTest.Test/SpellingTest.Test.csproj --no-restore
else
  echo "dotnet not available; skipping tests"
fi

if command -v devstudio >/dev/null 2>&1; then
  devstudio validate --repo "$repo_root"
else
  echo "devstudio not available; skipping DevStudio validation"
fi
