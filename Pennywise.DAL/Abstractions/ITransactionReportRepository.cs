using Pennywise.Common.Models;

namespace Pennywise.DAL.Abstractions;

public interface ITransactionReportRepository
{
    Task<IReadOnlyList<ReportTransaction>> GetTransactionsAsync(
        string userId,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default);
}
