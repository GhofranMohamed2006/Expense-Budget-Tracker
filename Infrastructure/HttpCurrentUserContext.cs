using System.Security.Claims;
using Pennywise.Common.Abstractions;

namespace Reports_Integration.PL.Infrastructure;

public sealed class HttpCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public HttpCurrentUserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string? UserId =>
        _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _accessor.HttpContext?.User?.FindFirstValue("sub");
}
