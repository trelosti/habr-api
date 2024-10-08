using Habr.Common.DTO.Auth;
using Habr.Common.DTO.Result;
using Habr.Common.DTO.Users;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<GenericResult<bool>> RegisterAsync(RegisterDTO dto);
        Task<GenericResult<AuthTokenDTO>> LoginAsync(LoginDTO dto);
        Task<GenericResult<UserInfoDTO>> GetUserInfo(int userId);
        Task<GenericResult<AuthTokenDTO>> RefreshToken(RefreshTokenDTO dto);
    }
}
