using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;

using WebApi.Controllers;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTests.Controllers
{
	public class UserControllerTests
	{
		private readonly Mock<IUserBl> _blMock;
		private readonly UserController _userController;

		public UserControllerTests()
		{
			_blMock = new Mock<IUserBl>();
			_userController = new UserController(_blMock.Object);
		}


		[Fact]
		public void GetMeAsync_CallsGetById()
		{
			_blMock.Setup(b => b.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new UserDto());

			var res = _userController.GetMeAsync().Result;

			_blMock.Verify(b => b.GetByIdAsync(It.IsAny<string>()));
		}

		[Fact]
		public void GetByIdAsync_CallsGetById()
		{
			_blMock.Setup(b => b.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new UserDto());

			var res = _userController.GetByIdAsync(It.IsAny<string>()).Result;

			_blMock.Verify(b => b.GetByIdAsync(It.IsAny<string>()));
		}

		[Fact]
		public void GetDetailedByIdAsync_GetDetailedByIdAsync()
		{
			_blMock.Setup(b => b.GetDetailedByIdAsync(It.IsAny<string>())).ReturnsAsync(new UserDto());

			var res = _userController.GetDetailedByIdAsync(It.IsAny<string>()).Result;

			_blMock.Verify(b => b.GetDetailedByIdAsync(It.IsAny<string>()));
		}


		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void GetByUserNameAsync_CallsGetByUserNameAsync(bool isDetailed)
		{
			_blMock.Setup(b => b.GetByUserNameAsync(It.IsAny<string>(), isDetailed)).ReturnsAsync(new UserDto());

			var res = _userController.GetByUserNameAsync(It.IsAny<string>(), isDetailed).Result;

			_blMock.Verify(b => b.GetByUserNameAsync(It.IsAny<string>(), isDetailed));
		}

		[Fact]
		public void GetProjectsAsync_CallsGetProjectsOfUser()
		{
			//_blMock.Setup(b => b.GetProjectsOfUser(It.IsAny<string>())).ReturnsAsync(new UserDto());
			string reqUserId = "some-user";

			var res = _userController.GetProjectsAsync(reqUserId).Result;

			_blMock.Verify(b => b.GetProjectsOfUser(It.IsAny<string>(), reqUserId));
		}

		[Fact]
		public void FindAsync_CallsFind()
		{
			var res = _userController.FindAsync(It.IsAny<string>(), It.IsAny<int>()).Result;

			_blMock.Verify(b => b.Find(It.IsAny<string>(), It.IsAny<int>()));
		}

		[Fact]
		public void PutAsync_CallsUpdateAsync()
		{
			var res = _userController.PutAsync(It.IsAny<UserDto>()).Result;

			_blMock.Verify(b => b.UpdateAsync(It.IsAny<string>(), It.IsAny<UserDto>()));
		}

		[Fact]
		public void DeleteAsync_CallsDeleteAsync()
		{
			var res = _userController.DeleteAsync().Result;

			_blMock.Verify(b => b.DeleteAsync(It.IsAny<string>()));
		}


	}
}
