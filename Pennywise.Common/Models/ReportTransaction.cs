namespace Pennywise.Common.Models;

public sealed record ReportTransaction(
    Guid Id,
    string UserId,
    decimal Amount,
    TransactionType Type,
    DateTime TransactionDate,
    Guid? CategoryId,
    string? CategoryName
);
