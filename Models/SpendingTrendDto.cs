namespace Pennywise.Common.Models;

public sealed record SpendingTrendDto(
    int Year,
    int Month,
    decimal Income,
    decimal Expenses,
    decimal Balance
);
