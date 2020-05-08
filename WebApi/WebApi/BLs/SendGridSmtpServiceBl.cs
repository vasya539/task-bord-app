using System.Threading.Tasks;
using System.Net.Mail;
using WebApi.BLs.Interfaces;

namespace WebApi.BLs
{
    public class SendGridSmtpServiceBl : ISmtpServiceBl
    {
        private readonly SmtpClient client;

        public SendGridSmtpServiceBl()
        {
            //Generate your own or use this one:
            var pass = "SG.8XnCm_CHQpmIwz8QAqyHIA.X6RrGQqgqSlfm6e_TT3ew0HCs2bpIv3r6zrgJpR2eI8";

            client = new SmtpClient("smtp.sendgrid.net", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false
            };
            client.Credentials = new System.Net.NetworkCredential(Address, pass)
            { 
                Password = pass,
                UserName = "apikey"
            };
        }
        
        public string Address => "if106.netgroup@gmail.com";

        public Task SendMailAsync(MailMessage message)
        {
            return client.SendMailAsync(message);
        }

    }
}
