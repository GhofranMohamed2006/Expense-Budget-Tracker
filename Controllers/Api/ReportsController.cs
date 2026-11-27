using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Pennywise.BLL.Abstractions;

namespace Reports_Integration.PL.Controllers.Api;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;
    private readonly ICurrentUserContext _currentUser;

    public ReportsController(
        IReportsService reportsService,
        ICurrentUserContext currentUser)
    {
        _reportsService = reportsService;
        _currentUser = currentUser;
    }

    private IActionResult? CheckUser()
    {
        return string.IsNullOrWhiteSpace(_currentUser.UserId)
            ? Unauthorized(new { message = "Authentication is required." })
            : null;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        CancellationToken cancellationToken)
    {
        var authResult = CheckUser();
        if (authResult is not null) return authResult;

        try
        {
            var result = await _reportsService.GetSummaryAsync(cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { message = ex.Message });
        }
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly(
        [FromQuery, Range(1, 9998)] int year,
        CancellationToken cancellationToken)
    {
        var authResult = CheckUser();
        if (authResult is not null) return authResult;

        try
        {
            var result = await _reportsService.GetMonthlyReportsAsync(
                year, cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { message = ex.Message });
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(
        [FromQuery, Range(1, 9998)] int year,
        [FromQuery, Range(1, 12)] int month,
        CancellationToken cancellationToken)
    {
        var authResult = CheckUser();
        if (authResult is not null) return authResult;

        try
        {
            var result = await _reportsService.GetCategorySpendingAsync(
                year, month, cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { message = ex.Message });
        }
    }

    [HttpGet("trends")]
    public async Task<IActionResult> GetTrends(
        [FromQuery, Range(1, 60)] int months = 6,
        CancellationToken cancellationToken = default)
    {
        var authResult = CheckUser();
        if (authResult is not null) return authResult;

        try
        {
            var result = await _reportsService.GetSpendingTrendsAsync(
                months, cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { message = ex.Message });
        }
    }
}
