namespace Pennywise.Common.DTOs;

public sealed record MonthlyReportDto(
    int Year,
    int Month,
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetSavings
);
