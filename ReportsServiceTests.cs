using Pennywise.BLL.Services;
using Pennywise.Common.Abstractions;
using Pennywise.Common.Models;
using Xunit;

namespace Reports_Integration.Tests;

public sealed class ReportsServiceTests
{
    private sealed class FakeUser : ICurrentUserContext
    {
        public string? UserId { get; set; } = "user-1";
    }

    private sealed class FakeRepository : ITransactionReportRepository
    {
        public List<ReportTransaction> Data { get; set; } = [];

        public Task<IReadOnlyList<ReportTransaction>> GetTransactionsAsync(
            string userId,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<ReportTransaction> result = Data
                .Where(x => x.UserId == userId)
                .Where(x => from == null || x.Date >= from)
                .Where(x => to == null || x.Date < to)
                .ToList();

            return Task.FromResult(result);
        }
    }

    private static ReportTransaction Transaction(
        string user,
        decimal amount,
        TransactionType type,
        string category = "Food",
        int month = 1)
    {
        return new ReportTransaction
        {
            UserId = user,
            Amount = amount,
            Type = type,
            Category = category,
            Date = new DateTime(2026, month, 10)
        };
    }

    [Fact]
    public async Task Summary_Calculates_Income_Expenses_And_Balance()
    {
        var repo = new FakeRepository
        {
            Data =
            [
                Transaction("user-1", 1000, TransactionType.Income),
                Transaction("user-1", 300, TransactionType.Expense)
            ]
        };

        var service = new ReportsService(repo, new FakeUser());

        var result = await service.GetSummaryAsync();

        Assert.Equal(1000, result.TotalIncome);
        Assert.Equal(300, result.TotalExpenses);
        Assert.Equal(700, result.TotalBalance);
        Assert.Equal(700, result.NetSavings);
    }

    [Fact]
    public async Task Summary_Excludes_Transfers_From_Income_And_Expenses()
    {
        var repo = new FakeRepository
        {
            Data =
            [
                Transaction("user-1", 1000, TransactionType.Income),
                Transaction("user-1", 200, TransactionType.Expense),
                Transaction("user-1", 500, TransactionType.Transfer)
            ]
        };

        var result = await new ReportsService(repo, new FakeUser())
            .GetSummaryAsync();

        Assert.Equal(1000, result.TotalIncome);
        Assert.Equal(200, result.TotalExpenses);
        Assert.Equal(3, result.TransactionCount);
        Assert.Equal(800, result.TotalBalance);
    }

    [Fact]
    public async Task Summary_Isolates_User_Data()
    {
        var repo = new FakeRepository
        {
            Data =
            [
                Transaction("user-1", 100, TransactionType.Income),
                Transaction("user-2", 9000, TransactionType.Income)
            ]
        };

        var result = await new ReportsService(repo, new FakeUser())
            .GetSummaryAsync();

        Assert.Equal(100, result.TotalIncome);
    }

    [Fact]
    public async Task Categories_Calculates_Percentages()
    {
        var repo = new FakeRepository
        {
            Data =
            [
                Transaction("user-1", 300, TransactionType.Expense, "Food"),
                Transaction("user-1", 100, TransactionType.Expense, "Transport"),
                Transaction("user-1", 500, TransactionType.Income)
            ]
        };

        var result = await new ReportsService(repo, new FakeUser())
            .GetCategoriesAsync(2026, 1);

        Assert.Equal(2, result.Count);
        Assert.Equal(75, result[0].Percentage);
        Assert.Equal(25, result[1].Percentage);
    }

    [Fact]
    public async Task Monthly_Groups_Transactions_By_Month()
    {
        var repo = new FakeRepository
        {
            Data =
            [
                Transaction("user-1", 100, TransactionType.Income, month: 1),
                Transaction("user-1", 50, TransactionType.Expense, month: 2)
            ]
        };

        var result = await new ReportsService(repo, new FakeUser())
            .GetMonthlyAsync(2026);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Month);
        Assert.Equal(2, result[1].Month);
    }

    [Fact]
    public async Task Summary_Handles_Empty_Dataset()
    {
        var service = new ReportsService(new FakeRepository(), new FakeUser());

        var result = await service.GetSummaryAsync();

        Assert.Equal(0, result.TotalIncome);
        Assert.Equal(0, result.TotalExpenses);
        Assert.Equal(0, result.TotalBalance);
    }

    [Fact]
    public async Task Categories_Rejects_Invalid_Month()
    {
        var service = new ReportsService(new FakeRepository(), new FakeUser());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetCategoriesAsync(2026, 13));
    }

    [Fact]
    public async Task Trends_Returns_Requested_Number_Of_Months()
    {
        var service = new ReportsService(new FakeRepository(), new FakeUser());

        var result = await service.GetTrendsAsync(6);

        Assert.Equal(6, result.Count);
    }

    [Fact]
    public async Task Summary_Rejects_Missing_User()
    {
        var user = new FakeUser { UserId = null };
        var service = new ReportsService(new FakeRepository(), user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.GetSummaryAsync());
    }

    [Fact]
    public async Task Repository_Receives_Cancellation()
    {
        var service = new ReportsService(
            new FakeRepository(),
            new FakeUser());

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.GetSummaryAsync(cts.Token));
    }
}
