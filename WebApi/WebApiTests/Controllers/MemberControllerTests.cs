using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;

using WebApi.Controllers;
using WebApi.BLs.Interfaces;
using WebApi.Data.DTOs;

namespace WebApiTests.Controllers
{
	public class MemberControllerTests
	{
		private readonly Mock<IMemberBl> _blMock;
		private readonly MemberController _memberController;

		public MemberControllerTests()
		{
			_blMock = new Mock<IMemberBl>();
			_memberController = new MemberController(_blMock.Object);
		}


		[Fact]
		public void GetAllUserRoles_CallsGetAvailableUserRoles()
		{
			var res = _memberController.GetAllUserRoles().Result;

			_blMock.Verify(b => b.GetAvailableUserRoles());
		}

		[Fact]
		public void GetMembersOfProjectAsync_CallsGetMemberOfProjectAsync()
		{
			var res = _memberController.GetMembersOfProjectAsync(It.IsAny<int>()).Result;

			_blMock.Verify(b => b.GetAllMembersOfProjectAsync(It.IsAny<string>(), It.IsAny<int>()));
		}

		[Fact]
		public void AddMemberAsync_CallsAddMemberAsync()
		{
			var member = new ProjectMemberDto() { Role = new AppUserRoleDto { Id = 1 } };
			var res = _memberController.AddMemberAsync(It.IsAny<int>(), member).Result;

			_blMock.Verify(b => b.AddMemberAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), member.Role));
		}

		[Fact]
		public void ChangeMemberRoleAsync_CallsChangeUserRoleAsync()
		{
			var role = new AppUserRoleDto { Id = 1 };
			var res = _memberController.ChangeMemberRoleAsync(It.IsAny<int>(), It.IsAny<string>(), role).Result;

			_blMock.Verify(b => b.ChangeUserRoleAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), role));
		}

		[Fact]
		public void RemoveMemberAsync_RemoveMemberAsync()
		{
			var role = new AppUserRoleDto { Id = 1 };
			var res = _memberController.RemoveMemberAsync(It.IsAny<int>(), It.IsAny<string>()).Result;

			_blMock.Verify(b => b.RemoveMemberAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
		}
	}
}
