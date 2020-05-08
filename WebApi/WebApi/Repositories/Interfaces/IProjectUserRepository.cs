using System.Collections.Generic;
using System.Threading.Tasks;

using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
	public interface IProjectUserRepository
	{
		/// <summary>
		/// Returns record for Project-User relation
		/// </summary>
		/// <param name="userId">id of User</param>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<ProjectUser> GetRecordAsync(string userId, int projectId);

		/// <summary>
		/// Creates new record for Project-User relation
		/// </summary>
		/// <param name="pu">instance of ProjectUser class</param>
		/// <returns></returns>
		Task CreateRecordAsync(ProjectUser pu);

		/// <summary>
		/// Changes existing record for Project-User relation
		/// </summary>
		/// <param name="pu">instance of ProjectUser class</param>
		/// <returns></returns>
		Task UpdateRecordAsync(ProjectUser pu);

		/// <summary>
		/// Deletes existing record for Project-User relation
		/// </summary>
		/// <param name="pu">instance of ProjectUser class</param>
		/// <returns></returns>
		Task DeleteRecordAsync(ProjectUser pu);

		/// <summary>
		/// Deletes existing record for Project-User relation
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task DeleteRecordAsync(string userId, int projectId);


		/// <summary>
		/// Returns all projects for specific user
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <returns></returns>
		Task<IEnumerable<Project>> GetProjectsOfUser(string userId);

		/// <summary>
		/// Returns all members of project
		/// </summary>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<IEnumerable<User>> GetMembersOfProject(int projectId);

		/// <summary>
		/// Returns owner of specific project
		/// </summary>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<User> GetOwnerOfProject(int projectId);

		/// <summary>
		/// Returns role of specific user in specific project
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<AppUserRole> GetRoleOfMember(string userId, int projectId);

		/// <summary>
		/// Returns true if specific project contains Scrum Master
		/// </summary>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<bool> DoesExistScrumMasterInProjectAsync(int projectId);

		/// <summary>
		/// Returns true if this project has this user
		/// </summary>
		/// <param name="userId">id of user</param>
		/// <param name="projectId">id of project</param>
		/// <returns></returns>
		Task<bool> DoesExistMemberOfProject(string userId, int projectId);
	}
}
