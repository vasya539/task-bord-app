using System.Threading.Tasks;
using WebApi.BLs.Interfaces;
using Microsoft.Extensions.Configuration;
using WebApi.Data.Models;

namespace WebApi.BLs
{
    public class NotificationBl : INotificationBl
    {
        private readonly IMessageBl _messageBl;
        public IConfiguration Configuration { get; }

        public NotificationBl(IMessageBl messageBl, IConfiguration configuration)
        {
            _messageBl = messageBl;
            Configuration = configuration;
        }

        public async Task SendPasswordResetNotification(User user, string passResetToken)
        {
            await _messageBl.SendAsync(new Message()
            {
                Destination = user.Email,
                Subject = "BoardApp New Password",
                Body = $"<h1>Hello {user.UserName}</h1>" +
                $"<p>Please proceed by the following " +
                $"<a href=\"http://{Configuration["HostingDomain"]}/reset?userId={user.Id}&code={passResetToken}\">link</a> " +
                $"to set your new password.</p>" +
                "<p>Best Regrads!</p>"
            });
        }
    }
}
