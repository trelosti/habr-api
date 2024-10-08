using Habr.DataAccess.Entities;
using System.Security.Claims;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IJwtService
    {
        Tuple<string, DateTime> GenerateAccessToken(User user);
        Tuple<string, DateTime> GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
