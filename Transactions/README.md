# Pennywise – Transactions Module

**Developer:** Ebrahim Elghazaly
**Module:** Transactions
**Architecture:** N-Tier Architecture
**Platform:** ASP.NET Core Web API
**Database:** SQL Server / Entity Framework Core

---

## 1. Module Overview

The Transactions Module is responsible for managing financial transactions within Pennywise, an Expense and Budget Tracking application.

It supports three transaction types:

- Income
- Expense
- Transfer

The module follows N-Tier Architecture to separate API communication, business logic, and database access.

## 2. Project Structure

```text
Transactions/
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
├── Pennywise.Presentation/
│   └── Controllers/
│       └── TransactionsController.cs
│
└── README.md


3. Architecture Responsibilities
Data Access Layer (DAL)
Responsible for database entities and Entity Framework Core configurations.

Files:
- Transaction.cs
- TransactionConfiguration.cs
Business Logic Layer (BLL)
Responsible for transaction operations, business rules, validation, and data transfer objects.

Files:
- CreateTransactionsDTO.cs
- UpdateTransactionsDTO.cs
- TransactionResponseDTO.cs
- ITransactionService.cs
- TransactionService.cs
- CreateTransactionValidator.cs
- UpdateTransactionValidator.cs
Presentation Layer
Responsible for receiving HTTP requests and returning API responses.

File:

- TransactionsController.cs
4. Transaction Entity
The Transaction entity follows the Pennywise Shared Backend Contract.
PropertyType
Idint
UserIdstring
AccountIdint
CategoryIdint?
TransferToAccountIdint?
TypeTransactionType
Amountdecimal
TransactionDateDateTime
Descriptionstring?
CreatedAtDateTime
UpdatedAtDateTime?

Financial amounts use decimal(18,2).

Transaction timestamps are intended to use UTC.
5. Transaction Types
Income
Represents money entering an account.

Rules:
- Requires a valid source account.
- Requires an Income category.
- Amount must be greater than zero.
- The account must belong to the authenticated user.
Expense
Represents money leaving an account.

Rules:
- Requires a valid source account.
- Requires an Expense category.
- Amount must be greater than zero.
- The account must belong to the authenticated user.
Transfer
Represents money moving between two accounts.

Rules:
- Requires a source account.
- Requires a destination account.
- Source and destination accounts must be different.
- Both accounts must belong to the authenticated user.
- Category is optional.
- Amount must be greater than zero.
6. Validation
The module validates:
- Positive transaction amount.
- Valid transaction type.
- Valid source account ID.
- Category requirements.
- Category type compatibility.
- Transfer destination requirements.
- Different source and destination accounts.
- Description maximum length of 500 characters.
- Valid transaction date.
Database ownership checks are handled inside the service layer.
7. DTOs
CreateTransactionDto
Contains:
- AccountId
- CategoryId
- TransferToAccountId
- Type
- Amount
- TransactionDate
- Description
UpdateTransactionDto
Contains:
- CategoryId
- Amount
- TransactionDate
- Description
The update operation preserves transaction type and source/destination accounts.
TransactionResponseDto
Returns transaction information, including:
- Transaction ID
- Source account ID and name
- Category ID and name
- Destination account ID and name
- Transaction type
- Amount
- Transaction date
- Description
- Creation and update timestamps
The additional response properties should remain aligned with the team's approved API contract.
8. Service Operations
ITransactionService provides the following operations:
MethodDescription
GetAllAsyncRetrieve user transactions
GetByIdAsyncRetrieve a specific transaction
CreateAsyncCreate a transaction
UpdateAsyncUpdate allowed transaction details
DeleteAsyncDelete a transaction

All database operations use asynchronous Entity Framework Core methods.
9. API Endpoints
Base Route:

/api/transactions

All transaction endpoints require authentication.
HTTP MethodEndpointDescription
GET/api/transactionsGet transactions
GET/api/transactions/Get transaction details
POST/api/transactionsCreate transaction
PUT/api/transactions/Update transaction
DELETE/api/transactions/Delete transaction

Optional Query Parameters
The GET endpoint supports:
- type
- accountId
- categoryId
- fromDate
- toDate
- pageNumber
- pageSize
Pagination is supported.
10. Authentication and Security
The controller uses JWT-based authorization.

The current user ID is extracted from authenticated JWT claims.

The API does not accept UserId from frontend request bodies.

Each operation checks transaction ownership.

Account and category ownership are validated by the service.
11. Account Balance Integration
The Transactions Module does not manually store or update account balances.

Account balance is calculated by the Accounts Module using:
Balance = OpeningBalance
        + Income
        - Expense
        - Transfer Out
        + Transfer In


The Accounts Module is responsible for balance calculations.
12. Shared Dependencies
This module relies on existing components from the main Pennywise solution.

Required shared components:
- Pennywise.Common.Enums.TransactionType
- Pennywise.Common.Enums.CategoryType
- Pennywise.DAL.Entities.Account
- Pennywise.DAL.Entities.Category
- Pennywise.DAL.Entities.ApplicationUser
- Pennywise.DAL.ApplicationDbContext
Required database context integration:
- Transactions DbSet
- TransactionConfiguration discovery
Required dependency injection:

ITransactionService → TransactionService

Required authentication:

JWT authentication configured by the shared Authentication module.

Important: Shared files are intentionally not duplicated in this module.
13. Database Integration
The following files are shared team responsibilities:
- ApplicationDbContext.cs
- Program.cs
- Shared Enums
- Project files
- Database migrations
TransactionConfiguration implements:

IEntityTypeConfiguration<Transaction>

The main DbContext should discover configurations through:

ApplyConfigurationsFromAssembly

Database migrations must be coordinated with the designated database integration owner.
14. Team Integration Notes
Before merging:
1. Confirm Account and Category navigation properties with their owners.
2. Confirm the shared TransactionType and CategoryType enums.
3. Coordinate ApplicationDbContext integration.
4. Coordinate JWT configuration with the Authentication developer.
5. Confirm additional response DTO properties.
6. Confirm the category update behavior.
7. Coordinate database migration creation.
15. Build and Testing
The original Pennywise solution successfully completed:
dotnet build Pennywise.PL.csproj


The Transactions directory contains module source files only.

It intentionally does not include the complete shared projects and cannot be built independently without integrating its dependencies into the main solution.

API verification should cover:
- Income creation.
- Expense creation.
- Transfer creation.
- Invalid transaction amounts.
- Invalid categories.
- Account ownership.
- Transaction ownership.
- Update behavior.
- Delete behavior.
- Unauthorized access.
Swagger or Postman should be used for endpoint testing.
16. Git Workflow
Development branch:

feature/transactions

All changes should be committed through the dedicated feature branch.

The module should be submitted through a Pull Request after integration checks.

Developer: Ebrahim Elghazaly
Project: Pennywise – Expense & Budget Tracker
Module: Transactions
Architecture: N-Tier Architecture
