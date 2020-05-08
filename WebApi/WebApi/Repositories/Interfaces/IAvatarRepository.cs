using System.IO;
using System.Threading.Tasks;

namespace WebApi.Repositories.Interfaces
{
	public interface IAvatarRepository
	{
		/// <summary>
		/// Returns Stream for avatar of specific user
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <returns></returns>
		Task<Stream> GetAvatarStreamAsync(string userId);

		/// <summary>
		/// Saves new stream for avatar of specific user
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <param name="stream">stream with avatar</param>
		/// <returns></returns>
		Task SaveAvatarStreamAsync(string userId, Stream stream);

		/// <summary>
		/// Reset avatar for specific user
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <returns></returns>
		Task DeleteAvatarAsync(string userId);
	}
}
