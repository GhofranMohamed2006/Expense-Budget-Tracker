using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Pennywise.Common.Exceptions;

namespace Pennywise.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted) { _logger.LogError(ex, "Error after response started"); return; }

            ProblemDetails problem;
            switch (ex)
            {
                case ValidationFailedException v:
                    problem = new ValidationProblemDetails(v.Errors) { Status = v.StatusCode, Title = "Validation failed" };
                    break;
                case AppException app:
                    problem = new ProblemDetails { Status = app.StatusCode, Title = app.GetType().Name.Replace("Exception", ""), Detail = app.Message };
                    break;
                default:
                    _logger.LogError(ex, "Unhandled exception");
                    problem = new ProblemDetails { Status = 500, Title = "Server error", Detail = "An unexpected error occurred." };
                    break;
            }

            context.Response.StatusCode = problem.Status!.Value;
            await context.Response.WriteAsJsonAsync(problem, problem.GetType(), (JsonSerializerOptions?)null, "application/problem+json");
        }
    }
}
