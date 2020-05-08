using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Data.DTOs;

namespace WebApi.BLs.Communication
{
	public class UserResponse : BaseResponse
	{
		public UserDto UserDto { get; }


		private UserResponse(bool success, string message, UserDto userDto) : base(success, message)
		{
			UserDto = userDto;
		}

		public UserResponse(UserDto userDto) : this(true, string.Empty, userDto)
		{ }

		public UserResponse(string message) : this(false, message, null)
		{ }
	}
}
