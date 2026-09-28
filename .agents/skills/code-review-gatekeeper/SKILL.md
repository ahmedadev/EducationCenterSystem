---
name: code-review-gatekeeper
description: Automated quality gate for PRs and pre-commit checks. Evaluates Clean Architecture compliance, cyclomatic complexity, code smells, and ensures zero truncation in EducationCenterSystem.
---

# Code Review Gatekeeper Protocol

Use this skill when auditing changes, reviewing git diffs, or evaluating code quality before merging/committing.

---

## 1. Review Checklist (Fail-Fast Rules)
1. **Zero Truncation Check:**
   - Immediately reject any code containing `// TODO`, `// ...`, or partial implementations.
2. **Domain Purity Verification:**
   - Verify `EducationCenterSystem.Domain` has NO references to EF Core, ASP.NET Core, or Infrastructure.
3. **CQRS & Result Pattern Compliance:**
   - Reject any business logic throwing raw exceptions for control flow; require `Result<T>` / `ErrorOr<T>`.
4. **WPF Leaks & Thread Safety:**
   - In `EducationCenterSystem.Presentation`, ensure events are unhooked or using `WeakEventManager`, and UI updates execute on the Dispatcher thread.
5. **Database Query Inspection:**
   - Reject queries iterating over entities without `.AsNoTracking()` or risking `N+1` roundtrips.

---

## 2. Review Output Format
Structure every code review output into:
- **Verdict:** [APPROVE | REQUEST_CHANGES | BLOCK]
- **Critical Violations (Blockers):** Exact file and line numbers violating zero-trust rules.
- **Improvements (Non-Blockers):** Performance or readability enhancements.
- **Diff Fix:** Concrete, production-ready replacement code for each violation.
