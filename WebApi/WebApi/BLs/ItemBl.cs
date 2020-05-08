using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Extensions.AddictionEnumerations;
using WebApi.Extensions.AppUserRolesExtensions.Items;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;

namespace WebApi.BLs
{
    /// <summary>
    /// Class for item business logic. Implements default interface for item business logic.
    /// </summary>
    public class ItemBl : IItemBl
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IProjectUserRepository _puRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly ISprintRepository _sprintRepository;

        
        /// <summary>
        /// Constructor for initializing ItemRepository, Mapper and other additional repositories.
        /// </summary>
        /// <param name="itemRepository">Repository for item</param>
        /// <param name="mapper">Auto-mapper</param>
        /// <param name="commentRepository">Repository for comments</param>
        /// <param name="puRepository">Repository for project-users</param>
        /// <param name="projectRepo">Repository for projects</param>
        /// <param name="sprintRepository">Repository for sprints</param>
        public ItemBl(IItemRepository itemRepository,
            IMapper mapper,
            ICommentRepository commentRepository,
            IProjectUserRepository puRepository,
            IProjectRepository projectRepo,
            ISprintRepository sprintRepository)
        {
            _mapper = mapper;
            _puRepo = puRepository;
            _projectRepo = projectRepo;
            _itemRepository = itemRepository;
            _commentRepository = commentRepository;
            _sprintRepository = sprintRepository;
        }

        #region CRUD-Operations

