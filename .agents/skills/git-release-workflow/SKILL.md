---
name: git-release-workflow
description: Standardized Git workflow, Conventional Commits formatting, automated Changelog generation, and Semantic Versioning (SemVer) for EducationCenterSystem.
---

# Git Workflow & Release Automation Protocol

Use this skill when preparing commits, tagging releases, or generating changelogs.

---

## 1. Conventional Commits Standard
Every commit message MUST adhere to:
`type(scope): subject`

### Allowed Types:
- `feat`: A new feature for the user or API.
- `fix`: A bug fix.
- `refactor`: Code change that neither fixes a bug nor adds a feature.
- `perf`: Code change that improves performance.
- `test`: Adding missing tests or correcting existing tests.
- `docs`: Documentation only changes.
- `chore`: Maintenance, build tasks, or dependency updates.

### Allowed Scopes:
`domain`, `application`, `infrastructure`, `api`, `wpf`, `tests`, `db`

### Examples:
- `feat(domain): add AttendanceStatus value object with guard clauses`
- `fix(wpf): unregister window event listener to prevent memory leak`
- `perf(infrastructure): add split queries to group details handler`

---

## 2. Release & Changelog Protocol
- **SemVer:** `MAJOR.MINOR.PATCH` (e.g. `1.2.0`).
  - `MAJOR`: Breaking architecture or schema changes.
  - `MINOR`: New backward-compatible features.
  - `PATCH`: Backward-compatible bug fixes.
- **Changelog Entry:** Summarize changes under `### Added`, `### Changed`, and `### Fixed` in `CHANGELOG.md`.
