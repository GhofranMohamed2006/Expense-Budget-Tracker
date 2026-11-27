namespace Pennywise.Common.DTOs;

public sealed record DashboardSummaryDto(
    decimal TotalBalance,
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetSavings
);