        /// <summary>
        /// Use repository to get all items from Database
        /// </summary>
        /// <returns>List of ItemDto to controller</returns>
        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            var items = await _itemRepository.GetAllAsync();

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);

            return dtoItems;
        }

        /// <summary>
        /// Read. Search item with param Id in database, and map it.
        /// </summary>
        /// <param name="id">Id of the item to be returned to client</param>
        /// <returns>Needful item to controller</returns>
        public async Task<ItemDto> ReadAsync(int id)
        {
            var item = await _itemRepository.ReadAsync(id);
            var dtoItem = _mapper.Map<ItemDto>(item);

            return dtoItem;
        }

        /// <summary>
        /// Create. Map ItemDto => Item and use repository to create a new item. Check permissions.
        /// Only team-member can create items. Only scrum master or owner can create user-stories.
        /// </summary>
        /// <param name="item"> Item to create</param>
        /// <param name="userId">User's id</param>
        /// <returns>Item response with message</returns>
        /// <exception cref="ForbiddenResponseException">IF user don't have access to create item</exception>
        public async Task<ItemResponse> CreateAsync(ItemDto item, string userId)
        {
            var origItem = _mapper.Map<Item>(item);
            // Get role
            var userRole = await GetUserRoleAsync(origItem, userId);

            // Check if user is Owner, Master, or Developer
            if (!userRole.IsPartOfTeam())
                throw new ForbiddenResponseException("You dont have access to create items! Please, call your scrum-master or owner.");
            // Check if type of item is user-story. Only master and owner can create story
            if (item.TypeId == (int)ItemTypes.UserStory)
                if (!userRole.IsScrumMasterOrOwner())
                    throw new ForbiddenResponseException("You dont have access to create the user-story!");
                
            CreateItemRulesCheck(origItem);
            userRole.CreateItemAccessValidation(origItem, userId);

            await _itemRepository.CreateAsync(origItem);
            return new ItemResponse(true, "Created");

        }

      
        
        /// <summary>
        /// Get itemDto from controller, map it , get role of user, and get existing item from repository. After that check access to make an update.
        /// Only member of team can update. More validation rules check in user-role extension methods. 
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <param name="userId">User id</param>
        /// <returns>Item response with message</returns>
        /// <exception cref="ForbiddenResponseException">IF user don't have access to do this update</exception>
        public async Task<ItemResponse> UpdateAsync(ItemDto item, string userId)
        {
            // Get old item
            var existingItem = await _itemRepository.ReadAsync(item.Id);
            // Get role
            var userRole = await GetUserRoleAsync(existingItem, userId);
            var newItem = _mapper.Map<Item>(item);
            
            // Check if user is Owner, Master, or Developer
            if (!userRole.IsPartOfTeam())
                throw new ForbiddenResponseException("You dont have access to do this operation! Please, call your scrum-master or owner.");
            // Set of rules
            userRole.CanDoSomething(existingItem, userId);
            userRole.CheckCorrectAssigning(existingItem, newItem, userId);
            userRole.CheckCorrectStatuses(existingItem, newItem, userId);
            await CheckCorrectRelationsAsync(existingItem, newItem);
            // Check if item-type was changed -> throw exception. Type can't be changed
            if (existingItem.TypeId != newItem.TypeId)
                throw new ForbiddenResponseException("Sorry. Type can't be changed!");

            UpdateItemRulesCheck(existingItem, newItem);
            await _itemRepository.UpdateAsync(newItem);
            return new ItemResponse(true, "Updated successfully!");

        }

        /// <summary>
        /// Delete. Get item Id from controller, get item from repository and if item exist - delete id.
        ///  Only Scrum-master or Owner can delete.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="userId">User id</param>
        /// <returns>Item response with message</returns>
        /// <exception cref="NotFoundResponseException"> If item not exist in database</exception>
        /// <exception cref="ForbiddenResponseException"> IF user don't have access to delete item</exception>
        public async Task<ItemResponse> DeleteAsync(int id, string userId)
        {
            // get necessary item
            var realItem = await _itemRepository.ReadAsync(id);
            
            if (realItem == null) throw new NotFoundResponseException();
            
            // get role
            var userRole = await GetUserRoleAsync(realItem, userId);
            
            // only master or owner can delete item
            if (!userRole.IsScrumMasterOrOwner()) throw new ForbiddenResponseException("You cannot delete this item!");
            await _itemRepository.DeleteAsync(id);
            return new ItemResponse(true, "Deleted successfully");
        }

        #endregion CRUD-Operations

        #region OtherOperations

        /// <summary>
        /// Get all items by sprint id
        /// </summary>
        /// <param name="sprintId">Sprint's id</param>
        /// <returns>List of ItemDto</returns>
        public async Task<IEnumerable<ItemDto>> GetBySprintIdAsync(int sprintId)
        {
            var items = await _itemRepository.GetBySprintIdAsync(sprintId);

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
            return dtoItems;
        }

        /// <summary>
        /// Method to item archivating. Check user's permissions.
        /// Only scrum master of owner can archive item. If item have child , they move to unparented.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="userId">User id</param>
        /// <returns>Item response with message</returns>
        /// <exception cref="ForbiddenResponseException">IF user don't have access to archive item</exception>
        public async Task<ItemResponse> ArchivingAsync(int id, string userId)
        {
            // get item
            var realItem = await _itemRepository.ReadAsync(id);
            
            if (realItem==null) throw new NotFoundResponseException();
            // get role
            var userRole = await GetUserRoleAsync(realItem, userId);

            
            // only master or owner can delete item
            if (!userRole.IsScrumMasterOrOwner()) throw new ForbiddenResponseException("You cannot archive this item!");
            realItem.IsArchived = !realItem.IsArchived;
            // Get item childs
            List<Item> childs = await _itemRepository.GetAllChildAsync(id) as List<Item>;
            if (childs != null && childs.Count > 0)
            {
                // for each childs -> set parent to null and update
                foreach (var item in childs)
                {
                    item.ParentId = null;
                    await _itemRepository.UpdateAsync(item);
                }
            }
            await _itemRepository.UpdateAsync(realItem);
            return new ItemResponse(true, "Archived successfully");
        }

        /// <summary>
        /// Get all archivated items by for specific sprint
        /// </summary>
        /// <param name="sprintId">Sprint id</param>
        /// <returns>List of ItemDto</returns>
        public async Task<IEnumerable<ItemDto>> GetArchivedBySprintIdAsync(int sprintId)
        {
            var items = await _itemRepository.GetArchivedBySprintIdAsync(sprintId);

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
            return dtoItems;
        }

        /// <summary>
        ///  Get all child for specific item
        /// </summary>
        /// <param name="itemId"> Item id</param>
        /// <returns>List of ItemDto</returns>
        public async Task<IEnumerable<ItemDto>> GetAllChildAsync(int itemId)
        {
            var items = await _itemRepository.GetAllChildAsync(itemId);

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
            return dtoItems;
        }

        /// <summary>
        /// Get all userstories for specific sprint
        /// </summary>
        /// <param name="sprintId">Item id</param>
        /// <returns>List of ItemDto </returns>
        public async Task<IEnumerable<ItemDto>> GetUserStoriesAsync(int sprintId)
        {
            var items = await _itemRepository.GetUserStoriesAsync(sprintId);

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
            return dtoItems;
        }

        /// <summary>
        /// Get all unparented item for specific sprint
        /// </summary>
        /// <param name="sprintId">Item id</param>
        /// <returns>List of ItemDto</returns>
        public async Task<IEnumerable<ItemDto>> GetUnparentedAsync(int sprintId)
        {
            var items = await _itemRepository.GetUnparentedAsync(sprintId);

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
            return dtoItems;
        }

        /// <summary>
        /// Get all childs with specific status for specific item
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="statusId">Status id</param>
        /// <returns>List of ItemDto</returns>
        public async Task<IEnumerable<ItemDto>> GetChildWithSpecificStatusAsync(int itemId, int statusId)
        {
            var items = await _itemRepository.GetChildWithSpecificStatusAsync(itemId, statusId);

            IEnumerable<ItemDto> dtoItems = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(items);
            return dtoItems;
        }

        #endregion OtherOperations

        #region Logic functions

        /// <summary>
        /// Check rules for item-update
        /// </summary>
        /// <param name="existingItem">Old item</param>
        /// <param name="newItem"> New item </param>
        /// <exception cref="ForbiddenResponseException">IF user don't have access to update item</exception>
        private void UpdateItemRulesCheck(Item existingItem, Item newItem)
        {
            // If item moved from 'New' and don't have assigned user -> return item to 'New';
            if (existingItem.StatusId == (int)ItemStatuses.New && (newItem.AssignedUserId == null && newItem.StatusId != (int)ItemStatuses.New))
            {
                newItem.StatusId = (int)ItemStatuses.New;
                throw new ForbiddenResponseException("Item must have user to be Active!");
            }

            // If item not 'New' and assigned user set to Null -> Move item to 'New'
            if (existingItem.StatusId != (int)ItemStatuses.New && newItem.AssignedUserId == null)
            {
                newItem.StatusId = (int)ItemStatuses.New;
            }

            // If item not 'New' and moved to 'New' -> Set assigned user to null;
            if (existingItem.StatusId != (int)ItemStatuses.New && newItem.StatusId == (int)ItemStatuses.New)
            {
                newItem.AssignedUserId = null;
            }

            // If item 'New' and have assigned user-> Move item to 'Active';
            if (existingItem.StatusId == (int)ItemStatuses.New && newItem.AssignedUserId != null)
            {
                newItem.StatusId = (int)ItemStatuses.Active;
            }
        }

        /// <summary>
        /// Check rules for item-create operation
        /// </summary>
        /// <param name="item">Item to create</param>
        /// <exception cref="ForbiddenResponseException">IF user don't have access to create item</exception>
        private void CreateItemRulesCheck(Item item)
        {
            // If item not 'New' and assigned user is Null -> Move item to 'New'
            if (item.StatusId != (int)ItemStatuses.New && item.AssignedUserId == null)
            {
                //item.StatusId = (int)ItemStatuses.New;
                throw new ForbiddenResponseException("Not 'New' item must be assigned by user!");
            }

            // If item 'New' and have assigned user-> Move item to 'Active';
            if (item.StatusId == (int)ItemStatuses.New && item.AssignedUserId != null)
            {
                //item.StatusId = (int)ItemStatuses.Active;
                throw new ForbiddenResponseException("'New' item can't be assigned by user!");
            }
        }

        /// <summary>
        /// Get user-role for loginned user.
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="userId">User id</param>
        /// <returns>User-Role</returns>
        private async Task<AppUserRole> GetUserRoleAsync(Item item, string userId)
        {
            Sprint sprint = await _sprintRepository.GetByIdAsync(item.SprintId);

            Project project = await _projectRepo.GetByIdAsync(sprint.ProjectId);

            return await _puRepo.GetRoleOfMember(userId, sprint.ProjectId);
        }

        /// <summary>
        /// Check item relations rules. Specific rules for parent/child.
        /// </summary>
        /// <param name="existingItem">Old item</param>
        /// <param name="newItem">New item</param>
        /// <returns>Void task</returns>
        /// <exception cref="ForbiddenResponseException">IF user don't have access to set this relation item</exception>
        private async Task CheckCorrectRelationsAsync(Item existingItem, Item newItem)
        {
            // if parent changed -> check rules
            if (existingItem.ParentId != newItem.ParentId)
                if (newItem.ParentId != null)
                {
                    // get second relation item
                    var secondItem = await _itemRepository.ReadAsync((int)newItem.ParentId);
                    if (newItem.TypeId == (int)ItemTypes.Task && secondItem.TypeId != (int)ItemTypes.UserStory) throw new ForbiddenResponseException($"Only user-story can be parent for task!");
                    // story can have parent from another sprint
                    if (secondItem.SprintId != newItem.SprintId && newItem.TypeId != (int)ItemTypes.UserStory) throw new ForbiddenResponseException($"Only user-story can have parent from another sprint");
                    // check if we set as a parent our child
                    if (secondItem.ParentId == existingItem.Id) throw new ForbiddenResponseException($"Item {secondItem.Name} is already our child!");
                }
        }

        #endregion Logic functions
    }
}