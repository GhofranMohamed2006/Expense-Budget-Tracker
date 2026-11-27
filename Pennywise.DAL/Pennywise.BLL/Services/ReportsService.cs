using Pennywise.BLL.Abstractions;
using Pennywise.Common.DTOs;
using Pennywise.Common.Models;
using Pennywise.DAL.Abstractions;

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

    private string RequireUserId()
    {
        return !string.IsNullOrWhiteSpace(_currentUser.UserId)
            ? _currentUser.UserId
            : throw new UnauthorizedAccessException("Authenticated user is required.");
    }

    private async Task<IReadOnlyList<ReportTransaction>> LoadAsync(
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken)
    {
        var userId = RequireUserId();

        var rows = await _repository.GetTransactionsAsync(
            userId, from, to, cancellationToken);

        // Defense in depth: never trust a repository to return another user's rows.
        return rows.Where(x => x.UserId == userId).ToList();
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await LoadAsync(null, null, cancellationToken);

        var income = rows
            .Where(x => x.Type == TransactionType.Income)
            .Sum(x => x.Amount);

        var expenses = rows
            .Where(x => x.Type == TransactionType.Expense)
            .Sum(x => x.Amount);

        return new DashboardSummaryDto(
            TotalBalance: income - expenses,
            TotalIncome: income,
            TotalExpenses: expenses,
            NetSavings: income - expenses);
    }

    public async Task<IReadOnlyList<MonthlyReportDto>> GetMonthlyReportsAsync(
        int year, CancellationToken cancellationToken = default)
    {
        if (year < 1 || year > 9998)
            throw new ArgumentOutOfRangeException(nameof(year));

        var rows = await LoadAsync(
            new DateTime(year, 1, 1),
            new DateTime(year + 1, 1, 1).AddTicks(-1),
            cancellationToken);

        return rows
            .Where(x => x.Type is TransactionType.Income or TransactionType.Expense)
            .GroupBy(x => new { x.TransactionDate.Year, x.TransactionDate.Month })
            .OrderBy(x => x.Key.Month)
            .Select(group =>
            {
                var income = group
                    .Where(x => x.Type == TransactionType.Income)
                    .Sum(x => x.Amount);

                var expenses = group
                    .Where(x => x.Type == TransactionType.Expense)
                    .Sum(x => x.Amount);

                return new MonthlyReportDto(
                    group.Key.Year,
                    group.Key.Month,
                    income,
                    expenses,
                    income - expenses);
            })
            .ToList();
    }

    public async Task<IReadOnlyList<CategorySpendingDto>> GetCategorySpendingAsync(
        int year, int month, CancellationToken cancellationToken = default)
    {
        if (year < 1 || year > 9998)
            throw new ArgumentOutOfRangeException(nameof(year));

        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1).AddTicks(-1);

        var expenses = (await LoadAsync(from, to, cancellationToken))
            .Where(x => x.Type == TransactionType.Expense)
            .ToList();

        var total = expenses.Sum(x => x.Amount);

        return expenses
            .GroupBy(x => new { x.CategoryId, x.CategoryName })
            .Select(group =>
            {
                var amount = group.Sum(x => x.Amount);
                var percentage = total == 0
                    ? 0
                    : Math.Round(amount / total * 100m, 2);

                return new CategorySpendingDto(
                    group.Key.CategoryId,
                    group.Key.CategoryName ?? "Uncategorized",
                    amount,
                    percentage);
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }

    public async Task<IReadOnlyList<SpendingTrendDto>> GetSpendingTrendsAsync(
        int months, CancellationToken cancellationToken = default)
    {
        if (months < 1 || months > 60)
            throw new ArgumentOutOfRangeException(nameof(months));

        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var start = currentMonth.AddMonths(-(months - 1));
        var end = currentMonth.AddMonths(1).AddTicks(-1);

        var rows = await LoadAsync(start, end, cancellationToken);

        var grouped = rows
            .Where(x => x.Type == TransactionType.Expense)
            .GroupBy(x => new { x.TransactionDate.Year, x.TransactionDate.Month })
            .ToDictionary(
                x => (x.Key.Year, x.Key.Month),
                x => x.Sum(t => t.Amount));

        return Enumerable.Range(0, months)
            .Select(offset =>
            {
                var date = start.AddMonths(offset);
                grouped.TryGetValue((date.Year, date.Month), out var total);

                return new SpendingTrendDto(date.Year, date.Month, total);
            })
            .ToList();
    }
}

