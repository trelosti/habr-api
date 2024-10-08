using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.Auth;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Habr.BusinessLogic.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfigurationDTO _jwtConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(IOptions<JwtConfigurationDTO> jwtConfiguration, IHttpContextAccessor httpContextAccessor) 
        { 
            _jwtConfiguration = jwtConfiguration.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public Tuple<string, DateTime> GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));

            var expires = DateTime.UtcNow.AddSeconds(int.Parse(_jwtConfiguration.AccessExpiresInSeconds));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtConfiguration.Issuer,
                Audience = _jwtConfiguration.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new Tuple<string, DateTime>(tokenHandler.WriteToken(token), expires);
        }

        public Tuple<string, DateTime> GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var expires = DateTime.UtcNow.AddDays(int.Parse(_jwtConfiguration.RefreshExpiresInDays));

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                var token =  Convert.ToBase64String(randomNumber);

                return new Tuple<string, DateTime>(token, expires);
            }
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtConfiguration.Issuer,
                ValidAudience = _jwtConfiguration.Audience,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new AuthenticationException();

            return principal;
        }
    }
}
