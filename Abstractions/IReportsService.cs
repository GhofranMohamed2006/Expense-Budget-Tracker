using Pennywise.Common.Models;

namespace Pennywise.BLL.Abstractions;

public interface IReportsService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MonthlyReportDto>> GetMonthlyAsync(
        int year,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategorySpendingDto>> GetCategoriesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SpendingTrendDto>> GetTrendsAsync(
        int months,
        CancellationToken cancellationToken = default);
}
