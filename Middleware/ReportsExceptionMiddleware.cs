using Microsoft.AspNetCore.Mvc;
using Pennywise.DAL.Integration;

namespace Reports_Integration.PL.Middleware;

public sealed class ReportsExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ReportsExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ReportsIntegrationUnavailableException ex)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = 503,
                    Title = "Reports integration unavailable",
                    Detail = ex.Message
                });
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499;
        }
    }
}
