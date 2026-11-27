namespace Pennywise.Common.Models;

public sealed record DashboardSummaryDto(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal TotalBalance,
    decimal NetSavings,
    int TransactionCount
);
