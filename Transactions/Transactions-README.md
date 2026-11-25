::: {align="center"}
# Pennywise · Transactions

### Financial activity management, structured for a clean N-Tier application.

[![.NET
10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET
Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core)
[![Entity Framework
Core](https://img.shields.io/badge/Entity_Framework_Core-ORM-6C3483?style=for-the-badge)](https://learn.microsoft.com/ef/core/)
[![SQL
Server](https://img.shields.io/badge/SQL_Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)

**A dedicated module of the Pennywise Expense & Budget Tracker**

[Repository](https://github.com/GhofranMohamed2006/Expense-Budget-Tracker)
· **Module owner:** Ebrahim Elghazaly
:::

------------------------------------------------------------------------

## Table of contents

-   [01 · Introduction](#01--introduction)
-   [02 · Functional scope](#02--functional-scope)
-   [03 · Architecture](#03--architecture)
-   [04 · Project structure](#04--project-structure)
-   [05 · Transaction rules](#05--transaction-rules)
-   [06 · Data model and persistence](#06--data-model-and-persistence)
-   [07 · Validation](#07--validation)
-   [08 · Integration checklist](#08--integration-checklist)
-   [09 · Build and verification](#09--build-and-verification)
-   [10 · Ownership](#10--ownership)

------------------------------------------------------------------------

## 01 · Introduction

The **Transactions** module is responsible for the
transaction-management domain in Pennywise. It organizes the
application's financial activity around three transaction types:
**Income**, **Expense**, and **Transfer**.

The implementation is organized according to the solution's **N-Tier
Architecture**. HTTP handling, business operations, validation,
data-transfer contracts, and Entity Framework Core mapping are kept in
their respective areas.

> **Repository boundary:** this directory contains Ebrahim Elghazaly's
> Transactions contribution. It is intended to be integrated with the
> shared Pennywise solution; it is not a separate, self-contained
> application.

## 02 · Functional scope

  -----------------------------------------------------------------------
  Capability                          Description
  ----------------------------------- -----------------------------------
  Income                              Records money received into a
                                      source account.

  Expense                             Records money spent from a source
                                      account.

  Transfer                            Records movement from a source
                                      account to a different destination
                                      account.

  Categorization                      Supports a category relationship
                                      where applicable.

  Transaction details                 Carries amount, date, optional
                                      description, and timestamps.

  API layer                           Exposes transaction operations
                                      through the Transactions
                                      controller.

  Business layer                      Encapsulates transaction operations
                                      behind a service contract.

  Persistence mapping                 Defines relationships, column
                                      constraints, delete behavior, and
                                      indexes.
  -----------------------------------------------------------------------

### Transaction lifecycle at a glance

``` mermaid
flowchart TD
    A[HTTP request] --> B[TransactionsController]
    B --> C[Request DTO]
    C --> D[Transaction validation]
    D --> E[Transaction service]
    E --> F[ApplicationDbContext]
    F --> G[(SQL Server)]
    G --> H[Response DTO]
    H --> I[HTTP response]
```

The diagram shows the module's architectural flow; exact routes and
response codes are defined by the controller implementation.

## 03 · Architecture

The module follows the existing Pennywise project boundaries.

``` mermaid
flowchart TB
    subgraph Presentation
        A[TransactionsController]
    end

    subgraph BLL[Business Logic Layer]
        B[DTOs]
        C[Validators]
        D[ITransactionService]
        E[TransactionService]
    end

    subgraph DAL[Data Access Layer]
        F[Transaction Entity]
        G[TransactionConfiguration]
        H[ApplicationDbContext]
    end

    subgraph Common
        J[TransactionType enum]
    end

    A --> B
    A --> D
    D --> E
    E --> H
    H --> F
    H --> G
    B --> J
    C --> B
```

  ---------------------------------------------------------------------------------------
  Layer                   Responsibility          Module location
  ----------------------- ----------------------- ---------------------------------------
  Presentation            Handles incoming HTTP   `Pennywise.Presentation/Controllers/`
                          requests and API        
                          responses.              

  Business Logic (BLL)    Defines DTOs,           `Pennywise.BLL/`
                          validation, service     
                          contracts, and          
                          transaction operations. 

  Data Access (DAL)       Defines the transaction `Pennywise.DAL/`
                          entity and EF Core      
                          configuration.          

  Common                  Supplies shared         `Pennywise.Common/Enums/` in the main
                          transaction-type        solution
                          definitions.            
  ---------------------------------------------------------------------------------------

## 04 · Project structure

``` text
Transactions/
│
├── README.md
│
├── Pennywise.DAL/
│   ├── Entities/
│   │   └── Transaction.cs
│   └── Configurations/
│       └── TransactionConfiguration.cs
│
├── Pennywise.BLL/
│   ├── DTOs/
│   │   └── Transactions/
│   │       ├── CreateTransactionsDTO.cs
│   │       ├── UpdateTransactionsDTO.cs
│   │       └── TransactionResponseDTO.cs
│   │
│   ├── Interfaces/
│   │   └── Transactions/
│   │       └── ITransactionService.cs
│   │
│   ├── Services/
│   │   └── Transactions/
│   │       └── TransactionService.cs
│   │
│   └── Validators/
│       └── Transactions/
│           ├── CreateTransactionValidator.cs
│           └── UpdateTransactionValidator.cs
│
└── Pennywise.Presentation/
    └── Controllers/
        └── TransactionsController.cs
```

## 05 · Transaction rules

The create validator applies different rules according to the selected
transaction type.

  -------------------------------------------------------------------------
  Rule                Income              Expense            Transfer
  ------------- ------------------- ------------------- -------------------
  Positive           Required            Required            Required
  source                                                
  account ID                                            

  Amount             Required            Required            Required
  greater than                                          
  zero                                                  

  Valid              Required            Required            Required
  transaction                                           
  date                                                  

  Category ID        Required            Required       Not required by the
                                                         create validator

  Destination      Must be null        Must be null          Required
  account ID                                            

  Source and            ---                 ---              Required
  destination                                           
  must differ                                           
  -------------------------------------------------------------------------

Additional input constraints:

-   `Description` is optional and limited to **500 characters**.
-   A supplied `CategoryId` must be a positive integer.
-   The transaction type must be a defined `TransactionType` enum value.

## 06 · Data model and persistence

The `Transaction` entity is configured through
`TransactionConfiguration`, which is applied by the shared
`ApplicationDbContext`.

### Relationships

  -----------------------------------------------------------------------
  Relationship            Foreign key             Configuration
  ----------------------- ----------------------- -----------------------
  User                    `UserId`                Required; delete
                                                  restricted

  Source account          `AccountId`             Required; delete
                                                  restricted

  Destination account     `TransferToAccountId`   Optional; delete
                                                  restricted

  Category                `CategoryId`            Optional; delete
                                                  restricted
  -----------------------------------------------------------------------

### Column and index configuration

  Property            Database configuration
  ------------------- -----------------------------------------------
  `Id`                Primary key; generated on add
  `Amount`            Required `decimal(18,2)`
  `Type`              Required string conversion; maximum length 20
  `TransactionDate`   Required
  `Description`       Optional; maximum length 500
  `CreatedAt`         Required
  `UpdatedAt`         Optional

Configured indexes:

-   `UserId`
-   `AccountId`
-   `TransferToAccountId`
-   `CategoryId`
-   `TransactionDate`
-   Composite index: `(UserId, TransactionDate)`

## 07 · Validation

Validation is separated into dedicated create and update validators.

  -----------------------------------------------------------------------
  Validator                           Responsibility
  ----------------------------------- -----------------------------------
  `CreateTransactionValidator`        Checks account, amount, type, date,
                                      description, category, and
                                      transfer-specific rules.

  `UpdateTransactionValidator`        Checks amount, date, description
                                      length, and a supplied category ID.
  -----------------------------------------------------------------------

The DTOs also declare data-annotation constraints. Keep the validator
behavior and DTO contract aligned when making future changes.

## 08 · Integration checklist

This module depends on shared Pennywise entities, enums, database
configuration, and application infrastructure. Before merging or
integrating it, verify the following with the team:

-   [ ] The shared solution contains the matching `Pennywise.DAL`,
    `Pennywise.BLL`, `Pennywise.Common`, and Presentation projects.
-   [ ] `ApplicationDbContext` exposes `DbSet<Transaction>` and applies
    assembly configurations.
-   [ ] Related `ApplicationUser`, `Account`, and `Category` entities
    contain the navigation properties expected by the transaction
    configuration.
-   [ ] `TransactionType` contains the values `Income`, `Expense`, and
    `Transfer`.
-   [ ] `ITransactionService` and `TransactionService` are registered in
    dependency injection.
-   [ ] The target environment has a valid SQL Server connection string.
-   [ ] Database migrations are handled through the team's shared
    migration workflow.
-   [ ] Authentication, authorization, and user-ownership behavior are
    reviewed against the team's agreed identity contract.

**Integration note:** the module's source files reference types in the
wider Pennywise solution. Copying this folder alone does not create a
runnable application.

## 09 · Build and verification

Run these commands from the **full Pennywise solution** directory, not
from the isolated `Transactions` folder.

``` powershell
dotnet restore
dotnet build
```

To start the Presentation project, use the solution's configured
environment and database settings:

``` powershell
dotnet run --project .\Pennywise.Presentation\Pennywise.PL.csproj
```

The module's source solution was built successfully during development.
A successful build in another environment still depends on its installed
.NET SDK, restored packages, shared project files, and configuration.

### Source of implementation details

For exact endpoint routes, HTTP verbs, payloads, and status codes, refer
to:

-   `Pennywise.Presentation/Controllers/TransactionsController.cs`
-   `Pennywise.BLL/DTOs/Transactions/`
-   `Pennywise.BLL/Interfaces/Transactions/ITransactionService.cs`

This document avoids hard-coding endpoint details so it does not become
inconsistent with the controller.

## 10 · Ownership

**Module:** Transactions\
**Developer:** **Ebrahim Elghazaly**\
**Parent project:** Pennywise --- Expense & Budget Tracker\
**Architecture:** N-Tier Architecture\
**Technology:** ASP.NET Core Web API · Entity Framework Core · SQL
Server

------------------------------------------------------------------------

::: {align="center"}
**Pennywise · Transactions Module**

*Structured financial activity. Clear separation of responsibilities.*
:::
