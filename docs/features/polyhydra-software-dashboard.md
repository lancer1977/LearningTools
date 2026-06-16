---
title: Polyhydra Software Dashboard
status: draft
owner: @DreadBreadcrumb
priority: medium
complexity: 2
created: 2026-06-16
updated: 2026-06-16
tags: [documentation, LearningTools, dashboard, keycloak]
github_issue: 10
---

# Polyhydra Software Dashboard

## Decision Record

The preferred direction is a custom Polyhydra dashboard that uses Keycloak for identity and authorization, while treating the Keycloak account console as a fallback and administrative surface.

Reasons:

- Keycloak already solves login, token issuance, and role claims.
- A branded dashboard can present the app portfolio in the language of the platform, not the identity provider.
- The dashboard can expose only the apps a user can actually access.
- Application links in Keycloak remain useful for bootstrap and admin workflows.

## Concept

The dashboard is the primary launchpad for Polyhydra apps and shared operator tools.

Candidate cards:

- Channel Cheevos
- Trivia
- Stream Tools
- Admin
- Docs
- Overlays
- Build and package status

## Route Sketch

- `/` opens the dashboard home.
- `/apps` shows the full app registry.
- `/apps/{slug}` opens a focused app detail card.
- `/status/build` shows build and package status.
- `/admin` links to operator and role-gated controls.

## App Metadata Model

The dashboard only needs a small registry shape to drive the UI:

| Field | Purpose |
| --- | --- |
| `name` | Display name shown on the card |
| `slug` | Stable route key |
| `description` | Short user-facing summary |
| `icon` | Glyph or image to identify the app |
| `url` | Destination link |
| `requiredRoles` | Roles needed to show or launch the app |
| `requiredClaims` | Optional claim filters for finer control |
| `category` | Grouping such as streaming, admin, docs, or build |
| `order` | Card ordering within the grid |
| `isExternal` | Whether the destination leaves the dashboard |

Example shape:

```text
{
  name: "Trivia",
  slug: "trivia",
  description: "Play or manage trivia experiences",
  icon: "question-circle",
  url: "https://trivia.example",
  requiredRoles: ["polyhydra.trivia.user"],
  requiredClaims: ["polyhydra:apps:trivia"],
  category: "streaming",
  order: 20,
  isExternal: true
}
```

## Authorization Model

The dashboard should read Keycloak roles and claims from the token, then filter the registry before rendering cards.

Likely sources:

- realm roles for broad access
- client roles for app-specific access
- custom claims for special operator or environment access

## Follow-Up Work

If this concept is approved, the next implementation issues should cover:

- token parsing and role mapping
- the app registry service
- dashboard UI composition
- access-controlled deep links
- Keycloak application-link fallback guidance

GitHub issue: #10
