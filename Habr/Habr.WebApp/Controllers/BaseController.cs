using Habr.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Habr.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int CurrentUserId
        {
            get { return int.Parse(this.User.Claims.First(x => x.Type == "jti").Value); }
            set { }
        }

        protected UserRole CurrentUserRole
        {
            get
            {
                var result = Enum.TryParse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value, out UserRole value);
                return value;
            }
            set { }
        }
    }
}
