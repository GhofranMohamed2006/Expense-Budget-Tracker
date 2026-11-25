using Pennywise.DAL.Entities;

namespace Pennywise.BLL.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}
