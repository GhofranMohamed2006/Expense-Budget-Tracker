using System.Security.Claims;
using Pennywise.Common.Exceptions;

namespace Pennywise.Presentation.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Current user's id from the JWT. Never accept UserId from the client.</summary>
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedException("Invalid token.");
}
