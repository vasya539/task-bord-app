using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Data.DTOs;
using AutoMapper;
using WebApi.BLs.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Extensions.ProjectRoleExtension;
using WebApi.Exceptions;
using WebApi.BLs.Communication;

namespace WebApi.BLs
{
    /// <summary>
    /// Class for project business logic. Implements default interface for project business logic.
    /// </summary>
    public class ProjectBl : IProjectBl
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ISprintRepository _sprintRepository;
        private readonly IMapper _mapper;
        private readonly IProjectUserRepository _projectUserRepository;


        /// <summary>
        /// Constructor for initializing ProjectRepository, Mapper and other additional repositories.
        /// </summary>
        /// <param name="projectRepository">Repository for project</param>
        /// <param name="mapper">Auto-mapper</param>
        /// <param name="projectUserRepository">Repository for project-users</param>
        /// <param name="sprintRepository">Repository for sprints</param>
        public ProjectBl(IProjectRepository projectRepository, IMapper mapper, IProjectUserRepository projectUserRepository, ISprintRepository sprintRepository)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
            _projectUserRepository = projectUserRepository;
            _sprintRepository = sprintRepository;
        }

        #region CRUD-Operations

        /// <summary>
        /// Use repository to get all projects from database
        /// </summary>
        /// <returns>List of ProjectDto to controller</returns>
        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            IEnumerable<Project> project = await _projectRepository.GetAllAsync();
            IEnumerable<ProjectDto> projectDto = _mapper.Map<IEnumerable<Project>, IEnumerable<ProjectDto>>(project);

            return projectDto;
        }

        /// <summary>
        /// Search project with param Id in database, and map it.
        /// </summary>
        /// <param name="id">Id of the project to be returned to client</param>
        /// <returns>Required project to controller</returns>
        public async Task<ProjectDto> GetByIdAsync(int projectId, string userId)
        {
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, projectId);
            if (!this.CanAccessProject(role))
                throw new ForbiddenResponseException("You do not have access");

            Project project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new NotFoundResponseException();
            }
            ProjectDto projectDto = _mapper.Map<Project ,ProjectDto>(project);

            return projectDto;
        }

        /// <summary>
        /// Map ProjectDto => Project and use repository to create a new project.
        /// All can create project.
        /// </summary>
        /// <param name="dto"> Project to create</param>
        /// <param name="userId">User's id</param>
        /// <returns>Project response with message</returns> 
        public async Task<ProjectResponse> CreateAsync(ProjectDto dto, string userId)
        {
            Project project = _mapper.Map<ProjectDto, Project>(dto);
            await _projectRepository.CreateAsync(project);

            // Add relation project to user
            var pu = new ProjectUser();
            pu.UserId = userId;
            pu.ProjectId = project.Id;
            pu.UserRoleId = AppUserRole.Owner.Id;
            await _projectUserRepository.CreateRecordAsync(pu);

            // Intitial first sprint for project
            Sprint initialSprint = GetInitialSprint(project.Id);
            await _sprintRepository.CreateAsync(initialSprint);

            var projectDto = _mapper.Map<Project, ProjectDto>(project);
            return new ProjectResponse(projectDto);
        }

        /// <summary>
        /// Get projectDto from controller, map it , get role of user, and get existing project from repository. After that check access to make an update.
        /// Only owner of project can update. 
        /// </summary>
        /// <param name="projectDto">Project to update</param>
        /// <param name="userId">User id</param>
        /// <returns>Project response with message</returns>
        /// <exception cref="ForbiddenResponseException">IF user don't have access to do this update</exception>
        public async Task<ProjectResponse> UpdateAsync(ProjectDto projectDto, string userId)
        {
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, projectDto.Id);
            if (!this.CanChangeProject(role))
                throw new ForbiddenResponseException("Only owner can update a project");


            Project existingProject = await _projectRepository.GetByIdAsync(projectDto.Id);
            if (existingProject == null)
            {
                return new ProjectResponse("Project not found");
            }

            existingProject.Name = projectDto.Name;
            existingProject.Description = projectDto.Description;

            await _projectRepository.UpdateAsync(existingProject);
            var existingProjectDto = _mapper.Map<Project, ProjectDto>(existingProject);

            return new ProjectResponse(existingProjectDto);
        }

        /// <summary>
        ///  Get project Id from controller, get project from repository and if project exist - delete id.
        ///  Only Owner can delete.
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <param name="userId">User id</param>
        /// <returns>Project response with message</returns>
        /// <exception cref="ForbiddenResponseException"> IF user don't have access to delete project</exception>
        public async Task<ProjectResponse> DeleteAsync(int projectId, string userId)
        {
            AppUserRole role = await _projectUserRepository.GetRoleOfMember(userId, projectId);
            if (!this.CanChangeProject(role))
                throw new ForbiddenResponseException("Only owner can delete a project");

            Project existingProject = await _projectRepository.GetByIdAsync(projectId);
            if (existingProject == null)
            {
                return new ProjectResponse("Project not found");
            }
            try
            {
                await _projectRepository.DeleteAsync(projectId);
                ProjectDto projectDto = _mapper.Map<Project, ProjectDto>(existingProject);
                return new ProjectResponse(projectDto);
            }
            catch (Exception)
            {
                return new ProjectResponse($"An error occurred when deleting the project");
            }
        }

        #endregion CRUD-Operations


        #region Logic functions

        /// <summary>
        /// Create first empty sprint for new project
        /// </summary>
        /// <param name="projectId">New project id</param>
        private Sprint GetInitialSprint(int projectId)
        {
            DateTime now = DateTime.Now;
            DateTime begins = new DateTime(now.Year, now.Month, now.Day);
            DateTime ends = begins.AddDays(28);
            Sprint sprint = new Sprint()
            {
                Id = 0,
                Name = "Sprint 1",
                Description = "Initial Sprint",
                ProjectId = projectId,
                StartDate = begins,
                EndDate = ends
            };
            return sprint;
        }

        #endregion Logic functions
    }
}
