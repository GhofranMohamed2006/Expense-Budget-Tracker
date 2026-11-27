using Pennywise.Common.DTOs;

namespace Pennywise.BLL.Abstractions;

public interface IReportsService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MonthlyReportDto>> GetMonthlyReportsAsync(
        int year, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategorySpendingDto>> GetCategorySpendingAsync(
        int year, int month, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SpendingTrendDto>> GetSpendingTrendsAsync(
        int months, CancellationToken cancellationToken = default);
}
