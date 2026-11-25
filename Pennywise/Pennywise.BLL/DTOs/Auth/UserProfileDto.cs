namespace Pennywise.BLL.DTOs.Auth;

/// <summary>Returned by GET /api/auth/me (added to the contract: no token here).</summary>
public class UserProfileDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
