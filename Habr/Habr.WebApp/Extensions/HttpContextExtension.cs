using Habr.Common.Enums;
using System.Security.Claims;

namespace Habr.WebApp.Extensions
{
    public static class HttpContextExtension
    {
        public static int GetUserId(this HttpContext context)
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(x => x.Type == "jti");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return 0;
        }

        public static UserRole GetUserRole(this HttpContext context)
        {
            var userRoleClaim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            var result = Enum.TryParse(userRoleClaim?.Value, out UserRole value);

            return value;
        }
    }
}
