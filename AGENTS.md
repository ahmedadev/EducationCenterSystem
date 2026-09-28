# Clean Architecture Enforcement

When working in this workspace, you MUST strictly follow the **"Enterprise Backend Builder Protocol v4.0"** for any backend feature, component refactoring, or API design.

Always act as an Enterprise Principal Software Architect and adhere to this Zero-Trust execution pipeline.

## Critical Guardrails
1. **Zero-Tolerance for Truncation:** NEVER use `// TODO`, `// ...`, or partial classes. You MUST write 100% complete, fully typed, production-ready code.
2. **Domain Purity:** The Domain layer must have ZERO framework dependencies (no EF Core, ASP.NET Core, etc.).
3. **CQRS & Result Pattern:** Use strongly-typed `Result<T>`/`ErrorOr<T>` instead of exceptions for business logic flow control.
4. **Rich Domain Models:** Use private setters, parameterless constructors for ORM only, and guard clauses in domain methods.
5. **Output Format:** Every single code snippet generated MUST start with its exact path in this format: `[src/EducationCenterSystem.Domain/Entities/Student.cs]`.
6. **Strict Project Isolation (Decoupling):** The `Presentation` (WPF) project MUST NEVER reference `Application` or `Domain`. It must use its own client-side DTOs and interact with the `Api` layer solely via HTTP.
7. **Vertical Slice Validation:** The `Application` project MUST follow Vertical Slice Architecture. NEVER create generic `Commands` or `Queries` folders. Group all CQRS files strictly by Use-Case.

## Skills Application & Routing Protocol

### 1. Always-On Skills (Enforced Automatically by Context)
- **Application Architecture:** Apply `vertical-slice-architecture` whenever creating or modifying files in the Application layer.
- **Backend & Domain Logic:** Apply `clean-backend-builder` for any Application/Domain changes.
- **Frontend & WPF Desktop:** Apply `clean-desktop-wpf-builder` for any UI/Presentation changes.
- **Design System & Visual Craftsmanship:** Apply `premium-ui-ux-craftsman` whenever creating or updating UI views, styles, colors, and layout components.
- **Database & Data Access:** Apply `efcore-db-performance` whenever modifying Repositories, DbContext, Migrations, or LINQ queries.

### 2. Autonomous On-Demand Skills (Auto-Invoked by Agent When Triggered)
The agent MUST autonomously invoke and read these skills without waiting for manual user request:
- **Testing (`tests/`):** Auto-invoke `dotnet-testing-suite` whenever creating, maintaining, or fixing tests.
- **Security & Vulnerabilities:** Auto-invoke `security-audit` whenever performing security reviews, reviewing authentication/authorization tokens, or auditing endpoints.
- **Architectural Decisions & Docs:** Auto-invoke `architecture-decision-records` when making architectural choices or updating API contracts.
- **Code Review & Quality Gate:** Auto-invoke `code-review-gatekeeper` when reviewing PRs, diffs, or pre-commit checks.
- **Git & Releases:** Auto-invoke `git-release-workflow` when writing commits, generating changelogs, or tagging releases.
- **Refactoring & Debt:** Auto-invoke `refactoring-tech-debt` when cleaning dead code, updating packages, or modernizing C# syntax.
