using Pennywise.Common.Models;

namespace Pennywise.Common.Abstractions;

public interface ITransactionReportRepository
{
    Task<IReadOnlyList<ReportTransaction>> GetTransactionsAsync(
        string userId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default
    );
}
