using Microsoft.AspNetCore.Identity;
using Pennywise.BLL.DTOs.Auth;
using Pennywise.BLL.Interfaces;
using Pennywise.Common.Exceptions;
using Pennywise.DAL.Entities;

namespace Pennywise.BLL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwt;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenService jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var email = dto.Email.Trim();

        if (await _userManager.FindByEmailAsync(email) is not null)
            throw new ConflictException("Email is already registered.");

        var user = new ApplicationUser
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            UserName = email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(e => e.Code.StartsWith("Password") ? "password" : "email")
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            throw new ValidationFailedException(errors);
        }

        return ToResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email.Trim());

        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedException("Invalid email or password.");

        return ToResponse(user);
    }

    public async Task<UserProfileDto> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException("User not found.");

        return new UserProfileDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            CreatedAt = user.CreatedAt
        };
    }

    private AuthResponseDto ToResponse(ApplicationUser user) => new()
    {
        Token = _jwt.GenerateToken(user),
        UserId = user.Id,
        FullName = user.FullName,
        Email = user.Email ?? string.Empty
    };
}
