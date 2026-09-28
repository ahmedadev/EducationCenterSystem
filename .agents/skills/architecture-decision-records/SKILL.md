---
name: architecture-decision-records
description: Protocol for documenting architectural decisions (ADRs) using MADR standard and maintaining up-to-date OpenAPI/Swagger documentation in EducationCenterSystem.
---

# Architecture Decision Records (ADRs) & API Documentation Protocol

Use this skill when introducing major architectural changes, adopting new libraries, or updating API contracts.

---

## 1. ADR Standard (MADR Format)
When making significant design decisions (e.g. database schema changes, authentication mechanism, messaging protocols), record them in `docs/adr/XXXX-title.md`:

```markdown
# [Short Title of Solved Problem]

* Status: [draft | proposed | accepted | rejected | superseded]
* Deciders: [List of decision makers]
* Date: [YYYY-MM-DD]

## Context and Problem Statement
[2-3 sentences explaining the technical context and why a decision was needed.]

## Considered Options
* [Option 1]
* [Option 2]

## Decision Outcome
Chosen option: "[Option 1]", because [justification in terms of architecture purity, performance, and maintenance].

### Consequences
* Good: [Positive impact]
* Bad: [Trade-offs or operational costs]
```

---

## 2. API Contract & OpenAPI Sync
1. Every Endpoint in `EducationCenterSystem.Api/Controllers` MUST have complete OpenAPI attributes (`[ProducesResponseType]`, `[EndpointSummary]`).
2. Error responses MUST document the `ProblemDetails` schema for 400, 404, 409, and 500 status codes.
3. Breaking changes to request/response contracts require creating an ADR before implementation.
