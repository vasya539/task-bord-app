using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Extensions.AddictionEnumerations;

namespace WebApi.Extensions.AppUserRolesExtensions.Items
{
    public static class ItemManagementExtensions
    {
        public static bool CanSetAssignedUser(this AppUserRole role)
        {
            return role.IsOwner() || role.IsScrumMaster();
        }


        public static void CheckCorrectAssigning(this AppUserRole userRole, Item existingItem, Item newItem, string userId)
        {
            // if assigning not changed -> OK
            if (existingItem.AssignedUserId == newItem.AssignedUserId) return;
            
            // if user master or owner -> OK
            if (userRole.IsScrumMasterOrOwner())
                return;
            
            // check if developer can assign item
            if (userRole.DevAssignOperations(existingItem, newItem, userId))
                return;
            throw new ForbiddenResponseException("You dont have access to change assigning. Please, call your scrum-master or owner.");
        }

        private static bool DevAssignOperations(this AppUserRole role, Item existingItem, Item newItem, string userId)
        {
            // User only can assign task which was 'New' and unassigned.
            if ((existingItem.StatusId == (int)ItemStatuses.New && existingItem.AssignedUserId == null) && (newItem.AssignedUserId == userId))
                return true;
            throw new ForbiddenResponseException("You only can assign NEW item to yourself!");
        }
        

        public static void CheckCorrectStatuses(this AppUserRole userRole, Item existingItem, Item newItem, string userId)
        {
            // if status not changed -> OK
            if (existingItem.StatusId == newItem.StatusId) return;
            
            // if user master or owner -> OK
            if (userRole.IsScrumMasterOrOwner())
                return;
            
            // check if developer can change status
            if (DevMoveItemOperations(existingItem, newItem, userId))
                return;
            
            throw new ForbiddenResponseException("You dont have access to change item status. Please, call your scrum-master or owner.");
        }

        private static bool DevMoveItemOperations(Item existingItem, Item newItem, string userId)
        {
            // Developer only can move himself item between 'Active' and 'Closed'.
            if (existingItem.StatusId >= (int)ItemStatuses.New && newItem.StatusId > (int)ItemStatuses.New && newItem.AssignedUserId == userId)
                return true;
            
            throw new ForbiddenResponseException("You only can move your items between Active and Closed.");
        }


        public static bool CanDeleteComment(this AppUserRole role, Comment comment, string userId)
        {
           // User can delete only comment which are written by himself
            return role.IsScrumMasterOrOwner() || (role.IsDeveloper() && comment.UserId == userId);
        }

        public static bool CanCreateItem(this AppUserRole role, Item item, string userId)
        {
            // User can't create item and assign another user to this item
            if (item.AssignedUserId == userId && role.IsDeveloper() || (item.StatusId == (int)ItemStatuses.New && item.AssignedUserId == null))
                return true;
            
            throw new ForbiddenResponseException("You only can create item assigned by your, or new unassigned");
        }

        public static void CanDoSomething(this AppUserRole role, Item item, string userId)
        {
            // User must be owner or master or to be assigned to this task to update it.
            if (!(role.IsScrumMasterOrOwner() || item.AssignedUserId == userId || item.AssignedUserId == null))
                throw new ForbiddenResponseException("Sorry, you can edit only items which are asssigned by you!");
            
        }

        public static void CreateItemAccessValidation(this AppUserRole userRole, Item item, string userId)
        {
            //Check if user is master or owner -> he can create everything.
            if (userRole.IsScrumMasterOrOwner()) return;
            
            // Check if Developer can create item
            if (userRole.CanCreateItem(item, userId)) return;
            
            throw new ForbiddenResponseException("You dont have access to create items!");
        }

        public static bool IsScrumMasterOrOwner(this AppUserRole role)
        {
            if (role.IsOwner() || role.IsScrumMaster())
                return true;
            else
                return false;
        }

        public static bool IsPartOfTeam(this AppUserRole role)
        {
            if (role.IsOwner() || role.IsScrumMaster() || role.IsDeveloper())
                return true;
            else
                return false;
        }
    }
}

namespace WebApi.Extensions.AddictionEnumerations
{
    internal enum ItemTypes : byte
    {
        UserStory = 1,
        Task,
        Bug,
        Test
    }

    internal enum ItemStatuses : byte
    {
        New = 1,
        Active,
        CodeReview,
        Resolved,
        Closed
    }
}