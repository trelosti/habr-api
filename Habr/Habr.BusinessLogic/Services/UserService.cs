using Habr.Common.Helpers.Validation;
using Habr.Common.DTO.Auth;
using Habr.Common.DTO.Result;
using Habr.BusinessLogic.Interfaces;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Habr.Common.Resources;
using Serilog;
using Habr.Common.DTO.Users;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Options;

namespace Habr.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly JwtConfigurationDTO _jwtConfiguration;

        public UserService(
            DataContext dataContext, 
            IJwtService jwtService, 
            IMapper mapper, 
            IOptions<JwtConfigurationDTO> jwtConfiguration)
        {
            _dataContext = dataContext;
            _jwtService = jwtService;
            _mapper = mapper;
            _jwtConfiguration = jwtConfiguration.Value;
        }

        public async Task<GenericResult<bool>> RegisterAsync(RegisterDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Email) ||
                string.IsNullOrEmpty(dto.Password) ||
                !UserValidationHelper.IsEmailValid(dto.Email))
            {
                return new GenericResult<bool>
                {
                    ErrorMessage = CommonMessageResource.InvalidInput
                };
            }

            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Email == dto.Email);

            if (user != null)
            {
                return new GenericResult<bool>()
                {
                    ErrorMessage = UserMessageResource.EmailTaken,
                };
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                Name = UserValidationHelper.ValidateUserName(dto.Name),
                Email = dto.Email,
                Password = hash,
                Created = DateTime.UtcNow
            };

            _dataContext.Users.Add(newUser);

            await _dataContext.SaveChangesAsync();

            Log.Information(string.Format(LogMessageResource.SuccessfulRegistration, dto.Email, DateTime.UtcNow));

            return new GenericResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        public async Task<GenericResult<AuthTokenDTO>> LoginAsync(LoginDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Email) ||
            string.IsNullOrEmpty(dto.Password) ||
            !UserValidationHelper.IsEmailValid(dto.Email))
            {
                return new GenericResult<AuthTokenDTO>
                {
                    ErrorMessage = CommonMessageResource.InvalidInput
                };
            }

            var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
            {
                return new GenericResult<AuthTokenDTO>()
                {
                    ErrorMessage = UserMessageResource.EmailIncorrect,
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return new GenericResult<AuthTokenDTO>()
                {
                    ErrorMessage = UserMessageResource.PasswordIncorrect,
                };
            }

            Log.Information(string.Format(LogMessageResource.SuccessfulLogin, dto.Email, DateTime.UtcNow));

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken.Item1;
            user.RefreshTokenExpiryTime = refreshToken.Item2;

            await _dataContext.SaveChangesAsync();

            return new GenericResult<AuthTokenDTO>
            {
                Success = true,
                Data = new AuthTokenDTO
                {
                    AccessToken = accessToken.Item1,
                    RefreshToken = refreshToken.Item1,
                    AccessTokenExpiration = accessToken.Item2,
                    RefreshTokenExpiration = refreshToken.Item2
                }
            };
        }

        public async Task<GenericResult<UserInfoDTO>> GetUserInfo(int userId)
        {
            var user = await _dataContext.Users
                .Where(x => x.Id == userId)
                .ProjectTo<UserInfoDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return new GenericResult<UserInfoDTO>
                {
                    ErrorMessage = UserMessageResource.UserNotFound
                };
            }

            return new GenericResult<UserInfoDTO>
            {
                Success = true,
                Data = user
            };
        }

        public async Task<GenericResult<AuthTokenDTO>> RefreshToken(RefreshTokenDTO dto)
        {
            var principal = _jwtService.GetPrincipalFromToken(dto.AccessToken);

            var jti = principal.Claims.First(x => x.Type == "jti").Value;

            var user = await _dataContext.Users
                .Where(x => x.Id.ToString() == jti)
                .SingleOrDefaultAsync();

            if (user?.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new GenericResult<AuthTokenDTO>
                {
                    ErrorMessage = CommonMessageResource.InvalidToken
                };
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken.Item1;
            user.RefreshTokenExpiryTime = newRefreshToken.Item2;

            await _dataContext.SaveChangesAsync();

            return new GenericResult<AuthTokenDTO>
            {
                Success = true,
                Data = new AuthTokenDTO
                {
                    AccessToken = newAccessToken.Item1,
                    RefreshToken = newRefreshToken.Item1,
                    AccessTokenExpiration = newAccessToken.Item2,
                    RefreshTokenExpiration = newRefreshToken.Item2
                }
            };
        }
    }
}
