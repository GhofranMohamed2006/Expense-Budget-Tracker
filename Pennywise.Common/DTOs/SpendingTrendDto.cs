namespace Pennywise.Common.DTOs;

public sealed record SpendingTrendDto(
    int Year,
    int Month,
    decimal TotalExpenses
);
