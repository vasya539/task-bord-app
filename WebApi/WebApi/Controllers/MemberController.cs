using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using WebApi.Data.Models;
using WebApi.Data.DTOs;
using WebApi.BLs.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
	[Authorize(Roles = "User")]
	[Route("api/members")]
	[ApiController]
	public class MemberController : LoginedUserControllerBase
	{
		private readonly IMemberBl _memberBl;

		public MemberController(IMemberBl memberBl)
		{
			_memberBl = memberBl;
		}

		[HttpGet]
		public ActionResult<IEnumerable<AppUserRole>> GetAllUserRoles()
		{
			return Ok(_memberBl.GetAvailableUserRoles());
		}

		[HttpGet("/api/{projectId}/members")]
		public async Task<ActionResult<IEnumerable<ProjectMemberDto>>> GetMembersOfProjectAsync(int projectId)
		{
			return Ok(await _memberBl.GetAllMembersOfProjectAsync(UserId, projectId));
		}

		[HttpPost("/api/{projectId}/members")]
		public async Task<ActionResult> AddMemberAsync(int projectId, [FromBody] ProjectMemberDto member)
		{
			await _memberBl.AddMemberAsync(UserId, projectId, member.Id, member.Role);
			return Ok();
		}

		[HttpPut("/api/{projectId}/members/{userId}")]
		public async Task<ActionResult> ChangeMemberRoleAsync(int projectId, string userId, AppUserRoleDto role)
		{
			await _memberBl.ChangeUserRoleAsync(UserId, projectId, userId, role);
			return Ok();
		}

		[HttpDelete("/api/{projectId}/members/{userId}")]
		public async Task<ActionResult> RemoveMemberAsync(int projectId, string userId)
		{
			await _memberBl.RemoveMemberAsync(UserId, projectId, userId);
			return NoContent();
		}
	}
}
