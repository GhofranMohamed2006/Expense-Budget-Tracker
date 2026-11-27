using Pennywise.BLL.Abstractions;
using Pennywise.Common.Abstractions;
using Pennywise.Common.Models;

namespace Pennywise.BLL.Services;

public sealed class ReportsService : IReportsService
{
    private readonly ITransactionReportRepository _repository;
    private readonly ICurrentUserContext _currentUser;

    public ReportsService(
        ITransactionReportRepository repository,
        ICurrentUserContext currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    private string GetUserId()
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
            throw new UnauthorizedAccessException("An authenticated user is required.");

        return _currentUser.UserId;
    }

    private async Task<IReadOnlyList<ReportTransaction>> LoadAsync(
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var transactions = await _repository.GetTransactionsAsync(
            userId, from, to, cancellationToken);

        return transactions
            .Where(x => x.UserId == userId)
            .ToList();
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(null, null, cancellationToken);

        var income = data
            .Where(x => x.Type == TransactionType.Income)
            .Sum(x => x.Amount);

        var expenses = data
            .Where(x => x.Type == TransactionType.Expense)
            .Sum(x => x.Amount);

        return new DashboardSummaryDto(
            income,
            expenses,
            income - expenses,
            income - expenses,
            data.Count);
    }

    public async Task<IReadOnlyList<MonthlyReportDto>> GetMonthlyAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        if (year < 2000 || year > 9999)
            throw new ArgumentOutOfRangeException(nameof(year));

        var data = await LoadAsync(
            new DateTime(year, 1, 1),
            new DateTime(year + 1, 1, 1),
            cancellationToken);

        return data
            .GroupBy(x => new { x.Date.Year, x.Date.Month })
            .OrderBy(x => x.Key.Month)
            .Select(g =>
            {
                var income = g.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount);
                var expenses = g.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount);

                return new MonthlyReportDto(
                    g.Key.Year,
                    g.Key.Month,
                    income,
                    expenses,
                    income - expenses,
                    g.Count());
            })
            .ToList();
    }

    public async Task<IReadOnlyList<CategorySpendingDto>> GetCategoriesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        if (year < 2000 || year > 9999)
            throw new ArgumentOutOfRangeException(nameof(year));

        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var expenses = (await LoadAsync(from, to, cancellationToken))
            .Where(x => x.Type == TransactionType.Expense)
            .ToList();

        var total = expenses.Sum(x => x.Amount);

        return expenses
            .GroupBy(x => x.Category)
            .Select(g => new CategorySpendingDto(
                g.Key,
                g.Sum(x => x.Amount),
                total == 0 ? 0 : Math.Round(g.Sum(x => x.Amount) / total * 100, 2)))
            .OrderByDescending(x => x.Amount)
            .ToList();
    }

    public async Task<IReadOnlyList<SpendingTrendDto>> GetTrendsAsync(
        int months,
        CancellationToken cancellationToken = default)
    {
        if (months < 1 || months > 60)
            throw new ArgumentOutOfRangeException(nameof(months));

        var current = DateTime.UtcNow;
        var firstMonth = new DateTime(current.Year, current.Month, 1)
            .AddMonths(-(months - 1));

        var end = firstMonth.AddMonths(months);

        var data = await LoadAsync(firstMonth, end, cancellationToken);

        return Enumerable.Range(0, months)
            .Select(offset =>
            {
                var date = firstMonth.AddMonths(offset);

                var monthly = data.Where(x =>
                    x.Date.Year == date.Year &&
                    x.Date.Month == date.Month);

                var income = monthly
                    .Where(x => x.Type == TransactionType.Income)
                    .Sum(x => x.Amount);

                var expenses = monthly
                    .Where(x => x.Type == TransactionType.Expense)
                    .Sum(x => x.Amount);

                return new SpendingTrendDto(
                    date.Year,
                    date.Month,
                    income,
                    expenses,
                    income - expenses);
            })
            .ToList();
    }
}
