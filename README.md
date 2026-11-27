# Ziad - Pennywise Reports Module

## Overview

An independent .NET 10 Reports API module prepared for future integration
with the main Pennywise solution.

## Architecture

- Pennywise.Common: models, DTOs, repository and user contracts.
- Pennywise.BLL: reporting calculations and business rules.
- Pennywise.DAL: data access boundary and unconfigured adapter.
- Reports-Integration.PL: ASP.NET Core API and HTTP infrastructure.
- Reports-Integration.Tests: isolated xUnit tests.

## Important Integration Boundary

The production repository is intentionally unconfigured.

No Pennywise database, EF Core context, external authentication,
or other team member's project is referenced.

The API returns HTTP 503 for report requests when the data source
is not configured.

The health endpoint is independent of database availability.

## Endpoints

- GET /api/reports/summary
- GET /api/reports/monthly?year=2026
- GET /api/reports/categories?year=2026&month=11
- GET /api/reports/trends?months=6
- GET /health

Reports require an authenticated HTTP user with a NameIdentifier or sub claim.
The module does not configure an authentication provider.

## Business Rules

- Transfers are excluded from income and expense totals.
- Balance = income - expenses.
- Category percentages are calculated from total expenses.
- User filtering is enforced by the service and repository contract.
- Empty datasets return zero-valued calculations.
- Monthly trends include months without transactions.

## Run

```powershell
dotnet restore
dotnet build
dotnet run --project Reports-Integration.PL
```

## Test

```powershell
dotnet test
```

## Future GitHub Integration

1. Commit this independent module to Ziad's branch.
2. Push the branch to GitHub.
3. Open a Pull Request against the main project.
4. Review namespace and project-reference compatibility.
5. Implement an adapter for the actual Pennywise database.
6. Connect the existing authentication system.
7. Register the adapter and authentication dependencies.
8. Run the integrated solution tests.

No merge or database connection is performed by this module.
