using System.Threading.Tasks;
using WebApi.BLs.Interfaces;
using System.Net.Mail;
using WebApi.Data.Models;

namespace WebApi.BLs
{
    public class MessageBl : IMessageBl
    {
        private readonly ISmtpServiceBl _smtpService;

        public MessageBl(ISmtpServiceBl smtpService)
        {
            _smtpService = smtpService;
        }

        public Task SendAsync(Message message)
        {
            var mail = new MailMessage(_smtpService.Address, message.Destination)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            return _smtpService.SendMailAsync(mail);
        }
    }
}
