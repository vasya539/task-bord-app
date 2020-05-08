using WebApi.BLs.Interfaces;
using WebApi.BLs;
using WebApi.Data.Models;
using Moq;
using System.Net.Mail;
using Xunit;

namespace BusinessLogicLayer.Tests.Account
{
    public class MassageBlTests
    {
        private readonly Mock<ISmtpServiceBl> _mockSmtpService;
        private readonly Mock<Message> mockMessage;

        public MassageBlTests()
        {
            _mockSmtpService = new Mock<ISmtpServiceBl>();
            mockMessage = new Mock<Message>();
        }

        [Fact]
        public async void MustSendMailAsync()
        {
            _mockSmtpService.SetupGet(smtpService => smtpService.Address).Returns("from@mail.com");
            mockMessage.SetupGet(message => message.Subject).Returns("someSubject");
            mockMessage.SetupGet(message => message.Body).Returns("someBody");
            mockMessage.SetupGet(message => message.Destination).Returns("to@mail.com");

            MessageBl messageBl = new MessageBl(_mockSmtpService.Object);
            await messageBl.SendAsync(mockMessage.Object);

            _mockSmtpService.Verify(smtpService => smtpService.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);
        }
    }
}
