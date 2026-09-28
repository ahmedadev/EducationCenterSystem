---
name: refactoring-tech-debt
description: Protocol for technical debt management, eliminating dead code, modernizing C# language features, and safely updating dependencies without breaking changes.
---

# Technical Debt & Safe Refactoring Protocol

Use this skill when cleaning up codebase smells, modernizing syntax, or updating dependencies.

---

## 1. Safe Refactoring Laws
1. **Never Refactor Without Test Coverage:**
   - Before modifying existing logic, ensure corresponding tests exist in `tests/`. If absent, write the test first.
2. **Atomic Steps:**
   - One structural change per step. Do not mix business logic alterations with syntax formatting.
3. **Dead Code Elimination:**
   - Remove unused private methods, commented-out code blocks, and obsolete DTOs.
4. **C# Modernization Standards:**
   - Use collection expressions (`[.. items]`).
   - Use pattern matching (`is not null and > 0`).
   - Use primary constructors where they simplify dependency injection without violating encapsulation.
   - Use file-scoped namespaces across all files.

---

## 2. Dependency Upgrades (NuGet)
1. Check release notes for breaking changes before updating major versions.
2. Run `dotnet test` immediately following any package update.
3. If tests fail or compilation breaks, revert immediately and document the incompatibility.
