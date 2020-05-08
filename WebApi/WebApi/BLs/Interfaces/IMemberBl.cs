using System.Collections.Generic;
using System.Threading.Tasks;

using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
	public interface IMemberBl
	{
		/// <summary>
		/// Returns list of available roles
		/// </summary>
		/// <returns></returns>
		IEnumerable<AppUserRoleDto> GetAvailableUserRoles();

		/// <summary>
		/// Returns all members of specific project
		/// </summary>
		/// <param name="senderUserId">id of user that makes request</param>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<IEnumerable<ProjectMemberDto>> GetAllMembersOfProjectAsync(string senderUserId, int projectId);

		/// <summary>
		/// Returns info about specific member in project
		/// </summary>
		/// <param name="senderUserId">id of user that makes request</param>
		/// <param name="projectId">id of project</param>
		/// <param name="userId">id of specific member</param>
		/// <returns></returns>
		Task<ProjectMemberDto> GetMemberOfProjectAsync(string senderUserId, int projectId, string userId);

		/// <summary>
		/// Returns current role of user in specific project
		/// </summary>
		/// <param name="userId">id of this user</param>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<AppUserRoleDto> GetMyRoleInProjectAsync(string userId, int projectId);

		/// <summary>
		/// Adds new member to project
		/// </summary>
		/// <param name="senderUserId">id of user that makes request</param>
		/// <param name="projectId">id of project</param>
		/// <param name="userId">id of user which should be added to project</param>
		/// <param name="role">start-role of new member</param>
		/// <returns></returns>
		Task AddMemberAsync(string senderUserId, int projectId, string userId, AppUserRoleDto role);

		/// <summary>
		/// Changes role of member of project
		/// </summary>
		/// <param name="senderUserId">id of user that makes request</param>
		/// <param name="projectId">id of project</param>
		/// <param name="userId">id of user that is member</param>
		/// <param name="newRole">new role of member</param>
		/// <returns></returns>
		Task ChangeUserRoleAsync(string senderUserId, int projectId, string userId, AppUserRoleDto newRole);

		/// <summary>
		/// Removes member from project
		/// </summary>
		/// <param name="senderUserId">id of user that makes request</param>
		/// <param name="projectId">id of project</param>
		/// <param name="userId">id of user that is the member</param>
		/// <returns></returns>
		Task RemoveMemberAsync(string senderUserId, int projectId, string userId);
	}
}
