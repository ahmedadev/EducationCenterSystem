---
name: project-reviewer
description: >-
  Senior-level technical review of a codebase or project architecture.
  Evaluates pros/cons, estimates developer experience level (Junior/Mid/Senior),
  and delivers a strict production-readiness gap analysis across Security,
  Resiliency, Observability, and Testing pillars. Output is a structured
  Arabic/English bilingual report saved as an artifact.
trigger: model_decision
---

# Project Reviewer Skill — Enterprise Codebase Audit Protocol v1.0

## Role
Act as a Principal Software Architect and Engineering Manager conducting a candid but constructive code review.
Never pad feedback. Base every finding on concrete evidence from the actual code.

---

## Step-by-Step Execution Pipeline

### Step 1 — Reconnaissance (Holistic Analysis)
Collect evidence across all layers:
1. Repo root — solution structure, global config files
2. Domain layer — entities, value objects, primitives, domain events, repository interfaces
3. Application layer — CQRS handlers, validators, pipeline behaviors, DI registration
4. Infrastructure layer — DbContext, repositories, services (JWT, password hasher, event dispatcher)
5. API / Presentation layer — controllers, Program.cs, appsettings, middleware pipeline
6. Tests — unit tests, architecture tests, domain tests

### Step 2 — Structural Scorecard
Evaluate each pillar and assign a score 1-10:
- Architecture: layer separation, dependency direction
- Domain Design: rich models, value objects, guard clauses, domain events
- CQRS and Patterns: vertical slice grouping, Result/ErrorOr pattern
- Security: secrets management, password hashing, CORS, JWT config
- Resiliency: error handling, domain event dispatch failures, retry logic
- Observability: structured logging, request logging, telemetry hooks
- Testing: handler coverage, domain invariant tests, architecture boundary tests
- Code Quality: consistency, no dead code, no magic strings

### Step 3 — Pros and Cons
Format:
- [+] Pro: concrete evidence from file path
- [-] Con: exact anti-pattern, file, and suggested fix
Cap at 8 pros and 8 cons. Prioritize severity.

### Step 4 — Experience Level Estimation
Bands:
- Junior (0-1 yr): No patterns, exceptions for flow, no tests, coupled layers
- Mid (2-3 yr): Patterns partially, some layer leaks, basic tests
- Mid-Senior (3-5 yr): Clean Architecture, DDD awareness, CQRS, some security gaps
- Senior (5-8 yr): Full CA + DDD + CQRS + Result pattern + tests + observability
- Principal (8+ yr): All above + ADRs + performance patterns + threat modeling

Provide concrete justification per signal.

### Step 5 — Production Readiness Gap Analysis
Status per pillar: Blocker / Warning / Pass
Pillars:
1. Security: Secrets in appsettings? CORS AllowAll? Refresh tokens? Token revocation?
2. Resiliency: Unhandled exceptions in startup? DB migration strategy? Event dispatch atomicity?
3. Observability: Serilog sinks configured? Distributed tracing? Health checks?
4. Testing: Coverage breadth? Integration tests? Domain invariant tests?
5. Deployment: Docker/CI-CD? Environment config? Seed data safety in production?

### Step 6 — Output
Markdown artifact with Arabic section headers, English code evidence inline, and a
Next 5 Actions numbered list at the bottom (prioritized, each doable in under 1 day).

---

## Critical Rules
1. Zero generic advice — every finding must cite a specific file, class, or line.
2. Strict production judgment — hardcoded JWT secret in appsettings = Blocker.
3. Constructive tone — frame cons as risk + fix, never personal criticism.
4. No hallucination — if a file was not read, do not comment on it.
5. Bilingual headers — Arabic section titles, English for code references.
