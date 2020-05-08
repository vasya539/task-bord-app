using WebApi.Data.Models;

namespace WebApi.Extensions.AppUserRolesExtensions.ProjectMemberManagementRolesExtensions
{
	public static class ProjectMemberManagementExtensions
	{
		public static bool CanAddNewMember(this AppUserRole role)
		{
			if (role.IsOwner())
				return true;
			else
				return false;
		}

		public static bool CanRemoveOtherMember(this AppUserRole role)
		{
			if (role.IsOwner())
				return true;
			else
				return false;
		}

		public static bool CanRemoveItself(this AppUserRole role)
		{
			if (role.IsOwner() || role.IsNone())
				return false;
			else
				return true;
		}

		public static bool HasPermissionsToChangeRoleOfMember(this AppUserRole role)
		{
			if (role.IsOwner() || role.IsScrumMaster())
				return true;
			else
				return false;
		}

		public static bool CanChangeRoleOfMember(this AppUserRole caller, AppUserRole oldRole, AppUserRole newRole)
		{
			if (caller.IsNone() || oldRole.IsNone() || newRole.IsNone())
				return false;
			if (!HasPermissionsToChangeRoleOfMember(caller))
				return false;
			if (oldRole == newRole)
				return false;
			if (oldRole.IsOwner() || newRole.IsOwner())
				return false;
			if ((oldRole.IsScrumMaster() || newRole.IsScrumMaster()) && !caller.IsOwner())
				return false;

			return true;
		}

		public static bool CanViewListOfMembers(this AppUserRole role)
		{
			if (!role.IsNone())
				return true;
			else
				return false;
		}

		//public static bool CanBeModified(this AppUserRole role, AppUserRole newRole)
		//{
		//	if (role == newRole)
		//		return false;
		//	if (role.IsOwner() || newRole.IsOwner())
		//		return false;
		//	if (role.IsNone() || newRole.IsNone())
		//		return false;

		//	return true;
		//}
	}
}
