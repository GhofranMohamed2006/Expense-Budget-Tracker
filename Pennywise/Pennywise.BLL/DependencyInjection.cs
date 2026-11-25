using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pennywise.BLL.Interfaces;
using Pennywise.BLL.Services;
using Pennywise.BLL.Settings;
using Pennywise.DAL;

namespace Pennywise.BLL;

public static class DependencyInjection
{
    /// <summary>Presentation only references BLL, so BLL wires up the DAL as well.</summary>
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection.");
        services.AddDataAccess(cs);

        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Other members register their services here:
        // services.AddScoped<IAccountService, AccountService>(); ...

        return services;
    }
}
