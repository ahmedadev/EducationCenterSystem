---
name: vertical-slice-architecture
description: Enforces the Vertical Slice Architecture style in the Application layer, ensuring all CQRS elements are grouped by feature and use-case without generic "Commands" or "Queries" folders.
---

# Vertical Slice Architecture Enforcement

When working on or generating code for the `Application` layer of this project, you MUST strictly adhere to the **Vertical Slice Architecture** style for CQRS implementation.

## Core Rules

1. **NO `Commands` or `Queries` Subdirectories:**
   You must NEVER create folders named `Commands` or `Queries` under a feature module.

2. **Feature -> Use-Case Grouping:**
   Group all files related to a specific use-case (Command, Handler, Validator, Response DTO) directly inside a specific use-case folder under the main feature folder.
   - ✅ CORRECT: `Application/Students/Register/RegisterStudentCommand.cs`
   - ✅ CORRECT: `Application/Students/Register/RegisterStudentCommandHandler.cs`
   - ❌ INCORRECT (Do Not Do This): `Application/Students/Commands/Register/RegisterStudentCommand.cs`

3. **Namespaces Must Match Folders:**
   Ensure the namespaces correctly reflect this flat structure.
   - ✅ CORRECT: `namespace EducationCenterSystem.Application.Students.Register;`
   - ❌ INCORRECT: `namespace EducationCenterSystem.Application.Students.Commands.Register;`

4. **Naming Conventions (Remove Feature Redundancy):**
   The folder structure already provides the context (e.g., `Students/Register`). Therefore, the file and class names MUST NOT redundantly include the entity or feature name. They should exactly match the action.
   - ✅ CORRECT: `RegisterCommand.cs`, `RegisterCommandHandler.cs` (inside `Students/Register`)
   - ❌ INCORRECT: `RegisterStudentCommand.cs`
   - ✅ CORRECT: `GetAllQuery.cs` (inside `Courses/GetAll`)
   - ❌ INCORRECT: `GetAllCoursesQuery.cs`
   - ✅ CORRECT: `Response.cs` or `StudentResponse.cs` (Responses can keep the name if shared, but shorter is better if scoped to a use-case).

## Fallback / Conflict Resolution
If you find yourself in a situation where applying this pattern is technically impossible, or the user explicitly asks you to revert to a different folder structure (like traditional CQRS with `Commands` and `Queries` folders), you MUST STOP and notify the user with exactly this phrase: 
"تعذر تطبيق نمط Vertical Slice هنا، يرجى التوضيح" 
Do not proceed until the user clarifies.
