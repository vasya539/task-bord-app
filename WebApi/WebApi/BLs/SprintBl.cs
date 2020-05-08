using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.BLs.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.BLs.Communication;
using AutoMapper;
using WebApi.Data.DTOs;
using WebApi.Extensions.SprintRoleExtension;
using WebApi.Exceptions;
using WebApi.Interfaces.IRepositories;

namespace WebApi.BLs
{
    /// <summary>
    /// Provides business logic of sprint managing operations.
    /// </summary>
    public class SprintBl : ISprintBl
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for sprint business logic.
        /// </summary>
        /// <param name="sprintRepository">Sprint repository object.</param>
        /// <param name="mapper">AutoMapper object.</param>
        /// <param name="projectUserRepository">ProjectUser repository object.</param>
        /// <param name="itemRepository">Item repository object.</param>
        public SprintBl(ISprintRepository sprintRepository, IMapper mapper,
            IProjectUserRepository projectUserRepository, IItemRepository itemRepository)
        {
            _sprintRepository = sprintRepository;
            _mapper = mapper;
            _projectUserRepository = projectUserRepository;
            _itemRepository = itemRepository;
        }

        /// <summary>
        /// Gets all sprints by project Id from sprint repository.
        /// </summary>
        /// <param name="projectId">Id of project.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>Collection of sprint DTO's.</returns>
        public async Task<IEnumerable<SprintDto>> GetAllByProjectIdAsync(int projectId, string userId)
        {
            // Gets role of user in current project and checks his rights to view sprints
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, projectId);
            if (!this.CanAccessSprint(role))
                throw new ForbiddenResponseException("You do not have access");

