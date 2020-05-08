using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.BLs.Interfaces
{
    public interface IMessageBl
    {
        Task SendAsync(Message message);
    }
}
