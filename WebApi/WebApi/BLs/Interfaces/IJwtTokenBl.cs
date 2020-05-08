using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;
using System.Security.Claims;

namespace WebApi.BLs.Interfaces
{
    public interface IJwtTokenBl
    {
        DateTime ExpirationTime { get; }
        string GenerateJwtAccessToken(IEnumerable<Claim> claims);
        string GenerateJwtRefreshToken();
        Task<Claim[]> GetClaimsAsync(User user);
        ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string accessToken);
        Task LoginByRefreshTokenAsync(string userId, string refreshToken);
        Task<string> UpdateRefreshTokenAsync(string refreshToken, ClaimsPrincipal userPrincipal);
        Task DeleteRefreshTokenAsync(ClaimsPrincipal userPrincipal);
    }
}
