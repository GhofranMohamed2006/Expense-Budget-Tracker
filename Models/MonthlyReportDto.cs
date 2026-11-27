namespace Pennywise.Common.Models;

public sealed record MonthlyReportDto(
    int Year,
    int Month,
    decimal Income,
    decimal Expenses,
    decimal Balance,
    int TransactionCount
);
