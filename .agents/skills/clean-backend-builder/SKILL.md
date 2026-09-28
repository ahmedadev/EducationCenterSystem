---
name: clean-backend-builder
description: >-
  The definitive Zero-Trust Enterprise .NET Clean Architecture Skill (v4.0).
  Strictly enforces the 5 Core Rules of Clean Architecture, Domain-Driven Design (DDD),
  CQRS, Result Pattern, Unit of Work, and prevents AI truncation or code-smell anti-patterns.
---

# Enterprise Backend Builder Protocol v4.0 (The Clean Architecture Rulebook)

When executing any backend feature, component refactoring, or API design, you act as an Enterprise Principal Software Architect. You MUST adhere to this Zero-Trust execution pipeline without skipping files, using shortcuts, or truncating code blocks.

---

## 🏛️ THE 5 CORE LAWS OF CLEAN ARCHITECTURE (NON-NEGOTIABLE)
1. **Independent of Frameworks:** The Architecture does not depend on the existence of some library of feature-laden software. Frameworks are tools, not constraints.
2. **Testable without UI/Database/Web:** The business rules can be tested without the UI, Database, Web Server, or any external element.
3. **Independent of UI:** The UI can change easily without changing the rest of the system (e.g., Web API to CLI/WPF).
4. **Independent of Database:** Business rules are not bound to Oracle, SQL Server, Mongo, etc. Schema changes MUST NOT break core logic.
5. **Independent of any External Agency:** Domain business rules simply do not know anything about the outside world.

---

## 🚫 ZERO-TOLERANCE GUARDRAILS (ANTI-PATTERNS & LAZINESS BANNED)
1. **NO LAZY CODING OR TRUNCATION:** FORBIDDEN from using `// TODO`, `// ...`, `/* logic here */`, or partial classes. You MUST write 100% complete, fully typed, production-ready code.
2. **File-Scoped Namespaces:** Strictly use modern C# file-scoped namespaces (`namespace EducationCenterSystem.Domain.Entities;`). No nested braces for namespaces.
3. **Domain Purity (Zero Dependency Leakage):** The `Domain` project MUST have ZERO NuGet packages or Framework references (no Entity Framework, Newtonsoft, or ASP.NET Core).
4. **Repository & UoW Purity:** Repositories MUST ONLY mutate the in-memory state (`DbSet.Add`, `Update`, `Remove`). `SaveChanges()` inside Repositories is strictly BANNED. Persistence commits occur ONLY via `IUnitOfWork` inside Application Handlers or Pipeline Behaviors.
5. **Explicit Result Pattern:** Throwing control-flow exceptions for domain or business validation is FORBIDDEN. Use strongly-typed `Result<T>` or `ErrorOr<T>` values.
6. **Rich Domain Models over Anemic Entities:** Aggregate Roots and Entities MUST have `private set` properties and encapsulate state transitions inside explicit Domain Methods with Guard Clauses.
7. **Cancellation Token Propagation:** EVERY asynchronous method across ALL layers MUST accept and propagate a `CancellationToken`.
8. **NEVER DROP THE DATABASE:** When making schema changes or EF Core migrations, you are FORBIDDEN from dropping the database or deleting existing tables. If existing data violates new constraints (e.g., adding a new `Unique Index` on an existing column), you MUST write a data migration or set appropriate default values in the `Up()` method to preserve all existing data before applying the constraint.

---

## 🏗️ THE 5-STAGE STRICT EXECUTION PIPELINE

### STAGE 1: DOMAIN & CONTRACTS LAYER (`src/*.Domain`)
1. **Aggregate Roots, Entities & Value Objects:**
   - Define immutable Value Objects for domain types (`Money`, `Email`, `PhoneNumber`).
   - Private parameterless constructors for ORM/EF Core rehydration.
   - Guard clauses inside entity methods to enforce invariants before updating state.
2. **Domain Events & Dispatcher Abstractions:**
   - Raise events before returning from domain methods (e.g., `AddDomainEvent(new StudentRegisteredEvent(Id))`).
3. **Core Abstractions:**
   - Define `I[Entity]Repository`, `IUnitOfWork`, and System/External Interfaces.

### STAGE 2: APPLICATION LAYER (`src/*.Application`)
1. **CQRS Contracts (Commands & Queries):**
   - Sealed `Record` DTOs for Request/Response messages.
   - Strict separation between Commands (mutations returning `Result`) and Queries (read-only projections returning `Result<T>`).
2. **MediatR Pipeline Behaviors & Handlers:**
   - Implement `LoggingBehavior`, `ValidationBehavior`, and `TransactionBehavior`.
   - Handlers follow the strict sequence: `Load Aggregate -> Execute Domain Method -> Persist via IUnitOfWork -> Return Result`.
3. **FluentValidation:**
   - Dedicated `AbstractValidator<T>` for every Command/Query. Must execute automatically via MediatR Pipeline Behavior.

### STAGE 3: INFRASTRUCTURE LAYER (`src/*.Infrastructure`)
1. **EF Core Configurations & Persistence:**
   - Implement `IEntityTypeConfiguration<T>` for clean schema mapping. Zero data annotations in Domain.
   - Map Value Objects using `OwnsOne` or `HasConversion`. Configure Indexes and Soft-Delete Filters.
2. **Concrete Repositories & Unit of Work:**
   - Execute query projections and data persistence adhering strictly to Domain interfaces.
3. **Outbox Pattern & Domain Event Interceptors:**
   - Implement `SaveChangeInterceptor` to serialize Domain Events into an Outbox table before committing the transaction.

### STAGE 4: API LAYER (`src/*.Api`)
1. **Thin Controllers / Minimal API Endpoints:**
   - Controllers/Endpoints contain ZERO business logic, no `if/else` checks, and no direct database/EF calls.
   - Inject ONLY `ISender` (MediatR).
   - Map `Result<T>` to standardized HTTP responses (`200 OK`, `400 Bad Request`, `404 Not Found`, `409 Conflict`).
2. **Global Exception Handling & Composition Root:**
   - Use .NET 8 `IExceptionHandler` for unhandled system crashes.
   - Provide clean extension methods: `AddDomain()`, `AddApplication()`, `AddInfrastructure()`.

### STAGE 5: PRESENTATION LAYER DECOUPLING (`src/*.Presentation`)
1. **Strict Client Isolation:**
   - The UI (WPF) project MUST NEVER have a ProjectReference to `Domain` or `Application`.
   - It communicates with the backend EXCLUSIVELY via HTTP (`HttpClient`).
2. **Client-Side Models & DTOs:**
   - Duplicate necessary Models, Enums, and Constants in `Presentation/Models`.
   - Create specific DTOs (`Presentation/Models/DTOs`) for requests. NEVER use backend MediatR Commands/Queries inside the UI ViewModels.

### STAGE 6: INTEGRITY & ARCHITECTURE VERIFICATION
- Ensure 100% C# `<Nullable>enable</Nullable>` safety.
- Verify zero static leaks or cyclic dependencies between layers.
- Confirm all async calls pass `CancellationToken`.

---

## 📄 OUTPUT FORMAT REQUIREMENT
Deliver generated code strictly file-by-file in execution order. Every single code snippet MUST start with its exact path:
`[src/EducationCenterSystem.Domain/Entities/Student.cs]`
