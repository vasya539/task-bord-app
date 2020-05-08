using WebApi.BLs;
using WebApi.Data.Models;

namespace WebApi.Extensions.SprintRoleExtension
{
    /// <summary>
    /// Extends sprint business logic. Defines permissions of specific user roles to do operations with sprints.
    /// </summary>
    public static class SprintRoleExtension
    {
        /// <summary>
        /// Defines user roles which can change sprint state.
        /// </summary>
        /// <param name="sprintBl">Extends sprint business logic class.</param>
        /// <param name="role">Role of user in project.</param>
        /// <returns>Boolean value which points to possibility to change sprint state.</returns>
        public static bool CanChangeSprint(this SprintBl sprintBl, AppUserRole role)
        {
            if (role.IsScrumMaster() || role.IsOwner())
                return true;
            else return false;
        }

        /// <summary>
        /// Defines user roles which can view sprint data.
        /// </summary>
        /// <param name="sprintBl">Extends sprint business logic class.</param>
        /// <param name="role">Role of user in project.</param>
        /// <returns>Boolean value which points to possibility to view sprint data.</returns>
        public static bool CanAccessSprint(this SprintBl sprintBl, AppUserRole role)
        {
            if (role.IsScrumMaster() || role.IsOwner() ||
                role.IsDeveloper() || role.IsObserver())
                return true;
            else return false;
        }
    }
}
