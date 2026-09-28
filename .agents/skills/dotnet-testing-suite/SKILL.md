---
name: dotnet-testing-suite
description: Enterprise .NET testing standard for EducationCenterSystem. Covers xUnit, FluentAssertions, Moq, CQRS Handlers, Domain Invariant tests, and Integration tests.
---

# Enterprise .NET Testing Standard

Apply this protocol whenever authoring, maintaining, or fixing tests in `tests/`.

---

## 1. Core Testing Rules
1. **Framework & Assertions:**
   - Use **xUnit** as test runner.
   - Use **FluentAssertions** for all assertions (`result.IsSuccess.Should().BeTrue();`).
   - Use **Moq** or **NSubstitute** for mocking external dependencies only (Repositories, Email, Clocks). Never mock Domain entities or Value Objects.
2. **Strict Test Naming Convention:**
   - Format: `[UnitOfWork]_[ExpectedBehavior]_[WhenScenario]`
   - Example: `RegisterStudentCommandHandler_ShouldReturnFailure_WhenNationalIdAlreadyExists`
3. **Triple-A Structure (AAA):**
   - Every test must clearly delimit `// Arrange`, `// Act`, and `// Assert`.
4. **Zero Fakes or Vacuous Assertions:**
   - Banned: `Assert.True(true)`, testing auto-properties getters/setters, or testing empty mocks.
   - Always verify state changes, returned errors (`result.FirstError.Code.Should().Be(...)`), or mock invocations (`mockRepo.Verify(...)`).

---

## 2. Test Categories & Boundaries

### A. Domain Unit Tests (`tests/EducationCenterSystem.Domain.UnitTests`)
- **Scope:** Pure business logic, entity state transitions, guard clauses, invariants.
- **Dependencies:** ZERO mocks allowed. Test pure C# objects.
- **Scenarios:** Happy paths, boundary limits (e.g. negative balances, full groups, invalid phone numbers).

### B. Application CQRS Tests (`tests/EducationCenterSystem.Application.UnitTests`)
- **Scope:** MediatR Command and Query handlers, pipeline behaviors, validation rules (FluentValidation).
- **Dependencies:** Mock Repositories (`IStudentRepository`), `IUnitOfWork`, and external gateways.
- **Scenarios:** Validation failures, repository not found errors, optimistic concurrency exceptions, successful orchestration.

### C. Architecture Tests (`tests/EducationCenterSystem.Architecture.Tests`)
- **Scope:** Enforcing Clean Architecture dependency rules with `NetArchTest.Rules`.
- **Enforcement:** Ensure Domain does not reference Infrastructure or Presentation, Handlers follow CQRS naming conventions.
