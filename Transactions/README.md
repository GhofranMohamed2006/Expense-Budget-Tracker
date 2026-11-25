# Pennywise --- Transactions Module

```{=html}
<p align="center">
```
`<strong>`{=html}A modular transaction-management component for the
Pennywise Expense & Budget Tracker`</strong>`{=html}
```{=html}
</p>
```
```{=html}
<p align="center">
```
`<img alt=".NET" src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white">`{=html}
`<img alt="ASP.NET Core" src="https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?logo=dotnet&logoColor=white">`{=html}
`<img alt="Entity Framework Core" src="https://img.shields.io/badge/Entity_Framework_Core-ORM-6C2EB9">`{=html}
`<img alt="SQL Server" src="https://img.shields.io/badge/SQL_Server-Database-CC2927?logo=microsoftsqlserver&logoColor=white">`{=html}
`<img alt="Architecture" src="https://img.shields.io/badge/Architecture-N--Tier-0A7A5A">`{=html}
```{=html}
</p>
```

------------------------------------------------------------------------

## Overview

The **Transactions Module** is the transaction-management component of
**Pennywise**, an expense and budget tracking application.

It is organized using an **N-Tier Architecture** to keep API handling,
business operations, data mapping, and persistence concerns separated.
The module is designed around three transaction types:

-   **Income** --- records money received into an account.
-   **Expense** --- records money spent from an account.
-   **Transfer** --- records money moved from a source account to a
    destination account.

