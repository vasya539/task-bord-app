using WebApi.Data.Models;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;
using WebApi.BLs.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.BLs
{
    public class JwtTokenBl : IJwtTokenBl
    {
        private readonly IUserRefreshTokenRepository<UserRefreshToken,int> refreshRepository;
        private readonly IUserBl userBl;
        private readonly IAccountBl accountBl;
        private readonly IConfiguration configuration;

        public JwtTokenBl(IUserRefreshTokenRepository<UserRefreshToken, int> refreshRepository, IUserBl userBl, IAccountBl accountBl, IConfiguration configuration)
        {
            this.refreshRepository = refreshRepository;
            this.userBl = userBl;
            this.accountBl = accountBl;
            this.configuration = configuration;
        }

        public DateTime ExpirationTime => DateTime.Now.AddMinutes(120);

        public async Task DeleteRefreshTokenAsync(ClaimsPrincipal userPrincipal)
        {
            if (!userPrincipal.HasClaim(claim => claim.Type == "uid"))
                throw new SecurityTokenException("Refresh token deletion failed: access token has no user id.");

            var userID = userPrincipal.FindFirst(claim => claim.Type == "uid").Value;
            var refreshToken = await refreshRepository.GetByUserIdAsync(userID);

            if (refreshToken == null)
                throw new SecurityTokenException("Refresh token deletion failed: cannot retrieve refresh token.");

            await refreshRepository.DeleteAsync(refreshToken.Id);
        }

        public string GenerateJwtAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: configuration["JwtIssuer"],
              audience: configuration["JwtAudience"],
              claims: claims,
              expires: ExpirationTime,
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJwtRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<Claim[]> GetClaimsAsync(User userInfo)
        {
            var roles = await accountBl.GetUserRoles(userInfo);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", userInfo.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims.ToArray();
        }

        public ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken))
                throw new SecurityTokenException("Retrieving principal from access token failed: access token validation failed.");

            if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Retrieving principal from access token failed: access token's algorithm is not correct.");

            return principal;
        }

        public async Task LoginByRefreshTokenAsync(string userId, string refreshToken)
        {
            var userRefreshToken = await refreshRepository.GetByUserIdAsync(userId);
            if (userRefreshToken != null)
            {
                userRefreshToken.RefreshToken = refreshToken;
                userRefreshToken.ExpireOn = DateTime.Now.AddMonths(3);
                await refreshRepository.UpdateAsync(userRefreshToken);
            }
            else
            {
                userRefreshToken = new UserRefreshToken
                {
                    UserId = userId,
                    RefreshToken = refreshToken,
                    ExpireOn = DateTime.Now.AddMonths(3)
                };
                await refreshRepository.CreateAsync(userRefreshToken);
            }
        }

        public async Task<string> UpdateRefreshTokenAsync(string oldRefreshTokenPlain, ClaimsPrincipal userPrincipal)
        {
            if (!userPrincipal.HasClaim(claim => claim.Type == "uid"))
                throw new SecurityTokenException("Refresh token update failed: access token has no user id.");

            string userId = userPrincipal.FindFirst(claim => claim.Type == "uid").Value;
            UserRefreshToken savedRefreshToken = await refreshRepository.GetByUserIdAsync(userId);

            if (oldRefreshTokenPlain != savedRefreshToken?.RefreshToken)
                throw new SecurityTokenException("Refresh token update failed: plain refresh tokens don't match.");

            var newRefreshToken = GenerateJwtRefreshToken();

            savedRefreshToken.RefreshToken = newRefreshToken;
            savedRefreshToken.ExpireOn = DateTime.Now.AddMonths(3);
            await refreshRepository.UpdateAsync(savedRefreshToken);
            return newRefreshToken;
        }
    }
}
