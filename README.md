# Education Center Management System

An enterprise-grade, highly scalable backend and MVVM desktop client designed to manage educational centers. This project demonstrates advanced architectural patterns and production-ready .NET engineering.

## 🏗 Architecture Overview

This project strictly adheres to **Clean Architecture** principles and **Domain-Driven Design (DDD)**. 

### Key Design Patterns & Practices:
- **Clean Architecture**: Strict separation of concerns ensuring the Domain has zero external dependencies. The Presentation layer is physically decoupled and interacts solely via HTTP API.
- **CQRS**: Commands and Queries are segregated using `MediatR` to ensure distinct models for reads and writes.
- **Vertical Slice Architecture**: Application layer logic is grouped by use-case (e.g., `Students/Register`, `Students/UpdateProfile`), improving cohesion and maintainability.
- **Result Pattern**: Control flow is managed safely via `ErrorOr<T>` to completely eliminate exceptions for expected business rule violations.
- **Domain-Driven Design (DDD)**: Rich domain models with encapsulated states (private setters), factory methods, guard clauses, and Domain Events.
- **Unit of Work & Repository Pattern**: Abstracted database interactions for atomic transactions.

## 💻 Tech Stack
- **Backend Framework**: .NET 9 (ASP.NET Core Web API)
- **Database**: PostgreSQL with Entity Framework Core
- **Desktop Client**: Windows Forms powered by `CommunityToolkit.Mvvm` and Dependency Injection (Modern MVVM approach).
- **Authentication/Security**: JWT Bearer Tokens, Rate Limiting, CORS.
- **Observability**: Serilog (Structured Logging, Request Logging).
- **Testing**: xUnit, FluentAssertions, Moq, NetArchTest.eRules (Architecture Tests).

## 📁 Solution Structure
- `src/EducationCenterSystem.Domain`: Enterprise business rules, Entities (e.g., Student, Teacher, Course), and Value Objects (Email, PhoneNumber).
- `src/EducationCenterSystem.Application`: MediatR Handlers, Validation, and Domain Event Handlers.
- `src/EducationCenterSystem.Infrastructure`: EF Core DbContext, Repositories, and Password Hashing.
- `src/EducationCenterSystem.Api`: Minimal/Controller Endpoints, Swagger, Middleware pipeline.
- `src/EducationCenterSystem.Presentation.WinForms`: The MVVM UI Client.
- `tests/`: Comprehensive test suite verifying Domain invariants, Application logic, and architectural boundaries.
