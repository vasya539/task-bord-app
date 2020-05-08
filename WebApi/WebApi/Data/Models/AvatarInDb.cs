using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data.Models
{
	[Table("Avatars")]
	public class AvatarInDb
	{
		public string UserId { get; set; }
		public byte[] Avatar { get; set; }

		public User User { get; set; }
	}
}
