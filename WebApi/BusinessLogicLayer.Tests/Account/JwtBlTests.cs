using WebApi.BLs;
using WebApi.BLs.Interfaces;
using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace BusinessLogicLayer.Tests.Account
{
    public class JwtBlTests
    {
        private readonly Mock<IUserRefreshTokenRepository<UserRefreshToken, int>> _mockRefreshRepository;
        private readonly Mock<IAccountBl> _mockAccountBl;
        private readonly Mock<IUserBl> _mockUserBl;
        private readonly Mock<IConfiguration> _mockConfigurations;
        private readonly Mock<ClaimsPrincipal> _mockClaimsPrincipal;
        private readonly Mock<UserRefreshToken> _mockUserRefreshToken;
        private readonly Mock<User> _mockUser;



        public JwtBlTests()
        {
            _mockRefreshRepository = new Mock<IUserRefreshTokenRepository<UserRefreshToken, int>>();
            _mockAccountBl = new Mock<IAccountBl>();
            _mockUserBl = new Mock<IUserBl>();
            _mockConfigurations = new Mock<IConfiguration>();
            _mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            _mockUserRefreshToken = new Mock<UserRefreshToken>();
            _mockUser = new Mock<User>();
        }
        [Fact]
        public async void DeleteRefreshTokenThrowsExceptionWhenClaimsPrincipalIsInvalidAsync()
        {
            WebApi.BLs.JwtTokenBl jwtTokenBl = new WebApi.BLs.JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);

            await Assert.ThrowsAsync<SecurityTokenException>(() => jwtTokenBl.DeleteRefreshTokenAsync(_mockClaimsPrincipal.Object));
            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.DeleteAsync(_mockUserRefreshToken.Object.Id), Times.Never);
        }

        [Fact]
        public async void DeleteRefreshTokenMustDeleteRefreshTokenByUserEmailAsync()
        {
            var mockClaim = new Mock<Claim>("uid", "id");
            _mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.HasClaim(It.IsAny<Predicate<Claim>>())).Returns(true);
            _mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(mockClaim.Object);
            _mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(_mockUserRefreshToken.Object);

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            await jwtService.DeleteRefreshTokenAsync(_mockClaimsPrincipal.Object);

            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.DeleteAsync(_mockUserRefreshToken.Object.Id), Times.Once);
        }

        [Fact]
        public async void LoginByRefreshTokenMustUpdateRefreshTokenWhenItExisitsAsync()
        {
            _mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(_mockUserRefreshToken.Object);

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            await jwtService.LoginByRefreshTokenAsync("id", "token");

            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(_mockUserRefreshToken.Object), Times.Once);
            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.CreateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
        }

        [Fact]
        public async void LoginByRefreshTokenMustCreateRefreshTokenWhenUserNotHaveYetAsync()
        {
            _mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync((UserRefreshToken)null);

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            await jwtService.LoginByRefreshTokenAsync("id", "token");

            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.CreateAsync(It.IsAny<UserRefreshToken>()), Times.Once);
        }

        [Fact]
        public async void UpdateRefreshTokenThrowsExceptionWhenClaimsPrincipalIsInvalidAsync()
        {
            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);

            await Assert.ThrowsAsync<SecurityTokenException>(() => jwtService.UpdateRefreshTokenAsync("token", _mockClaimsPrincipal.Object));
            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
        }
        [Fact]
        public async void GetClaimsMustWriteUserNameInClaims()
        {
            var roles = new List<string> { "somerole" };
            _mockAccountBl.Setup(userService => userService.GetUserRoles(_mockUser.Object)).ReturnsAsync(roles);
            _mockUser.SetupGet(user => user.UserName).Returns("userName");
            _mockUser.SetupGet(user => user.Email).Returns("email");
            _mockUser.SetupGet(user => user.Id).Returns("id");

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            var actualClaims = await jwtService.GetClaimsAsync(_mockUser.Object);
            var existsUserName = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == "userName");

            _mockAccountBl.Verify();
            _mockUser.Verify();
            Assert.True(existsUserName);
        }


        [Fact]
        public async void UpdateRefreshTokenThrowsExceptionWhenRefreshTokensIsNotEqualAsync()
        {
            var mockClaim = new Mock<Claim>("uid", "id");
            _mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.HasClaim(It.IsAny<Predicate<Claim>>())).Returns(true);
            _mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(mockClaim.Object);
            _mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(_mockUserRefreshToken.Object);

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);

            await Assert.ThrowsAsync<SecurityTokenException>(() => jwtService.UpdateRefreshTokenAsync("token", _mockClaimsPrincipal.Object));
            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(It.IsAny<UserRefreshToken>()), Times.Never);
        }

        [Fact]
        public async void UpdateRefreshTokenMustUpdateRefreshTokenAndReturnNewRefreshTokenAsync()
        {
            var userRefreshToken = new UserRefreshToken { RefreshToken = "token" };
            var mockClaim = new Mock<Claim>("uid", "id");
            _mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.HasClaim(It.IsAny<Predicate<Claim>>())).Returns(true);
            _mockClaimsPrincipal.Setup(claimsPrincipal => claimsPrincipal.FindFirst(It.IsAny<Predicate<Claim>>())).Returns(mockClaim.Object);
            _mockRefreshRepository.Setup(refreshRepository => refreshRepository.GetByUserIdAsync(It.IsAny<string>())).ReturnsAsync(userRefreshToken);

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            var actualRefreshToken = await jwtService.UpdateRefreshTokenAsync("token", _mockClaimsPrincipal.Object);

            _mockRefreshRepository.Verify(refreshRepository => refreshRepository.UpdateAsync(userRefreshToken), Times.Once);
        }

        [Fact]
        public async void GetClaimsMustWriteRoleInClaims()
        {
            var roles = new List<string> { "somerole" };
            _mockAccountBl.Setup(userService => userService.GetUserRoles(_mockUser.Object)).ReturnsAsync(roles);
            _mockUser.SetupGet(user => user.UserName).Returns("userName");
            _mockUser.SetupGet(user => user.Email).Returns("email");
            _mockUser.SetupGet(user => user.Id).Returns("id");

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            var actualClaims = await jwtService.GetClaimsAsync(_mockUser.Object);
            var existsRole = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == ClaimTypes.Role && claim.Value == "somerole");

            _mockAccountBl.Verify();
            _mockUser.Verify();
            Assert.True(existsRole);
        }

        [Fact]
        public async void GetClaimsMustGenerateJtiInClaims()
        {
            var roles = new List<string> { "somerole" };
            _mockAccountBl.Setup(userService => userService.GetUserRoles(_mockUser.Object)).ReturnsAsync(roles);
            _mockUser.SetupGet(user => user.UserName).Returns("userName");
            _mockUser.SetupGet(user => user.Email).Returns("email");
            _mockUser.SetupGet(user => user.Id).Returns("id");

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            var actualClaims = await jwtService.GetClaimsAsync(_mockUser.Object);
            var existsJti = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == JwtRegisteredClaimNames.Jti);

            _mockAccountBl.Verify();
            _mockUser.Verify();
            Assert.True(existsJti);
        }

        [Fact]
        public async void GetClaimsMustWriteEmailInClaims()
        {
            var roles = new List<string> { "somerole" };
            _mockAccountBl.Setup(userService => userService.GetUserRoles(_mockUser.Object)).ReturnsAsync(roles);
            _mockUser.SetupGet(user => user.UserName).Returns("userName");
            _mockUser.SetupGet(user => user.Email).Returns("email");
            _mockUser.SetupGet(user => user.Id).Returns("id");

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            var actualClaims = await jwtService.GetClaimsAsync(_mockUser.Object);
            var expectedClaim = new Claim(JwtRegisteredClaimNames.Email, "email");
            var existsEmail = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == "email");

            _mockAccountBl.Verify();
            _mockUser.Verify();
            Assert.True(existsEmail);
        }

        [Fact]
        public async void GetClaimsMustWriteIdInClaims()
        {
            var roles = new List<string> { "somerole" };
            _mockAccountBl.Setup(userService => userService.GetUserRoles(_mockUser.Object)).ReturnsAsync(roles);
            _mockUser.SetupGet(user => user.UserName).Returns("userName");
            _mockUser.SetupGet(user => user.Email).Returns("email");
            _mockUser.SetupGet(user => user.Id).Returns("id");

            JwtTokenBl jwtService = new JwtTokenBl(_mockRefreshRepository.Object, _mockUserBl.Object, _mockAccountBl.Object, _mockConfigurations.Object);
            var actualClaims = await jwtService.GetClaimsAsync(_mockUser.Object);
            var expectedClaim = new Claim("uid", "id");
            var existsId = new List<Claim>(actualClaims)
                .Exists(claim => claim.Type == "uid" && claim.Value == "id");

            _mockAccountBl.Verify();
            _mockUser.Verify();
            Assert.True(existsId);
        }
    }
}
