using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

using WebApi.Data.Models;
using WebApi.Data.DTOs;
using WebApi.BLs.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Interfaces.IRepositories; // TODO: update interface's placement
using WebApi.Exceptions;
using WebApi.Extensions.AppUserRolesExtensions.ProjectMemberManagementRolesExtensions;

namespace WebApi.BLs
{
	public class MemberBl : IMemberBl
	{
		private readonly IProjectUserRepository _puRepo;
		private readonly IProjectRepository _projectRepo;
		private readonly IUserRepository _userRepo;
		private readonly IMapper _mapper;

		public MemberBl(
				IProjectUserRepository puRepository,
				IProjectRepository projectRepo,
				IMapper mapper,
				IUserRepository userRepo
				)
		{
			_puRepo = puRepository;
			_projectRepo = projectRepo;
			_mapper = mapper;
			_userRepo = userRepo;
		}


		private async Task<AppUserRole> GetRoleIfMember(string userId, int projectId)
		{
			AppUserRole role = await _puRepo.GetRoleOfMember(userId, projectId);
			if(role.IsNone())
				throw new NotFoundResponseException("Project not found or you don't have permissions.");

			return role;
		}

		private ProjectMemberDto MakeProjectMemberDto(User u, AppUserRole role)
		{
			ProjectMemberDto dto = _mapper.Map<User, ProjectMemberDto>(u);
			dto.Role = _mapper.Map<AppUserRole, AppUserRoleDto>(role);

			return dto;
		}


		public IEnumerable<AppUserRoleDto> GetAvailableUserRoles()
		{
			return _mapper.Map<IEnumerable<AppUserRole>, IEnumerable<AppUserRoleDto>>(AppUserRole.GetAllRoles());
		}

		public async Task<IEnumerable<ProjectMemberDto>> GetAllMembersOfProjectAsync(string senderUserId, int projectId)
		{
			AppUserRole senderRole = await GetRoleIfMember(senderUserId, projectId);

			if (!senderRole.CanViewListOfMembers())
				throw new ForbiddenResponseException("You cannot get members of this project");

			var members = await _puRepo.GetMembersOfProject(projectId);
			var res = new List<ProjectMemberDto>();

			foreach (User u in members)
			{
				AppUserRole role = await _puRepo.GetRoleOfMember(u.Id, projectId);

				ProjectMemberDto dto = MakeProjectMemberDto(u, role);
				res.Add(dto);
			}

			return res;
		}

		public async Task<ProjectMemberDto> GetMemberOfProjectAsync(string senderUserId, int projectId, string userId)
		{
			AppUserRole senderRole = await GetRoleIfMember(senderUserId, projectId);

			if (!senderRole.CanViewListOfMembers())
				throw new ForbiddenResponseException("You cannot get list of members of this project");

			AppUserRole role = await _puRepo.GetRoleOfMember(userId, projectId);
			if (!role.IsNone())
			{
				User u = await _userRepo.GetByIdAsync(userId);
				ProjectMemberDto dto = MakeProjectMemberDto(u, role);
				return dto;
			}
			else
				throw new NotFoundResponseException($"No member with id {userId} in project with id {projectId}");
		}

		public async Task<AppUserRoleDto> GetMyRoleInProjectAsync(string userId, int projectId)
		{
			AppUserRole role = await GetRoleIfMember(userId, projectId);

			if (!role.CanViewListOfMembers())
				throw new ForbiddenResponseException("You have not access to this project.");

			return _mapper.Map<AppUserRole, AppUserRoleDto>(role);
		}

		public async Task AddMemberAsync(string senderUserId, int projectId, string userId, AppUserRoleDto roleDto)
		{
			AppUserRole senderRole = await GetRoleIfMember(senderUserId, projectId);

			if (!senderRole.CanAddNewMember())
				throw new ForbiddenResponseException("You haven't permissions to add members to this project");

			if (!await _userRepo.ExistsWithId(userId))
				throw new NotFoundResponseException("Cannot find user to add into project.");

			if (!((await _puRepo.GetRoleOfMember(userId, projectId)).IsNone()))
				throw new BadRequestResponseException("This user already exists in project.");

			AppUserRole role = _mapper.Map<AppUserRoleDto, AppUserRole>(roleDto);

			if (role.IsNone())
				throw new BadRequestResponseException("Given role is bad.");

			if (role.IsOwner())
				throw new BadRequestResponseException("Cannot add Owner to project.");

			if (role.IsScrumMaster() && await _puRepo.DoesExistScrumMasterInProjectAsync(projectId))
				throw new BadRequestResponseException("Scrum Master already exists in project.");

			var pu = new ProjectUser()
			{
				ProjectId = projectId,
				UserId = userId,
				UserRoleId = role.Id
			};
			await _puRepo.CreateRecordAsync(pu);
		}

		public async Task ChangeUserRoleAsync(string senderUserId, int projectId, string userId, AppUserRoleDto newRoleDto)
		{
			AppUserRole senderRole = await GetRoleIfMember(senderUserId, projectId);

			if (!senderRole.HasPermissionsToChangeRoleOfMember())
				throw new ForbiddenResponseException("You haven't permissions to change roles of members in this project");

			AppUserRole currRole = await _puRepo.GetRoleOfMember(userId, projectId);
			AppUserRole newRole = _mapper.Map<AppUserRoleDto, AppUserRole>(newRoleDto);

			if (!senderRole.CanChangeRoleOfMember(currRole, newRole))
				throw new ForbiddenResponseException($"You cannot change role from {currRole.Name} to {newRole.Name}.");

			if (newRole.IsScrumMaster() && await _puRepo.DoesExistScrumMasterInProjectAsync(projectId))
				throw new ForbiddenResponseException("Only one Scrum Master can be on project");

			ProjectUser pu = new ProjectUser()
			{
				ProjectId = projectId,
				UserId = userId,
				UserRoleId = newRole.Id
			};
			await _puRepo.UpdateRecordAsync(pu);
		}

		public async Task RemoveMemberAsync(string senderUserId, int projectId, string userId)
		{
			AppUserRole senderRole = await GetRoleIfMember(senderUserId, projectId);

			if(senderUserId == userId)
			{
				if (!senderRole.CanRemoveItself())
					throw new ForbiddenResponseException("You cannot leave project.");
			} else
			{
				if (!senderRole.CanRemoveOtherMember())
					throw new ForbiddenResponseException("You cannot remove other members.");
			}

			if (!await _puRepo.DoesExistMemberOfProject(userId, projectId))
				throw new NotFoundResponseException("No member with this id in project");

			await _puRepo.DeleteRecordAsync(userId, projectId);
		}

	}
}
