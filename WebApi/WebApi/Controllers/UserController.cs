using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using WebApi.Data.DTOs;
using WebApi.BLs.Interfaces;


namespace WebApi.Controllers
{
	[Authorize(Roles = "User")]
	[Route("api/users")]
	[ApiController]
	public class UserController : LoginedUserControllerBase
	{
		private readonly IUserBl _userBl;

		public UserController(IUserBl userBl)
		{
			_userBl = userBl;
		}


		[Authorize(Roles = "User")]
		[HttpGet("/api/users-all")]
		public async Task<IEnumerable<UserDto>> GetAllAsync()
		{
			IEnumerable<UserDto> users = await _userBl.GetAllAsync();
			return users;
		}

		[HttpGet]
		public async Task<ActionResult<UserDto>> GetMeAsync()
		{
			UserDto res = await _userBl.GetByIdAsync(UserId);
			return Ok(res);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserDto>> GetByIdAsync([FromRoute] string id)
		{
			UserDto res = await _userBl.GetByIdAsync(id);
			return Ok(res);
		}

		[HttpGet("detailed/{id}")]
		public async Task<ActionResult<UserDto>> GetDetailedByIdAsync([FromRoute] string id)
		{
			UserDto res = await _userBl.GetDetailedByIdAsync(id);
			return Ok(res);
		}

		[HttpGet("by-username/{userName}")]
		public async Task<ActionResult<UserDto>> GetByUserNameAsync([FromRoute] string userName, [FromQuery] bool detailed=true)
		{
			UserDto res = await _userBl.GetByUserNameAsync(userName, detailed);
			return Ok(res);
		}

		[HttpGet("{id}/projects")]
		public async Task<ActionResult<IEnumerable<SimpleProjectDto>>> GetProjectsAsync([FromRoute] string id)
		{
			return Ok(await _userBl.GetProjectsOfUser(UserId, id));
		}

		[HttpGet("search")]
		public async Task<ActionResult<FoundUsersPageDto>> FindAsync([FromQuery] string template, [FromQuery] int page=1)
		{
			var res = await _userBl.Find(template, page);
			return Ok(res);
		}

		[HttpPut]
		public async Task<ActionResult> PutAsync([FromBody] UserDto user)
		{
			await _userBl.UpdateAsync(UserId, user);
			return Ok();
		}

		[HttpDelete]
		public async Task<ActionResult> DeleteAsync()
		{
			await _userBl.DeleteAsync(UserId);
			return NoContent();
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("{userId}")]
		public async Task<ActionResult> Delete([FromRoute] string userId)
		{
			await _userBl.DeleteAsync(userId);
			return NoContent();
		}
	}
}