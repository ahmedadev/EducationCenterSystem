---
name: efcore-db-performance
description: Enterprise EF Core optimization protocol, migration safety, query indexing, N+1 prevention, split queries, and transaction management for EducationCenterSystem.
---

# Enterprise EF Core & Database Performance Protocol

Strictly follow these guidelines when writing queries, entities configuration, or database migrations in `EducationCenterSystem.Infrastructure`.

---

## 1. Query Performance Laws
1. **Always Use `AsNoTracking()` for Read Queries:**
   - Any query returning data to display (WPF/API) MUST use `.AsNoTracking()`.
2. **Prevent N+1 Query Traps:**
   - Never iterate over an entity collection and query navigation properties in a loop.
   - Use explicit `.Include()` or projection with `.Select()` to fetch only required fields directly in SQL.
3. **Cartesian Explosion Defense:**
   - When loading multiple collection navigations with `.Include()`, always append `.AsSplitQuery()` to avoid massive duplicate row joins.
4. **Enforce Keyset / Pagination:**
   - Never execute `.ToList()` or `.ToListAsync()` on unbounded tables (Students, Attendances, Transactions). Always enforce `.Take(pageSize).Skip(...)` or keyset pagination.

---

## 2. Entity Configuration & Indexing
1. **Explicit Fluent API Configurations:**
   - Every entity in `EducationCenterSystem.Infrastructure/Persistence/Configurations` MUST implement `IEntityTypeConfiguration<T>`.
   - Never use Data Annotations inside the Domain layer.
2. **Strategic Indexing for Core Workflows:**
   - Foreign Keys: Always ensure indexed foreign keys (`StudentId`, `GroupId`, `CourseId`).
   - High-Frequency Lookups: Add indexes to `Barcode`, `PhoneNumber`, and `NationalId` with unique constraints where applicable.
   - Compound Indexes: Index composite filters like `(GroupId, AttendanceDate)`.
3. **Decimal Precision & Money:**
   - All monetary properties (Prices, Balances, Payments) MUST explicitly specify precision: `builder.Property(x => x.Amount).HasPrecision(18, 2);`.

---

## 3. Migration & Concurrency Safety
1. **Zero-Downtime Migrations:**
   - Never drop columns or tables without deprecation phases.
   - Provide safe defaults for newly added non-nullable columns.
2. **Concurrency Tokens:**
   - Critical entities with financial balances or seats (e.g., `Group`, `StudentBalance`) MUST have concurrency tokens (`[Timestamp]` or `.IsRowVersion()`) to prevent race conditions during rapid registrations.
3. **UnitOfWork Commitment:**
   - Repositories only prepare mutations (`Add`, `Update`, `Remove`). Actual `SaveChangesAsync` MUST only be committed through the Unit of Work pipeline.
