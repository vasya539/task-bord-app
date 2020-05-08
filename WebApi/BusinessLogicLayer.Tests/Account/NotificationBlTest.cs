using System;
using System.Collections.Generic;
using System.Text;
using Xunit;


using Microsoft.Extensions.Configuration;
using Moq;
using WebApi.BLs.Interfaces;
using WebApi.Data.Models;
using WebApi.BLs;

namespace BusinessLogicLayer.Tests.Account
{
    public class NotificationBlTest
    {
        private readonly Mock<IConfiguration> _mockConfigs;
        private readonly Mock<IMessageBl> _mockMessageBl;
        private readonly Mock<IAccountBl> _mockAccountBl;
        private readonly Mock<User> _mockUser;

        public NotificationBlTest()
        {
            _mockMessageBl = new Mock<IMessageBl>();
            _mockAccountBl = new Mock<IAccountBl>();
            _mockUser = new Mock<User>();
            _mockConfigs = new Mock<IConfiguration>();
        }


        [Theory]
        [InlineData("token1")]
        [InlineData("token2")]
        [InlineData("token3")]
        public async void ForgetPasswordMailMustSendForgetMessageForUser(string resetToken)
        {
            var body = $"Click to <a href=\"https://somesite.com?userId={_mockUser.Object.Id}&code={resetToken}\">link</a>, if you want restore your password";
            var message = new Message
            {
                Body = body,
                Destination = _mockUser.Object.Email,
                Subject = "Forget Password"
            };
            _mockAccountBl.Setup(accountBl => accountBl.GeneratePasswordResetTokenAsync(_mockUser.Object)).ReturnsAsync(resetToken);


            NotificationBl notificationService = new NotificationBl(_mockMessageBl.Object, _mockConfigs.Object);
            await notificationService.SendPasswordResetNotification(_mockUser.Object, "token");

            _mockMessageBl.Verify(messageService => messageService.SendAsync(It.IsAny<Message>()), Times.Once);
        }
    }
}