            // Gets all sprints of current project and returns them to controller
            IEnumerable<Sprint> sprints = await _sprintRepository.GetAllByProjectIdAsync(projectId);
            IEnumerable<SprintDto> dto = _mapper.Map<IEnumerable<Sprint>, IEnumerable<SprintDto>>(sprints);
            return dto;
        }
        /// <summary>
        /// Gets all unarchived items of sprint from sprint repository and groups them by parent item.
        /// </summary>
        /// <param name="id">Id of sprint.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>Collection of ItemList DTO's.</returns>
        public async Task<IEnumerable<ItemListDto>> GetAllSprintItemsAsync(int id, string userId)
        {
            // Searchs sprint by id and extracts project id from result
            Sprint sprint = await _sprintRepository.GetByIdAsync(id);
            int projectId = sprint.ProjectId;

            // Gets role of user in current project and checks his rights to view sprint items
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, projectId);
            if (!this.CanAccessSprint(role))
                throw new ForbiddenResponseException("You do not have access");

            // Gets all non archived items without parents
            var unparantedItems = (await _itemRepository.GetUnparentedAsync(id)).Where(item => item.IsArchived == false);
            var userStories = (await _itemRepository.GetUserStoriesAsync(id)).Where(item => item.IsArchived == false);
            var topLevelItems = (userStories ?? Enumerable.Empty<Item>()).Concat(unparantedItems ?? Enumerable.Empty<Item>());
            IEnumerable<ItemListDto> itemList = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemListDto>>(topLevelItems);

            // Gets all non archived children for each item without parent
            foreach (var parentItem in itemList)
            {
                var childItems = (await _itemRepository.GetAllChildAsync(parentItem.Id)).Where(item => item.IsArchived == false);
                IEnumerable<ItemDto> childItemsDto = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(childItems);
                parentItem.Items.AddRange(childItemsDto);
            }
            return itemList;
        }

        /// <summary>
        /// Gets sprint by Id from sprint repository.
        /// </summary>
        /// <param name="id">Id of sprint.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>Sprint DTO.</returns>
        public async Task<SprintDto> GetByIdAsync(int id, string userId)
        {
            // Searchs sprint by id
            Sprint sprint = await _sprintRepository.GetByIdAsync(id);

            // Gets role of user in current project and checks his rights to view sprints
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, sprint.ProjectId);
            if (!this.CanAccessSprint(role))
                throw new ForbiddenResponseException("You do not have access");

            // Maps and returns sprint DTO to controller
            SprintDto sprintDTO = _mapper.Map<Sprint, SprintDto>(sprint);
            return sprintDTO;
        }

        /// <summary>
        /// Validates and maps sprint DTO, calls creating sprint method from sprint repository.
        /// </summary>
        /// <param name="dto">Sprint DTO.</param>
        /// <param name="userId">Id of user</param>
        /// <returns>SprintResponse object</returns>
        public async Task<SprintResponse> CreateAsync(SprintDto dto, string userId)
        {
            // Gets role of user in current project and checks his rights to create the sprint
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, dto.ProjectId);
            if (!this.CanChangeSprint(role)) 
                throw new ForbiddenResponseException("Only owner and scrum master can create a sprint");
            try
            {
                Sprint sprint = _mapper.Map<SprintDto, Sprint>(dto);
                
                // Checks that sprint end date is greater than sprint start date
                if (sprint.EndDate <= sprint.StartDate)
                    return new SprintResponse("Sprint end date must be later than start date");

                // Searchs all sprints of current project and takes last sprint
                IEnumerable<Sprint> sprints = await _sprintRepository.GetAllByProjectIdAsync(dto.ProjectId);
                Sprint lastSprint = sprints.OrderByDescending(sp => sp.EndDate).FirstOrDefault();

                // Checks if new sprint start date is greater than previous sprint end date
                if (lastSprint != null && lastSprint.EndDate >= sprint.StartDate)
                {
                    return new SprintResponse($"Sprint overlaps. Choose start date after {lastSprint.EndDate.Value.ToShortDateString()}");                    
                }

                // Tries to create new sprint in database and returns response to controller
                await _sprintRepository.CreateAsync(sprint);
                var sprintDTO = _mapper.Map<Sprint, SprintDto>(sprint);
                return new SprintResponse(sprintDTO);
            }
            catch (Exception)
            {
                return new SprintResponse($"An error occurred when saving the sprint");
            }
        }

        /// <summary>
        /// Validates and maps sprint DTO, calls updating sprint method from sprint repository.
        /// </summary>
        /// <param name="dto">Sprint DTO.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>SprintResponse object.</returns>
        public async Task<SprintResponse> UpdateAsync(SprintDto dto, string userId)
        {
            // Searchs if exist sprint with id took from sprint DTO
            var existingSprint = await _sprintRepository.GetByIdAsync(dto.Id);

            // Gets role of user in current project and checks his rights to update the sprint
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, existingSprint.ProjectId);
            if (!this.CanChangeSprint(role))
                throw new ForbiddenResponseException("Only owner and scrum master can update a sprint");

            // Returns response in controller when sprint not found
            if (existingSprint == null)
                return new SprintResponse("Sprint not found");

            // Checks that sprint end date is greater than sprint start date
            if (dto.EndDate <= dto.StartDate)
                return new SprintResponse("Sprint end date must be later than start date");
            
            // Searchs all sprints of current project and orders by descending
            IEnumerable<Sprint> sprints = await _sprintRepository.GetAllByProjectIdAsync(existingSprint.ProjectId);
            var orderedSprintDesc = sprints.OrderByDescending(sp => sp.EndDate).ToList();

            // Searchs indexes of current, previous and next sprints in ordered sprint list
            int existingSprintIndex = orderedSprintDesc.IndexOf(existingSprint);
            int previousSprintIndex = (existingSprintIndex != orderedSprintDesc.Count() - 1) ? existingSprintIndex + 1 : -1;
            int nextSprintIndex = (existingSprintIndex != 0) ? existingSprintIndex - 1 : -1;

            // Searchs previous and next sprints in ordered sprint list
            Sprint previousSprint = (previousSprintIndex != -1) ? orderedSprintDesc[previousSprintIndex] : null;
            Sprint nextSprint = (nextSprintIndex != -1) ? orderedSprintDesc[nextSprintIndex] : null;

            // Changing first sprint, checks if first sprint end date is greater than next sprint start date
            if (orderedSprintDesc.Count>1 && previousSprint == null && nextSprint.StartDate <= dto.EndDate)
            {
                return new SprintResponse($"Error. Choose end date before {nextSprint.StartDate.Value.ToShortDateString()}");
            }

            // Changing sprint "in the middle", checks if changed sprint start date is greater than previous
            // sprint end date and changed sprint end date is less than next sprint start date
            if (previousSprint != null && nextSprint != null &&
                (dto.StartDate <= previousSprint.EndDate || dto.EndDate >= nextSprint.StartDate))
            {

                return new SprintResponse($"Error. Choose dates after {previousSprint.EndDate.Value.ToShortDateString()} and before {nextSprint.StartDate.Value.ToShortDateString()}");
            }

            // Changing last sprint, checks if last sprint start date is greater than previous sprint end date
            if (orderedSprintDesc.Count > 1 && nextSprint == null && previousSprint.EndDate >= dto.StartDate)
            {
                return new SprintResponse($"Error. Choose start date after {previousSprint.EndDate.Value.ToShortDateString()}");
            }

            // Updates sprint data from DTO
            existingSprint.Name = dto.Name;
            existingSprint.Description = dto.Description;
            existingSprint.StartDate = dto.StartDate;
            existingSprint.EndDate = dto.EndDate;

            try
            {
                // Tries to save changes in database and returns response to controller
                await _sprintRepository.UpdateAsync(existingSprint);
                SprintDto sprintDTO = _mapper.Map<Sprint, SprintDto>(existingSprint);
                return new SprintResponse(sprintDTO);
            }
            catch (Exception)
            {
                return new SprintResponse($"An error occurred when updating the sprint");
            }
        }
        
        /// <summary>
        /// Checks if deletion of sprint is permitted and calls deleting sprint method from sprint repository.
        /// </summary>
        /// <param name="id">Id of sprint.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>SprintResponse object.</returns>
        public async Task<SprintResponse> DeleteAsync(int id, string userId)
        {
            // Searchs if exist sprint with id took from parameter
            Sprint existingSprint = await _sprintRepository.GetByIdAsync(id);

            // Returns response in controller when sprint not found
            if (existingSprint == null)
                return new SprintResponse("Sprint not found");

            // Gets role of user in current project and checks his rights to update the sprint
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, existingSprint.ProjectId);
            if (!this.CanChangeSprint(role))
                throw new ForbiddenResponseException("Only owner and scrum master can delete a sprint");

            // Cheks if user tries to delete last sprint and prohibits deletion of the last sprint
            IEnumerable<Sprint> sprints = await _sprintRepository.GetAllByProjectIdAsync(existingSprint.ProjectId);
            if (sprints.ToList().Count < 2)
                return new SprintResponse("Only one sprint exist. Cannot delete");

            try
            {
                // Tries to delete sprint in database and returns response to controller
                await _sprintRepository.DeleteAsync(existingSprint);
                var sprintDTO = _mapper.Map<Sprint, SprintDto>(existingSprint);
                return new SprintResponse(sprintDTO);
            }
            catch (Exception)
            {
                return new SprintResponse($"An error occurred when deleting the sprint");
            }
        }

    }
}
