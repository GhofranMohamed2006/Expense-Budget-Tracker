using Pennywise.BLL.DTOs.Auth;

namespace Pennywise.BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<UserProfileDto> GetProfileAsync(string userId);
}
