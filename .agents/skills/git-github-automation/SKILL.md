---
name: git-github-automation
description: Automated Git & GitHub operations, branch management, safe atomic commits, PR workflows, conflict resolution, and GitHub CLI integration for EducationCenterSystem.
---

# Git & GitHub Automation Protocol

Autonomous operational guidelines for version control, branch management, GitHub workflows, and repository hygiene.

---

## 1. Trigger Conditions
Autonomously invoke this skill when:
- Initializing, configuring, or inspecting git repositories.
- Staging, committing, branching, stashing, rebasing, or merging code.
- Interacting with GitHub remotes (`origin`), GitHub CLI (`gh`), creating PRs, or releasing tags.
- Resolving merge conflicts or untracked file sync issues.

---

## 2. Safety Guardrails
1. **Never Force Push to Main/Master:** Running `git push --force` or `git push -f` against primary protected branches (`main`, `master`) is strictly forbidden.
2. **Pre-Commit Verification:**
   - Always run `git status` and verify staged changes before committing.
   - Verify `.gitignore` rules: never stage secrets, `.env` files, build artifacts (`bin/`, `obj/`), user preferences, or runtime database files (`*.db`, `*.sqlite`).
3. **Atomic Commits:**
   - Group related file modifications into small, self-contained commits.
   - Do not bundle unrelated refactoring with new feature implementations.

---

## 3. Commit Standards (Conventional Commits)
Format: `<type>(<scope>): <concise action-oriented description>`

### Allowed Types
- `feat`: New capability or public contract addition.
- `fix`: Bug fix or defect correction.
- `refactor`: Structural code cleanup without changing behavior.
- `perf`: Performance enhancement or query optimization.
- `test`: Test suite additions, updates, or fixtures.
- `docs`: Documentation, markdown, or architectural decision updates.
- `chore`: Tooling, build scripts, configuration, or dependency updates.
- `ci`: CI/CD pipelines and GitHub Actions workflows.

### Allowed Scopes
`domain`, `application`, `infrastructure`, `api`, `wpf`, `tests`, `db`, `git`, `scripts`

### Examples
- `feat(api): add student enrollment endpoint with fluent validation`
- `fix(wpf): detach window event listener to stop memory leak`
- `chore(git): configure gitignore and github actions workflow`

---

## 4. Branching & Remote Workflow
- **Branch Naming:**
  - Feature: `feature/<feature-name>` (e.g., `feature/attendance-tracking`)
  - Fix: `fix/<issue-description>` (e.g., `fix/token-refresh-expiry`)
  - Refactoring: `refactor/<target>` (e.g., `refactor/student-repository-queries`)
- **Syncing Protocol:**
  1. Pull remote updates cleanly: `git pull --rebase origin <branch>`
  2. If local dirty files exist: `git stash push -m "WIP before pull"` -> pull -> `git stash pop`
  3. Push to upstream: `git push -u origin <branch>`

---

## 5. GitHub CLI (`gh`) Automation
When GitHub CLI (`gh`) is available in environment:
- **Inspect Status:** `gh auth status` and `gh repo view`
- **Create Pull Request:**
  ```bash
  gh pr create --title "type(scope): summary" --body "## Description\n...\n## Verification\n..."
  ```
- **Review PR Status:** `gh pr status` and `gh pr checks`
- **Create Release Tag:**
  ```bash
  gh release create vX.Y.Z --title "Release vX.Y.Z" --notes-file CHANGELOG.md
  ```

---

## 6. Conflict Resolution Protocol
1. Identify conflicting files via `git status`.
2. Inspect exact markers (`<<<<<<< HEAD`, `=======`, `>>>>>>>`).
3. Preserve clean architecture boundaries and business logic invariants.
4. Stage resolved files: `git add <path>`.
5. Finalize merge/rebase: `git commit` or `git rebase --continue`.
