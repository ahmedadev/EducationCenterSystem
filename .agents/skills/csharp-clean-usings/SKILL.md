---
name: csharp-clean-usings
description: Automatically identifies, prevents, and eliminates unnecessary 'using' directives (IDE0005) across C# files and projects in EducationCenterSystem.
---

# C# Clean Usings & Directive Pruning Protocol

Use this skill whenever authoring, refactoring, or reviewing any C# file in the solution to eliminate unused `using` directives (`IDE0005: Using directive is unnecessary`).

---

## 1. Zero-Tolerance for Unused Directives
1. **Never Leave Dead Imports:**
   - Any `using` directive that is not actively referenced in the file must be removed immediately.
   - Proactively inspect and strip unneeded namespace references whenever touching or creating C# files.
2. **Directive Placement & Order:**
   - Place `using` directives outside the namespace block, sorted with `System` namespaces at the top.
3. **Implicit Global Usings Awareness:**
   - Do not re-declare namespaces already provided globally by the SDK/project configuration.

---

## 2. Automated Solution & Project Cleansing

### Solution-Wide Formatter
To strip all `IDE0005` unused using directives across the entire solution:
```powershell
dotnet format EducationCenterSystem.sln style --diagnostics IDE0005
```

### Single Project Formatter
To clean a specific project:
```powershell
dotnet format src/EducationCenterSystem.Presentation.WinForms/EducationCenterSystem.Presentation.WinForms.csproj style --diagnostics IDE0005
```

### Manual Verification
Ensure zero build errors after pruning:
```powershell
dotnet build --no-incremental
```
