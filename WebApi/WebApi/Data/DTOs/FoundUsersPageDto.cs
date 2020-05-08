using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.DTOs
{
	public class FoundUsersPageDto
	{
		public IEnumerable<UserDto> Users { get; set; }
		public int PageNumber { get; set; }
		public bool HasNext { get; set; }
	}
}
