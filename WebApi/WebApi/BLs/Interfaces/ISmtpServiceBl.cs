using System.Net.Mail;
using System.Threading.Tasks;

namespace WebApi.BLs.Interfaces
{
    public interface ISmtpServiceBl
    {
        string Address { get; }
        Task SendMailAsync(MailMessage message);
    }
}
