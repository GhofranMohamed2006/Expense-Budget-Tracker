using Pennywise.BLL.Abstractions;
using Pennywise.BLL.Services;
using Pennywise.Common.Models;
using Pennywise.DAL.Abstractions;
using Xunit;

namespace Reports_Integration.Tests;

public sealed class ReportsServiceTests
{
    private const string UserId = "user-1";

    private static ReportsService CreateService(
        IEnumerable<ReportTransaction> transactions,
        string? userId = UserId)
    {
        return new ReportsService(
            new FakeTransactionRepository(transactions),
            new FakeCurrentUserContext(userId));
    }

    private static ReportTransaction Transaction(
        decimal amount,
        TransactionType type,
        DateTime date,
        string userId = UserId,
        Guid? categoryId = null,
        string? categoryName = null)
    {
        return new ReportTransaction(
            Guid.NewGuid(),
            userId,
            amount,
            type,
            date,
            categoryId,
            categoryName);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldCalculateIncomeExpensesAndSavings()
    {
        var transactions = new[]
        {
            Transaction(5000m, TransactionType.Income, new DateTime(2026, 11, 1)),
            Transaction(1500m, TransactionType.Expense, new DateTime(2026, 11, 2)),
            Transaction(500m, TransactionType.Transfer, new DateTime(2026, 11, 3))
        };

        var service = CreateService(transactions);

        var result = await service.GetSummaryAsync();

        Assert.Equal(3500m, result.TotalBalance);
        Assert.Equal(5000m, result.TotalIncome);
        Assert.Equal(1500m, result.TotalExpenses);
        Assert.Equal(3500m, result.NetSavings);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldExcludeOtherUsersTransactions()
    {
        var transactions = new[]
        {
            Transaction(1000m, TransactionType.Income, DateTime.UtcNow),
            Transaction(9000m, TransactionType.Income, DateTime.UtcNow, "user-2")
        };

        var service = CreateService(transactions);

        var result = await service.GetSummaryAsync();

        Assert.Equal(1000m, result.TotalIncome);
        Assert.Equal(1000m, result.TotalBalance);
    }

    [Fact]
    public async Task GetMonthlyReportsAsync_ShouldGroupTransactionsByMonth()
    {
        var transactions = new[]
        {
            Transaction(3000m, TransactionType.Income, new DateTime(2026, 1, 5)),
            Transaction(1000m, TransactionType.Expense, new DateTime(2026, 1, 10)),
            Transaction(2000m, TransactionType.Income, new DateTime(2026, 2, 5)),
            Transaction(500m, TransactionType.Expense, new DateTime(2026, 2, 10)),
            Transaction(700m, TransactionType.Transfer, new DateTime(2026, 2, 15))
        };

        var service = CreateService(transactions);

        var result = await service.GetMonthlyReportsAsync(2026);

        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Month);
        Assert.Equal(3000m, result[0].TotalIncome);
        Assert.Equal(1000m, result[0].TotalExpenses);
        Assert.Equal(2000m, result[0].NetSavings);

        Assert.Equal(2, result[1].Month);
        Assert.Equal(2000m, result[1].TotalIncome);
        Assert.Equal(500m, result[1].TotalExpenses);
        Assert.Equal(1500m, result[1].NetSavings);
    }

    [Fact]
    public async Task GetCategorySpendingAsync_ShouldCalculatePercentages()
    {
        var transactions = new[]
        {
            Transaction(30m, TransactionType.Expense, new DateTime(2026, 11, 5),
                categoryId: Guid.NewGuid(), categoryName: "Food"),

            Transaction(70m, TransactionType.Expense, new DateTime(2026, 11, 6),
                categoryId: Guid.NewGuid(), categoryName: "Transport"),

            Transaction(100m, TransactionType.Income, new DateTime(2026, 11, 7)),

            Transaction(50m, TransactionType.Transfer, new DateTime(2026, 11, 8))
        };

        var service = CreateService(transactions);

        var result = await service.GetCategorySpendingAsync(2026, 11);

        Assert.Equal(2, result.Count);

        var transport = result.Single(x => x.CategoryName == "Transport");
        var food = result.Single(x => x.CategoryName == "Food");

        Assert.Equal(70m, transport.TotalAmount);
        Assert.Equal(70m, transport.Percentage);

        Assert.Equal(30m, food.TotalAmount);
        Assert.Equal(30m, food.Percentage);
    }

    [Fact]
    public async Task GetSpendingTrendsAsync_ShouldReturnChronologicalMonths()
    {
        var currentMonth = new DateTime(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month,
            1);

        var previousMonth = currentMonth.AddMonths(-1);
        var service = CreateService(new[]
        {
            Transaction(100m, TransactionType.Expense, previousMonth.AddDays(5)),
            Transaction(200m, TransactionType.Expense, currentMonth.AddDays(5))
        });

        var result = await service.GetSpendingTrendsAsync(3);

        Assert.Equal(3, result.Count);

        Assert.Equal(previousMonth.AddMonths(-1).Month, result[0].Month);
        Assert.Equal(previousMonth.Month, result[1].Month);
        Assert.Equal(currentMonth.Month, result[2].Month);

        Assert.Equal(0m, result[0].TotalExpenses);
        Assert.Equal(100m, result[1].TotalExpenses);
        Assert.Equal(200m, result[2].TotalExpenses);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldHandleEmptyDataset()
    {
        var service = CreateService(Array.Empty<ReportTransaction>());

        var result = await service.GetSummaryAsync();

        Assert.Equal(0m, result.TotalBalance);
        Assert.Equal(0m, result.TotalIncome);
        Assert.Equal(0m, result.TotalExpenses);
        Assert.Equal(0m, result.NetSavings);
    }

    [Fact]
    public async Task GetCategorySpendingAsync_ShouldReturnEmptyForNoExpenses()
    {
        var transactions = new[]
        {
            Transaction(1000m, TransactionType.Income, new DateTime(2026, 11, 5))
        };

        var service = CreateService(transactions);

        var result = await service.GetCategorySpendingAsync(2026, 11);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldRequireAuthenticatedUser()
    {
        var service = CreateService(
            Array.Empty<ReportTransaction>(),
            userId: null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetSummaryAsync());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    public async Task GetMonthlyReportsAsync_ShouldRejectInvalidYear(int year)
    {
        var service = CreateService(Array.Empty<ReportTransaction>());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetMonthlyReportsAsync(year));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public async Task GetCategorySpendingAsync_ShouldRejectInvalidMonth(int month)
    {
        var service = CreateService(Array.Empty<ReportTransaction>());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetCategorySpendingAsync(2026, month));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(61)]
    public async Task GetSpendingTrendsAsync_ShouldRejectInvalidMonths(int months)
    {
        var service = CreateService(Array.Empty<ReportTransaction>());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetSpendingTrendsAsync(months));
    }

    private sealed class FakeCurrentUserContext : ICurrentUserContext
    {
        public string? UserId { get; }

        public FakeCurrentUserContext(string? userId)
        {
            UserId = userId;
        }
    }

    private sealed class FakeTransactionRepository
        : ITransactionReportRepository
    {
        private readonly IReadOnlyList<ReportTransaction> _transactions;

        public FakeTransactionRepository(
            IEnumerable<ReportTransaction> transactions)
        {
            _transactions = transactions.ToList();
        }

        public Task<IReadOnlyList<ReportTransaction>> GetTransactionsAsync(
            string userId,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<ReportTransaction> result = _transactions
                .Where(x => from == null || x.TransactionDate >= from.Value)
                .Where(x => to == null || x.TransactionDate <= to.Value)
                .ToList();

            return Task.FromResult(result);
        }
    }
}
