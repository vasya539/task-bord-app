using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.BLs.Interfaces
{
	public interface IAvatarBl
	{
		/// <summary>
		/// Returns avatar as file (picture) to send it to user
		/// </summary>
		/// <param name="userId">id of user whose avatar is requested</param>
		/// <returns></returns>
		Task<FileResult> GetAvatarAsync(string userId);

		/// <summary>
		/// Updates avatar for some user
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <param name="fileStream">stream with new avatar</param>
		/// <returns></returns>
		Task UpdateAvatarAsync(string userId, Stream fileStream);

		/// <summary>
		/// Clears avatar for user
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <returns></returns>
		Task DeleteAvatarAsync(string userId);
	}
}
