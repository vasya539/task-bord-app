using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.DTOs
{
	public class UpdateUserDto : SaveUserDto
	{
		public string Id { get; set; }
	}
}
