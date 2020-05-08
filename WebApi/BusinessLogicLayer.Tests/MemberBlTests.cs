using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Xunit;
using Moq;

using WebApi.Exceptions;
using WebApi.BLs.Interfaces;
using WebApi.BLs;
using WebApi.Repositories.Interfaces;
using WebApi.Interfaces.IRepositories;
using WebApi.Data.Models;
using WebApi.Data.DTOs;

namespace BusinessLogicLayer.Tests
{
	public class MemberBlTests
	{
		private readonly IMapper _mapper;

		private readonly Mock<IUserRepository> _mur;
		private readonly Mock<IProjectRepository> _mpr;
		private readonly Mock<IProjectUserRepository> _mpur;

		public MemberBlTests()
		{
			var cfg = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
			_mapper = cfg.CreateMapper();

			_mur = new Mock<IUserRepository>();
			_mpr = new Mock<IProjectRepository>();
			_mpur = new Mock<IProjectUserRepository>();

		}

		private IMemberBl getMemberBl()
		{
			return new MemberBl(_mpur.Object, _mpr.Object, _mapper, _mur.Object);
		}

		private AppUserRoleDto getObserverDto() => new AppUserRoleDto { Id = AppUserRole.Observer.Id };




		[Theory]
		[InlineData("user-that-owner")]
		[InlineData("user-that-master")]
		[InlineData("user-that-dev")]
		[InlineData("user-that-observer")]
		public void CheckThatEveryMemberOfProjectCanViewGetOfMembers(string callerId)
		{
			_mpur.Setup(r => r.GetRoleOfMember("user-that-owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			IMemberBl mbl = new MemberBl(_mpur.Object, _mpr.Object, _mapper, _mur.Object);

			var members = mbl.GetAllMembersOfProjectAsync(callerId, It.IsAny<int>()).Result;

			_mpur.Verify(r => r.GetMembersOfProject(It.IsAny<int>()), Times.Once);
		}

		[Fact]
		public void CheckThatNoMemberOfProjectCannotGetListOfMembers()
		{
			_mpur.Setup(r => r.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			IMemberBl mbl = new MemberBl(_mpur.Object, _mpr.Object, _mapper, _mur.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await mbl.GetAllMembersOfProjectAsync(It.IsAny<string>(), It.IsAny<int>())).Wait();

			_mpur.Verify(r => r.GetMembersOfProject(It.IsAny<int>()), Times.Never);
		}

		[Theory]
		[InlineData("user-that-owner")]
		[InlineData("user-that-master")]
		[InlineData("user-that-dev")]
		[InlineData("user-that-observer")]
		public void CheckThatEveryMemberCanViewAnyOtherMember(string callerId)
		{
			_mpur.Setup(r => r.GetRoleOfMember("user-that-owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			string targetMemberId = "user-x";
			_mur.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new User { Id = targetMemberId });
			_mpur.Setup(r => r.GetRoleOfMember(targetMemberId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			var bl = new MemberBl(_mpur.Object, _mpr.Object, _mapper, _mur.Object);

			var memb = bl.GetMemberOfProjectAsync(callerId, It.IsAny<int>(), targetMemberId).Result;

			_mpur.Verify(r => r.GetRoleOfMember(callerId, It.IsAny<int>()), Times.Once);
			_mpur.Verify(r => r.GetRoleOfMember(targetMemberId, It.IsAny<int>()), Times.Once);
			_mur.Verify(r => r.GetByIdAsync(targetMemberId), Times.Once);
		}

		[Fact]
		public void CheckThatNoMemberCanNotViewAnyOtherMember()
		{
			_mpur.Setup(r => r.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			var bl = new MemberBl(_mpur.Object, _mpr.Object, _mapper, _mur.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.GetMemberOfProjectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Wait();
		}

		[Fact]
		public void CheckThatNotFoundWillBeReturnedIfNoGivenMemberInProject()
		{
			_mpur.Setup(r => r.GetRoleOfMember("user-1", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);
			_mpur.Setup(r => r.GetRoleOfMember("user-2", It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			var bl = new MemberBl(_mpur.Object, _mpr.Object, _mapper, _mur.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.GetMemberOfProjectAsync("user-1", It.IsAny<int>(), "user-2")).Wait();
		}

		[Fact]
		public void CheckThatOwnerCanAddMemberToProject()
		{
			string userOwnerId = "user-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(userOwnerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mur.Setup(r => r.ExistsWithId(userId)).ReturnsAsync(true);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);


			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.Observer.Id };

			bl.AddMemberAsync(userOwnerId, It.IsAny<int>(), userId, roleDto).Wait();

			_mpur.Verify(r => r.CreateRecordAsync(It.IsAny<ProjectUser>()), Times.Once);
		}

		[Fact]
		public void CheckThatNotFoundWillBeReturnedIfNoCallerInProject()
		{
			_mpur.Setup(r => r.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.None);
			var bl = getMemberBl();

			Assert.ThrowsAsync<NotFoundResponseException>(async ()
					=> await bl.AddMemberAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), getObserverDto())).Wait();
		}

		[Theory]
		[InlineData("user-that-master")]
		[InlineData("user-that-dev")]
		[InlineData("user-that-observer")]
		public void CheckThatNonOwnerCanNotAddMember(string callerId)
		{
			_mpur.Setup(r => r.GetRoleOfMember("user-that-master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			var bl = getMemberBl();

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.AddMemberAsync(callerId, It.IsAny<int>(), It.IsAny<string>(), getObserverDto())).Wait();
		}

		[Fact]
		public void CheckThatCannotAddMemberIfUserDoesNotExist()
		{
			string callerId = "user-owner", userId = "user-none";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mur.Setup(r => r.ExistsWithId(userId)).ReturnsAsync(false);

			var bl = getMemberBl();

			Assert.ThrowsAsync<NotFoundResponseException>(async ()
					=> await bl.AddMemberAsync(callerId, It.IsAny<int>(), userId, getObserverDto())).Wait();
		}

		[Fact]
		public void CheckThatCannotAddMemberIfItAlreadyIsMember()
		{
			string callerId = "user-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mur.Setup(r => r.ExistsWithId(userId)).ReturnsAsync(true);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			var bl = getMemberBl();

			Assert.ThrowsAsync<BadRequestResponseException>(async ()
				=> await bl.AddMemberAsync(callerId, It.IsAny<int>(), userId, getObserverDto())).Wait();
		}

		[Fact]
		public void CheckThatCannotAddMemberWithBadRole()
		{
			string callerId = "user-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mur.Setup(r => r.ExistsWithId(userId)).ReturnsAsync(true);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = -1 };

			Assert.ThrowsAsync<BadRequestResponseException>(async ()
				=> await bl.AddMemberAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatCannotAddMemberWithOwnerRole()
		{
			string callerId = "user-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mur.Setup(r => r.ExistsWithId(userId)).ReturnsAsync(true);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.Owner.Id };

			Assert.ThrowsAsync<BadRequestResponseException>(async ()
				=> await bl.AddMemberAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatOwnerCanAddScrumMasterIfItIsNotAlreadyInProject()
		{
			string callerId = "user-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mur.Setup(r => r.ExistsWithId(userId)).ReturnsAsync(true);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);
			_mpur.Setup(r => r.DoesExistScrumMasterInProjectAsync(It.IsAny<int>())).ReturnsAsync(false);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.ScrumMaster.Id };

			bl.AddMemberAsync(callerId, It.IsAny<int>(), userId, roleDto).Wait();

			_mpur.Verify(r => r.CreateRecordAsync(It.IsAny<ProjectUser>()));
		}

		[Fact]
		public void CheckThatCannotAddScrumMasterIfItIsAlreadyInProject()
		{
			string callerId = "user-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mur.Setup(r => r.ExistsWithId(userId)).ReturnsAsync(true);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);
			_mpur.Setup(r => r.DoesExistScrumMasterInProjectAsync(It.IsAny<int>())).ReturnsAsync(true);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.ScrumMaster.Id };

			Assert.ThrowsAsync<BadRequestResponseException>(async ()
				=> await bl.AddMemberAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}



		[Fact]
		public void CheckThatNotFoundWillBeReturnedIfNoMemberIsTryingChangeRoles()
		{
			_mpur.Setup(r => r.GetRoleOfMember(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			var bl = getMemberBl();

			Assert.ThrowsAsync<NotFoundResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), getObserverDto())).Wait();
		}


		[Theory]
		[InlineData("user-that-dev")]
		[InlineData("user-that-observer")]
		public void CheckThatAllMembersWithoutOwnerAndScrumMasterCanNotChangeRoles(string callerId)
		{
			_mpur.Setup(r => r.GetRoleOfMember("user-that-dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			var bl = getMemberBl();

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), "user-x", getObserverDto())).Wait();
		}

		[Fact]
		public void CheckThatOwnerCanChangeRolesInGeneralCase()
		{
			string callerId = "user-that-owner", userId = "user-that-dev";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

			var bl = getMemberBl();

			bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, getObserverDto()).Wait();

			_mpur.Verify(r => r.UpdateRecordAsync(It.IsAny<ProjectUser>()));
		}

		[Fact]
		public void CheckThatScrumMasterCanChangeRolesInGeneralCase()
		{
			string callerId = "user-that-master", userId = "user-that-dev";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

			var bl = getMemberBl();

			bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, getObserverDto()).Wait();

			_mpur.Verify(r => r.UpdateRecordAsync(It.IsAny<ProjectUser>()));
		}

		[Fact]
		public void CheckThatItIsNotPossibleToChangeRoleToNone()
		{
			string callerId = "user-that-owner", userId = "user-that-dev";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.None.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatItIsNotPossibleToChangeRoleFromNone()
		{
			string callerId = "user-that-owner", userId = "user-that-none";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.None.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatItIsNotPossibleToChangeRoleToOwner()
		{
			string callerId = "user-that-owner", userId = "user-that-dev";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.None.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatItIsNotPossibleToChangeRoleFromOwner()
		{
			string callerId = "user-that-owner";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.None.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), callerId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatOwnerCanMakeScrumMasterFromMember()
		{
			string callerId = "user-that-owner", userId = "user-dev";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.DoesExistScrumMasterInProjectAsync(It.IsAny<int>())).ReturnsAsync(false);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.ScrumMaster.Id };

			bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, roleDto).Wait();

			_mpur.Verify(r => r.UpdateRecordAsync(It.IsAny<ProjectUser>()));
		}

		[Fact]
		public void CheckThatOwnerCanNotMakeNewScrumMasterIfItAlreadyExists()
		{
			string callerId = "user-that-owner", userId = "user-dev";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.DoesExistScrumMasterInProjectAsync(It.IsAny<int>())).ReturnsAsync(true);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.ScrumMaster.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatScrumMasterHasNotPermissionsToSetScrumMaster()
		{
			string callerId = "user-that-master", userId = "user-dev";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.DoesExistScrumMasterInProjectAsync(It.IsAny<int>())).ReturnsAsync(false);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.ScrumMaster.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}

		[Fact]
		public void CheckThatScrumMasterHasNotPermissionsToResetScrumMaster()
		{
			string callerId = "user-that-master";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.DoesExistScrumMasterInProjectAsync(It.IsAny<int>())).ReturnsAsync(true);

			var bl = getMemberBl();
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = AppUserRole.Developer.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), callerId, roleDto)).Wait();
		}

		[Theory]
		[InlineData("user-that-master")]
		[InlineData("user-that-dev")]
		[InlineData("user-that-observer")]
		public void CheckThatItIsNotPossibleToChangeRoleWhenNewRoleIsTheSameAsOldRole(string userId)
		{
			string callerId = "user-that-owner";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			var bl = getMemberBl();
			IProjectUserRepository puRepo = _mpur.Object;
			AppUserRole role = puRepo.GetRoleOfMember(userId, It.IsAny<int>()).Result;
			AppUserRoleDto roleDto = new AppUserRoleDto { Id = role.Id };

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.ChangeUserRoleAsync(callerId, It.IsAny<int>(), userId, roleDto)).Wait();
		}







		[Fact]
		public void CheckThatNoMemberOfProjectCannotRemoveMembers()
		{
			string callerId = "user-1", userId = "user-2";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);
			_mpur.Setup(r => r.GetRoleOfMember(userId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			var bl = getMemberBl();

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.RemoveMemberAsync(callerId, It.IsAny<int>(), userId)).Wait();
		}

		[Fact]
		public void CheckThatNoMemberOfProjectCannotLeaveProject()
		{
			string callerId = "user-1";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

			var bl = getMemberBl();

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.RemoveMemberAsync(callerId, It.IsAny<int>(), callerId)).Wait();
		}

		[Fact]
		public void CheckThatOwnerCanRemoveAnyMember()
		{
			string callerId = "user-that-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.DoesExistMemberOfProject(userId, It.IsAny<int>())).ReturnsAsync(true);

			var bl = getMemberBl();

			bl.RemoveMemberAsync(callerId, It.IsAny<int>(), userId).Wait();

			_mpur.Verify(r => r.DeleteRecordAsync(userId, It.IsAny<int>()));
		}

		[Fact]
		public void CheckThatItIsNotPossibleToDeleteNoMember()
		{
			string callerId = "user-that-owner", userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
			_mpur.Setup(r => r.DoesExistMemberOfProject(userId, It.IsAny<int>())).ReturnsAsync(false);

			var bl = getMemberBl();

			Assert.ThrowsAsync<NotFoundResponseException>(async ()
					=> await bl.RemoveMemberAsync(callerId, It.IsAny<int>(), userId)).Wait();
		}

		[Fact]
		public void CheckThatOwnerCanNotLeaveProject()
		{
			string callerId = "user-owner";
			_mpur.Setup(r => r.GetRoleOfMember(callerId, It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

			var bl = getMemberBl();

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.RemoveMemberAsync(callerId, It.IsAny<int>(), callerId)).Wait();
		}

		[Theory]
		[InlineData("user-that-master")]
		[InlineData("user-that-dev")]
		[InlineData("user-that-observer")]
		public void CheckThatNoOwnerCanNotRemoveOtherMembers(string callerId)
		{
			string userId = "user-x";
			_mpur.Setup(r => r.GetRoleOfMember("user-that-master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			var bl = getMemberBl();

			Assert.ThrowsAsync<ForbiddenResponseException>(async ()
					=> await bl.RemoveMemberAsync(callerId, It.IsAny<int>(), userId)).Wait();
		}

		[Theory]
		[InlineData("user-that-master")]
		[InlineData("user-that-dev")]
		[InlineData("user-that-observer")]
		public void CheckThatNoOwnerCanLeaveProject(string callerId)
		{
			_mpur.Setup(r => r.GetRoleOfMember("user-that-master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
			_mpur.Setup(r => r.GetRoleOfMember("user-that-observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);

			_mpur.Setup(r => r.DoesExistMemberOfProject(callerId, It.IsAny<int>())).ReturnsAsync(true);

			var bl = getMemberBl();

			bl.RemoveMemberAsync(callerId, It.IsAny<int>(), callerId).Wait();

			_mpur.Verify(r => r.DeleteRecordAsync(callerId, It.IsAny<int>()));
		}
	}
}
