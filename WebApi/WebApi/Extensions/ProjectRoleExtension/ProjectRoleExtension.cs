using WebApi.BLs;
using WebApi.Data.Models;

namespace WebApi.Extensions.ProjectRoleExtension
{
    /// <summary>
    /// Extends project business logic. Defines permissions of specific user roles to do operations with projects.
    /// </summary>
    public static class ProjectRoleExtension
    {
        /// <summary>
        /// Defines user roles which can change project state.
        /// </summary>
        /// <param name="projectBl">Extends project business logic class.</param>
        /// <param name="role">Role of user in project.</param>
        /// <returns>Boolean value which points to possibility to change project state.</returns>
        public static bool CanChangeProject(this ProjectBl projectBl, AppUserRole role)
        {
            if (role.IsOwner())
                return true;
            else return false;
        }

        /// <summary>
        /// Defines user roles which can view project data.
        /// </summary>
        /// <param name="projectBl">Extends project business logic class.</param>
        /// <param name="role">Role of user in project.</param>
        /// <returns>Boolean value which points to possibility to view project data.</returns>
        public static bool CanAccessProject(this ProjectBl projectBl, AppUserRole role)
        {
            if (role.IsScrumMaster() || role.IsOwner() ||
                role.IsDeveloper() || role.IsObserver())
                return true;
            else return false;
        }
    }
}
