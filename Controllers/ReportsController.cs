using Microsoft.AspNetCore.Mvc;
using Pennywise.BLL.Abstractions;
using Pennywise.Common.Abstractions;

namespace Reports_Integration.PL.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportsService _service;
    private readonly ICurrentUserContext _user;

    public ReportsController(
        IReportsService service,
        ICurrentUserContext user)
    {
        _service = service;
        _user = user;
    }

    private IActionResult? CheckUser()
    {
        return string.IsNullOrWhiteSpace(_user.UserId)
            ? Unauthorized(new { message = "Authentication is required." })
            : null;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var denied = CheckUser();
        if (denied is not null) return denied;

        return Ok(await _service.GetSummaryAsync(cancellationToken));
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> Monthly(
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var denied = CheckUser();
        if (denied is not null) return denied;

        if (year < 2000 || year > 9999)
            return BadRequest(new { message = "Year must be between 2000 and 9999." });

        return Ok(await _service.GetMonthlyAsync(year, cancellationToken));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> Categories(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var denied = CheckUser();
        if (denied is not null) return denied;

        if (year < 2000 || year > 9999 || month < 1 || month > 12)
            return BadRequest(new { message = "A valid year and month are required." });

        return Ok(await _service.GetCategoriesAsync(year, month, cancellationToken));
    }

    [HttpGet("trends")]
    public async Task<IActionResult> Trends(
        [FromQuery] int months = 6,
        CancellationToken cancellationToken = default)
    {
        var denied = CheckUser();
        if (denied is not null) return denied;

        if (months < 1 || months > 60)
            return BadRequest(new { message = "Months must be between 1 and 60." });

        return Ok(await _service.GetTrendsAsync(months, cancellationToken));
    }
}
