namespace Pennywise.Common.Models;

public sealed class ReportTransaction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string UserId { get; init; }
    public decimal Amount { get; init; }
    public TransactionType Type { get; init; }
    public string Category { get; init; } = "Uncategorized";
    public DateTime Date { get; init; }
}
