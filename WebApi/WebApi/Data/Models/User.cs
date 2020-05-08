using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace WebApi.Data.Models
{
	[Table("AspNetUsers")]
	public class User: IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }

		[StringLength(1000)]
		public string Info { get; set; }

		/// <summary>
		/// Something like hashcode, hashsum, version of avatar
		/// Value that should be added at the end of request to AvatarController to ignore cached image.
		/// When avatar image has been changed this value should be updated
		/// if brouser has cached image for 111 (.../avatars/123.jpg?111) and avatar updated (new tail is 222
		/// then request .../avatars/123.jpg?222 will be sended to backend)
		/// </summary>
		public int? AvatarTail { get; set; }



		// Navigation properties

		public ICollection<ProjectUser> ProjectsUsers { get; set; }

		public List<Item> Items { get; set; }
		public AvatarInDb Avatar { get; set; }
	}
}
