using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.BLs.Interfaces;

namespace WebApi.Controllers
{
	[Route("api/avatars")]
	[ApiController]
	[Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
	public class AvatarController : LoginedUserControllerBase
	{
		private readonly IAvatarBl _avatarBl;

		public AvatarController(IAvatarBl avatarBl)
		{
			_avatarBl = avatarBl;
		}

		[ResponseCache(Duration = 24*60*60)] // Set cache duration to 24hours
		[HttpGet("{userId}.jpg")]
		public async Task<IActionResult> GetAvatarAsync([FromRoute] string userId)
		{
			return await _avatarBl.GetAvatarAsync(userId);
		}

		[Authorize(Roles = "User")]
		[HttpPost]
		public async Task<ActionResult> PostAvatarAsync([FromForm(Name = "avatar")] IFormFile file)
		{
			using (var input = file.OpenReadStream())
			{
				await _avatarBl.UpdateAvatarAsync(UserId, input);
			}

			return Ok();
		}
		
		[Authorize(Roles = "User")]
		[HttpDelete]
		public async Task<ActionResult> DeleteAvatarAsync()
		{
			await _avatarBl.DeleteAvatarAsync(UserId);
			return NoContent();
		}
	}
}