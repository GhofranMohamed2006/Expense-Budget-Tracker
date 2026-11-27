using Pennywise.Common.Abstractions;
using Pennywise.Common.Models;

namespace Pennywise.DAL.Integration;

public sealed class UnconfiguredTransactionReportRepository
    : ITransactionReportRepository
{
    public Task<IReadOnlyList<ReportTransaction>> GetTransactionsAsync(
        string userId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        throw new ReportsIntegrationUnavailableException();
    }
}
