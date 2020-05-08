using System.Collections.Generic;
using System.Threading.Tasks;

using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<IEnumerable<User>> GetAllAsync();

		/// <summary>
		/// Returns user by id
		/// </summary>
		/// <param name="id">id of user</param>
		/// <returns></returns>
		Task<User> GetByIdAsync(string id);

		/// <summary>
		/// Returns detailed user (with Info field) by id
		/// </summary>
		/// <param name="id">id of user</param>
		/// <returns></returns>
		Task<User> GetDetailedByIdAsync(string id);

		/// <summary>
		/// Returns user by username
		/// </summary>
		/// <param name="userName">username of user</param>
		/// <param name="detailed">if true extracts Info field, else Info will be equal null</param>
		/// <returns></returns>
		Task<User> GetByUserNameAsync(string userName, bool detailed);

		/// <summary>
		/// Updates user
		/// </summary>
		/// <param name="user">new state user</param>
		/// <returns></returns>
		Task UpdateAsync(User user);

		/// <summary>
		/// Deletes user
		/// </summary>
		/// <param name="id">id of user</param>
		/// <returns></returns>
		Task DeleteAsync(string id);


		/// <summary>
		/// Returns true if user with specific id does exist
		/// </summary>
		/// <param name="id">if of user</param>
		/// <returns></returns>
		Task<bool> ExistsWithId(string id);

		/// <summary>
		/// Returns true if user with specific username does exist
		/// </summary>
		/// <param name="userName">username of user</param>
		/// <returns></returns>
		Task<bool> ExistsWithUserName(string userName);



		/// <summary>
		/// Finds users by template (username, firstname, lastname) and returns one more user (if available) 
		/// </summary>
		/// <param name="template">template to find users</param>
		/// <param name="page">number of page. first page -- 1</param>
		/// <param name="count">count of users in page. count+1 will be returned if there are more ussers</param>
		/// <returns></returns>
		Task<IEnumerable<UserFoundModel>> FindAdvancedPlusOneRowAsync(string template, int page, int count);

		/// <summary>
		/// Finds users by template (only username) and returns one more user (if available) 
		/// </summary>
		/// <param name="template">template to find users. Must start with '@'</param>
		/// <param name="page">number of page. first page -- 1</param>
		/// <param name="count">count of users in page. count+1 will be returned if there are more ussers</param>
		/// <returns></returns>
		Task<IEnumerable<UserFoundModel>> FindAdvancedByUserNamePlusOneRowAsync(string template, int page, int count);

		/// <summary>
		/// Finds users by template (firstname + lastname) and returns one more user (if available) 
		/// </summary>
		/// <param name="template">template to find users. Must contain 2 words separated by space</param>
		/// <param name="page">number of page. first page -- 1</param>
		/// <param name="count">count of users in page. count+1 will be returned if there are more ussers</param>
		/// <returns></returns>
		Task<IEnumerable<UserFoundModel>> FindAdvancedByFullNamePlusOneRowAsync(string firstName, string lastName, int page, int count);


		/// <summary>
		/// Updates field 'AvatarTail'
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <param name="value">new value</param>
		/// <returns></returns>
		Task UpdateAvatarTailAsync(string userId, int? value);

	}
}
