using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.DTOs
{
	public class UserDto
	{
		public string Id { get; set; }
		public string UserName { get; set; }

		[MaxLength(256)]
		[EmailAddress]
		public string Email { get; set; }

		[MaxLength(20)]
		public string FirstName { get; set; }

		[MaxLength(20)]
		public string LastName { get; set; }

		[StringLength(1000)]
		public string Info { get; set; }

		public int? AvatarTail { get; set; }

		//public List<ProjectDto> Projects { get; set; } = null;
	}
}
