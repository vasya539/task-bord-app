using WebApi.Data.Models;
using System.Threading.Tasks;

namespace WebApi.BLs.Interfaces
{
    public interface INotificationBl
    {
        Task SendPasswordResetNotification(User user, string passResetToken);
    }
}
