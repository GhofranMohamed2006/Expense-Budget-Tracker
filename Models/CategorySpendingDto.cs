namespace Pennywise.Common.Models;

public sealed record CategorySpendingDto(
    string Category,
    decimal Amount,
    decimal Percentage
);
