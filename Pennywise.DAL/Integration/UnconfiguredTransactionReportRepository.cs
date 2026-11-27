using Pennywise.Common.Models;
using Pennywise.DAL.Abstractions;

namespace Pennywise.DAL.Integration;

/// <summary>
/// Temporary integration guard. Replace this adapter with the main
/// Pennywise EF Core repository when the shared DbContext and entities
/// are available. It intentionally does not return fabricated data.
/// </summary>
public sealed class UnconfiguredTransactionReportRepository
    : ITransactionReportRepository
{
    public Task<IReadOnlyList<ReportTransaction>> GetTransactionsAsync(
        string userId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(
            "Transaction reporting data source is not configured. " +
            "Connect the shared Pennywise transaction repository.");
    }
}
