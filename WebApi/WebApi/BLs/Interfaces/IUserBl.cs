using System.Collections.Generic;
using System.Threading.Tasks;

using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
	public interface IUserBl
	{
		Task<IEnumerable<UserDto>> GetAllAsync();

		/// <summary>
		/// Returns user with specific id as Dto
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<UserDto> GetByIdAsync(string id);

		/// <summary>
		/// Returns detailed user with specific id as Dto
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<UserDto> GetDetailedByIdAsync(string id);

		/// <summary>
		/// Returns user with specific username as Dto
		/// </summary>
		/// <param name="userName">username of user</param>
		/// <param name="detailed">if detailed equals true extract Info else set Info to null</param>
		/// <returns></returns>
		Task<UserDto> GetByUserNameAsync(string userName, bool detailed);

		/// <summary>
		/// Returns list of projects for requestedUser which are available for senderUser. Returns common projects for these users
		/// </summary>
		/// <param name="senderId">id of user who makes request</param>
		/// <param name="requestedUserId">id of user whose projects should be returned</param>
		/// <returns></returns>
		Task<IEnumerable<SimpleProjectDto>> GetProjectsOfUser(string senderId, string requestedUserId);

		/// <summary>
		/// Finds users using given template and returns page
		/// </summary>
		/// <param name="template">template</param>
		/// <param name="pageNumber">number of page. first page number is 1</param>
		/// <returns></returns>
		Task<FoundUsersPageDto> Find(string template, int pageNumber);

		/// <summary>
		/// Updates user
		/// </summary>
		/// <param name="userId">id of user whose settings should be changed</param>
		/// <param name="dto">new state of user</param>
		/// <returns></returns>
		Task UpdateAsync(string userId, UserDto dto);

		/// <summary>
		/// Deletes user with specific id
		/// </summary>
		/// <param name="id">id of user</param>
		/// <returns></returns>
		Task DeleteAsync(string id);
	}
}