**Module owner:** Ebrahim Elghazaly\
**Repository:**
[Expense-Budget-Tracker](https://github.com/GhofranMohamed2006/Expense-Budget-Tracker)\
**Module directory:** `Transactions/`

## Features

-   Create, retrieve, update, and delete transaction operations exposed
    through the Transactions API controller, according to the
    implemented controller actions.
-   Dedicated request and response DTOs.
-   Separate create and update validation rules.
-   Support for Income, Expense, and Transfer transaction types.
-   Optional transaction descriptions with a maximum length of 500
    characters.
-   Decimal amount mapping using `decimal(18,2)` precision.
-   Entity Framework Core relationship configuration for users,
    accounts, destination accounts, and categories.
-   Database indexes on commonly queried transaction fields.
-   A dedicated service interface and implementation to keep business
    logic out of the API controller.

## Architecture

The module follows the solution's N-Tier separation:

  -----------------------------------------------------------------------------
  Layer                   Responsibility          Transactions files
  ----------------------- ----------------------- -----------------------------
  Presentation            Receives HTTP requests  `TransactionsController.cs`
                          and returns API         
                          responses               

  Business Logic (BLL)    Defines transaction     `DTOs/`, `Interfaces/`,
                          operations, DTOs,       `Services/`, `Validators/`
                          validation, and service 
                          logic                   

  Data Access (DAL)       Defines the transaction `Entities/`,
                          entity and EF Core      `Configurations/`
                          mapping                 

  Common                  Holds shared enums used `Pennywise.Common/Enums/`
                          by the solution         
  -----------------------------------------------------------------------------

### Module structure

``` text
Transactions/
├── README.md
├── Pennywise.DAL/
│   ├── Entities/
│   │   └── Transaction.cs
│   └── Configurations/
│       └── TransactionConfiguration.cs
├── Pennywise.BLL/
│   ├── DTOs/
│   │   └── Transactions/
│   │       ├── CreateTransactionsDTO.cs
│   │       ├── UpdateTransactionsDTO.cs
│   │       └── TransactionResponseDTO.cs
│   ├── Interfaces/
│   │   └── Transactions/
│   │       └── ITransactionService.cs
│   ├── Services/
│   │   └── Transactions/
│   │       └── TransactionService.cs
│   └── Validators/
│       └── Transactions/
│           ├── CreateTransactionValidator.cs
│           └── UpdateTransactionValidator.cs
└── Pennywise.Presentation/
    └── Controllers/
        └── TransactionsController.cs
```

> The module is maintained in its own `Transactions/` directory in the
> shared repository. Its source files reference the corresponding
> Pennywise solution projects and shared entities/enums.

## Transaction model

The transaction entity is associated with:

-   A user (`UserId`).
-   A source account (`AccountId`).
-   An optional destination account (`TransferToAccountId`), used for
    transfers.
-   An optional category (`CategoryId`).
-   A transaction type, amount, date, and optional description.
-   Creation and update timestamps.

### Transaction types and validation

  -----------------------------------------------------------------------
  Type                    Category                Destination account
  ----------------------- ----------------------- -----------------------
  Income                  Required                Must not be supplied

  Expense                 Required                Must not be supplied

  Transfer                Optional                Required and must
                                                  differ from the source
                                                  account
  -----------------------------------------------------------------------

Additional validation includes:

-   Source account ID must be a positive integer.
-   Amount must be greater than zero.
-   Transaction type must be a defined `TransactionType` value.
-   Transaction date must be valid.
-   Description must not exceed 500 characters.
-   A supplied category ID must be positive.

## Data configuration

`TransactionConfiguration` configures the EF Core entity mapping,
including:

-   Primary key generation.
-   Required user and source-account references.
-   `Amount` as `decimal(18,2)`.
-   Transaction type stored as a string with a maximum length of 20.
-   Optional description with a maximum length of 500.
-   Relationships to the user, source account, destination account, and
    category.
-   `DeleteBehavior.Restrict` for the configured relationships.
-   Indexes on `UserId`, `AccountId`, `TransferToAccountId`,
    `CategoryId`, and `TransactionDate`, plus a composite index on
    `UserId` and `TransactionDate`.

## Main components

  -----------------------------------------------------------------------
  File                                Purpose
  ----------------------------------- -----------------------------------
  `Transaction.cs`                    Transaction data entity

  `TransactionConfiguration.cs`       Entity Framework Core mapping and
                                      relationships

  `CreateTransactionsDTO.cs`          Input model for creating a
                                      transaction

  `UpdateTransactionsDTO.cs`          Input model for updating a
                                      transaction

  `TransactionResponseDTO.cs`         Response model for transaction data

  `ITransactionService.cs`            Contract for transaction business
                                      operations

  `TransactionService.cs`             Transaction business-logic
                                      implementation

  `CreateTransactionValidator.cs`     Validation rules for creating
                                      transactions

  `UpdateTransactionValidator.cs`     Validation rules for updating
                                      transactions

  `TransactionsController.cs`         HTTP API endpoints for the module
  -----------------------------------------------------------------------

## Integration requirements

The module is part of the wider Pennywise solution and depends on shared
solution components. When integrating it into another checkout of the
application, verify that:

1.  The corresponding `Pennywise.DAL`, `Pennywise.BLL`,
    `Pennywise.Common`, and Presentation projects are present.
2.  The `Transaction` entity and its related `Account`, `Category`, and
    `ApplicationUser` entities are consistent with the shared solution.
3.  `ApplicationDbContext` exposes `DbSet<Transaction>` and applies the
    entity configurations.
4.  The transaction service is registered in dependency injection.
5.  The SQL Server connection string and database schema are configured
    for the target environment.
6.  Authentication and authorization configuration is aligned with the
    shared application's identity/JWT implementation.
7.  Required database migrations are created and applied by the team
    using the shared migration strategy.

> This repository folder is a contribution module, not a standalone
> runnable solution. Build and run the complete Pennywise solution using
> the team's agreed startup project and configuration.

## Build and verification

From the full Pennywise solution directory, restore and build the
solution:

``` powershell
dotnet restore
dotnet build
```

Run the Presentation project using the solution's configured environment
and connection string:

``` powershell
dotnet run --project .\Pennywise.Presentation\Pennywise.PL.csproj
```

The exact startup settings, database availability, authentication
configuration, and migrations must be supplied by the integrated
solution.

## API documentation

The HTTP routes and request/response contracts are defined in:

-   `Pennywise.Presentation/Controllers/TransactionsController.cs`
-   `Pennywise.BLL/DTOs/Transactions/`

Consult these files for the exact route templates, HTTP verbs, status
codes, and payload fields. This README intentionally does not duplicate
route names that may change during integration.

## Design principles

-   **Separation of concerns:** each tier has a focused responsibility.
-   **Maintainability:** DTOs, validators, interfaces, services, and
    persistence mapping are separated.
-   **Data integrity:** relationships and delete behaviors are
    explicitly configured.
-   **Validation:** transaction-specific rules are checked before
    business operations are completed.
-   **Integration-first:** shared user, account, category, enum,
    database, and authentication contracts remain owned by the main
    Pennywise solution.

## Author

**Ebrahim Elghazaly**\
Transactions Module --- Pennywise Expense & Budget Tracker

------------------------------------------------------------------------

```{=html}
<p align="center">
```
`<sub>`{=html}Part of the Pennywise Expense & Budget Tracker
project.`</sub>`{=html}
```{=html}
</p>
```
