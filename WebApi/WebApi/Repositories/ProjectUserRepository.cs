using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
{
	public class ProjectUserRepository : IProjectUserRepository
	{
		private readonly AppDbContext _context;

		public ProjectUserRepository(AppDbContext context)
		{
			_context = context;
		}


		public async Task<ProjectUser> GetRecordAsync(string userId, int projectId)
		{
			return await _context.ProjectUsers.FindAsync(new object[]{ userId, projectId });
		}

		public async Task CreateRecordAsync(ProjectUser pu)
		{
			await _context.ProjectUsers.AddAsync(pu);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateRecordAsync(ProjectUser pu)
		{
			ProjectUser rec = await GetRecordAsync(pu.UserId, pu.ProjectId);
			if(rec != null)
			{
				rec.UserRoleId = pu.UserRoleId;
				_context.ProjectUsers.Update(rec);
				await _context.SaveChangesAsync();
			}
		}

		public async Task DeleteRecordAsync(ProjectUser pu)
		{
			_context.ProjectUsers.Remove(pu);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteRecordAsync(string userId, int projectId)
		{
			ProjectUser pu = await GetRecordAsync(userId, projectId);
			if(pu != null)
				await DeleteRecordAsync(pu);
		}



		public async Task<IEnumerable<Project>> GetProjectsOfUser(string userId)
		{
			// join tables Users, Projects using ProjectsUsers and select projects for user with userId
			List<Project> projs =
				await _context.Users
							.Join(_context.ProjectUsers, u => u.Id, pu => pu.UserId, (u, pu) => new { pu.UserId, pu.ProjectId })
							.Join(_context.Projects, pp => pp.ProjectId, p => p.Id, (pp, p) => new { pp.UserId, p.Id, p.Name, p.Description })
					.Where(x => x.UserId == userId)
					.Select(x => new Project
					{
						Id = x.Id,
						Name = x.Name,
						Description = x.Description
					}).ToListAsync();

			return projs;
		}

		public async Task<IEnumerable<User>> GetMembersOfProject(int projectId)
		{
			// join tables Users, Projects using ProjectsUsers and select users for project with projectId
			List<User> users =
				await _context.Projects
						.Join(_context.ProjectUsers, p => p.Id, pu => pu.ProjectId, (p, pu) => new { p.Id, pu.UserId })
						.Join(_context.Users, pp => pp.UserId, u => u.Id, (pp, u) => new { pp.Id, u })
					.Where(x => x.Id == projectId)
					.Select(x => x.u)
					.Select(UserRepository.UserSelector)
					.ToListAsync();

			return users;
		}

		public async Task<User> GetOwnerOfProject(int projectId)
		{
			string userId = await
				_context.ProjectUsers
				.Where(pu => pu.ProjectId == projectId && pu.UserRoleId == AppUserRole.Owner.Id)
				.Select(pu => pu.UserId)
				.SingleOrDefaultAsync();

			return await _context.Users.Where(u => u.Id == userId).Select(UserRepository.UserSelector).SingleOrDefaultAsync();

		}

		public async Task<AppUserRole> GetRoleOfMember(string userId, int projectId)
		{
			ProjectUser pu = await GetRecordAsync(userId, projectId);
			if(pu != null)
				return AppUserRole.GetUserRoleById(pu.UserRoleId);
			else
				return AppUserRole.None;
		}

		public async Task<bool> DoesExistScrumMasterInProjectAsync(int projectId)
		{
			return await _context.ProjectUsers
					.AnyAsync(pu => pu.ProjectId == projectId && pu.UserRoleId == AppUserRole.ScrumMaster.Id);
		}

		public async Task<bool> DoesExistMemberOfProject(string userId, int projectId)
		{
			return await _context.ProjectUsers
					.AnyAsync(pu => pu.UserId == userId && pu.ProjectId == projectId);
		}
	}
}
