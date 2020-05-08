using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;

using WebApi.Controllers;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using Microsoft.AspNetCore.Http;

namespace WebApiTests.Controllers
{
	public class AvatarControllerTests
	{
		private readonly Mock<IAvatarBl> _blMock;
		private readonly AvatarController _avatarController;

		public AvatarControllerTests()
		{
			_blMock = new Mock<IAvatarBl>();
			_avatarController = new AvatarController(_blMock.Object);
		}


		[Fact]
		public void GetAvatarAsync_GetAvatarAsync()
		{
			var res = _avatarController.GetAvatarAsync(It.IsAny<string>()).Result;

			_blMock.Verify(b => b.GetAvatarAsync(It.IsAny<string>()));
		}

		[Fact]
		public void PostAvatarAsync_UpdateAvatarAsync()
		{
			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream());

			var res = _avatarController.PostAvatarAsync(fileMock.Object).Result;

			_blMock.Verify(b => b.UpdateAvatarAsync(It.IsAny<string>(), It.IsAny<System.IO.Stream>()));
		}

		[Fact]
		public void GetAllUserRoles_CallsGetAvailableUserRoles()
		{
			var res = _avatarController.DeleteAvatarAsync().Result;

			_blMock.Verify(b => b.DeleteAvatarAsync(It.IsAny<string>()));
		}
	}
}
