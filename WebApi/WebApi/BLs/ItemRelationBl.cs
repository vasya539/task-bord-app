using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using WebApi.BLs.Communication;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Extensions.AppUserRolesExtensions.Items;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;

namespace WebApi.BLs
{
    /// <summary>
    /// Class for ItemRelation business logic. Implements default interface for ItemRelation business logic.
    /// </summary>
    public class ItemRelationBl : IItemRelationBl
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        private readonly IItemRelationRepository _itemRelationRepository;
        private readonly IProjectUserRepository _puRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly ISprintRepository _sprintRepository;


        /// <summary>
        /// Constructor for initializing ItemRelationRepository, ItemRepository, Mapper and other additional repositories.
        /// </summary>
        /// <param name="itemRepository">Repository for item</param>
        /// <param name="mapper">Auto-mapper</param>
        /// <param name="itemRelationRepository">Repository for ItemRelations</param>
        /// <param name="puRepository">Repository for project-users</param>
        /// <param name="projectRepo">Repository for projects</param>
        /// <param name="sprintRepository">Repository for sprints</param>
        public ItemRelationBl(IItemRepository itemRepository,
            IMapper mapper,
            IItemRelationRepository itemRelationRepository, IProjectUserRepository puRepository,
            IProjectRepository projectRepo,
            ISprintRepository sprintRepository)
        {
            _mapper = mapper;
            _itemRepository = itemRepository;
            _itemRelationRepository = itemRelationRepository;
            _puRepo = puRepository;
            _projectRepo = projectRepo;
            _sprintRepository = sprintRepository;
        }

        /// <summary>
        /// Get specific relation from database
        /// </summary>
        /// <param name="firstItemId">Id of first related item</param>
        /// <param name="secondItemId">Id of second related item</param>
        /// <returns></returns>
        public async Task<ItemRelation> GetRecordAsync(int firstItemId, int secondItemId)
        {
            return await _itemRelationRepository.GetRecordAsync(firstItemId, secondItemId);
        }

        /// <summary>
        /// Get related items for specific item
        /// </summary>
        /// <param name="itemId">Item which must be related to each of this list</param>
        /// <returns>List of ItemDto</returns>
        /// <exception cref="NotFoundResponseException">Relations not found</exception>
        public async Task<IEnumerable<ItemDto>> GetRelatedItemsAsync(int itemId)
        {
            List<ItemDto> items = new List<ItemDto>();
            // Get all relations where 1-st or 2-nd item is our itemId
            var allRelations = await _itemRelationRepository.GetRelatedItems(itemId);
            
            // if relations not found -> exception
            if (allRelations == null) throw new NotFoundResponseException();
            
            // try to get only item which different from out itemId
            foreach (var relation in allRelations)
            {
                // if first item != itemId -> push it into item list
                if (relation.FirstItemId != itemId)
                {
                    var item = await _itemRepository.ReadAsync(relation.FirstItemId);
                    var dtoItem = _mapper.Map<ItemDto>(item);
                    items.Add(dtoItem);
                }
                // if second item != itemId -> push it into item list
                else if (relation.SecondItemId != itemId)
                {
                    var item = await _itemRepository.ReadAsync(relation.SecondItemId);
                    var dtoItem = _mapper.Map<ItemDto>(item);
                    items.Add(dtoItem);
                }
            }
            return items;
        }

        /// <summary>
        /// Create new relation between 2 items
        /// </summary>
        /// <param name="firstItemId">Id of first item</param>
        /// <param name="secondItemId">Id of second item</param>
        /// <param name="userId">id of loginned user</param>
        /// <returns>Response with success message</returns>
        /// <exception cref="ForbiddenResponseException">User don't have access to relate items</exception>
        public async Task<ItemResponse> CreateRecordAsync(int firstItemId, int secondItemId, string userId)
        {
            // Get 2 items
            var firstItem = await _itemRepository.ReadAsync(firstItemId);
            var secondItem = await _itemRepository.ReadAsync(secondItemId);

            // Get user role
            var userRole = await GetUserRoleAsync(firstItem.SprintId, userId);
            
            // User must be part of team to create relation
            if (!userRole.IsPartOfTeam())
                throw new ForbiddenResponseException("Yuo don't have access to create relations!");

            // Check if this relation already exist
            var existRelation = await _itemRelationRepository.GetRecordAsync(firstItemId, secondItemId);
            var existRelation2 = await _itemRelationRepository.GetRecordAsync(secondItemId, firstItemId);

            if (existRelation != null || existRelation2 != null)
                throw new ForbiddenResponseException("This relation is already exist!");

            // Create relation
            var relation = new ItemRelation { FirstItemId = firstItem.Id, SecondItemId = secondItem.Id };
            await _itemRelationRepository.CreateRecordAsync(relation);
            return new ItemResponse(true, "Related!");
        }

        /// <summary>
        /// Delete specific relation from database
        /// </summary>
        /// <param name="firstItemId">Id of first item</param>
        /// <param name="secondItemId">Id of second item</param>
        /// <param name="userId">id of loginned user</param>
        /// <returns>Response with success message</returns>
        /// <exception cref="ForbiddenResponseException">User don't have access to delete relation</exception>
        public async Task<ItemResponse> DeleteRecordAsync(int firstItemId, int secondItemId, string userId)
        {
            // Check if this relation exist
            var existRelation = await _itemRelationRepository.GetRecordAsync(firstItemId, secondItemId);
            var existRelation2 = await _itemRelationRepository.GetRecordAsync(secondItemId, firstItemId);

            if (existRelation == null && existRelation2 == null) throw new ForbiddenResponseException("This relation does not exist!");

            // If relation exist -> delete it
            if (existRelation != null) await _itemRelationRepository.DeleteRecordAsync(existRelation);

            if (existRelation2 != null) await _itemRelationRepository.DeleteRecordAsync(existRelation2);

            return new ItemResponse(true, "Deleted!");
        }

        /// <summary>
        /// Addition method to get user role for validations
        /// </summary>
        /// <returns>AppUserRole object</returns>
        public async Task<AppUserRole> GetUserRoleAsync(int sprintId, string userId)
        {
            Sprint sprint = await _sprintRepository.GetByIdAsync(sprintId);

            Project project = await _projectRepo.GetByIdAsync(sprint.ProjectId);
            if (project == null)
                throw new NotFoundResponseException();

            return await _puRepo.GetRoleOfMember(userId, sprint.ProjectId);
        }
    }
}