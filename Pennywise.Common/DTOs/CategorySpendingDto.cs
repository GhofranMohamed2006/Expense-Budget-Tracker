namespace Pennywise.Common.DTOs;

public sealed record CategorySpendingDto(
    Guid? CategoryId,
    string CategoryName,
    decimal TotalAmount,
    decimal Percentage
);
