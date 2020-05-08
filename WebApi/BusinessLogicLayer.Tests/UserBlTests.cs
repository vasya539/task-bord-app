using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AutoMapper;
using Xunit;
using Moq;

using WebApi.Exceptions;
using WebApi.BLs.Interfaces;
using WebApi.BLs;
using WebApi.Repositories.Interfaces;
using WebApi.Data.Models;
using WebApi.Data.DTOs;

namespace BusinessLogicLayer.Tests
{
	public class UserBlTests
	{
		private readonly IMapper _mapper;
		private readonly Mock<IUserRepository> _userRepoMock;
		private readonly Mock<IProjectUserRepository> _puRepoMock;

		private User user1 = new User { Id = "user-1", UserName = "user_1" };
		private User user2 = new User { Id = "user-2", UserName = "user_2" };
		private User user3 = new User { Id = "user-3", UserName = "user_3" };
		private User user4 = new User { Id = "user-4", UserName = "user_4" };
		private List<Project> projectsForUser1 = new List<Project>();
		private List<Project> projectsForUser2 = new List<Project>();
		private List<Project> projectsForUser3 = new List<Project>();

		//private readonly string _validTemplateForUserSearch = "john";

		public UserBlTests()
		{
			var mapperConfiguration = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
			_mapper = mapperConfiguration.CreateMapper();

			_userRepoMock = new Mock<IUserRepository>();
			_puRepoMock = new Mock<IProjectUserRepository>();

			MakeProjects();
		}

		//Users 1 & 2 have common projects.
		//User 3 haven't common projects with users 1 or 2		 
		private void MakeProjects()
		{
			Project p1 = new Project { Id = 1, Name = "Project 1" };
			Project p2 = new Project { Id = 2, Name = "Project 2" };
			Project p3 = new Project { Id = 3, Name = "Project 3" };
			Project p4 = new Project { Id = 4, Name = "Project 4" };
			Project p5 = new Project { Id = 5, Name = "Project 5" };

			projectsForUser1.AddRange(new Project[] { p1, p2, p3 });
			projectsForUser2.AddRange(new Project[] { p2, p3, p4 });
			projectsForUser3.Add(p5);
		}


		[Fact]
		public void GetByIdAsync_ReturnsUser()
		{
			_userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var u = bl.GetByIdAsync(It.IsAny<string>()).Result;

			_userRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<string>()));
		}

		[Fact]
		public void GetByIdAsync_ThrowsNotFoundExceptionIfUserDoesNotExist()
		{
			_userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(null as User);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.GetByIdAsync(It.IsAny<string>())).Wait();
		}


		[Fact]
		public void GetDetailedByIdAsync_ReturnsUser()
		{
			_userRepoMock.Setup(r => r.GetDetailedByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var u = bl.GetDetailedByIdAsync(It.IsAny<string>()).Result;

			_userRepoMock.Verify(r => r.GetDetailedByIdAsync(It.IsAny<string>()));
		}

		[Fact]
		public void GetDetailedByIdAsync_ThrowsNotFoundExceptionIfUserDoesNotExist()
		{
			_userRepoMock.Setup(r => r.GetDetailedByIdAsync(It.IsAny<string>())).ReturnsAsync(null as User);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.GetDetailedByIdAsync(It.IsAny<string>())).Wait();
		}



		[Fact]
		public void GetByUserNameNotDetailedAsync_ReturnsUser()
		{
			_userRepoMock.Setup(r => r.GetByUserNameAsync(It.IsAny<string>(), false)).ReturnsAsync(user1);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var u = bl.GetByUserNameAsync(It.IsAny<string>(), false).Result;

			_userRepoMock.Verify(r => r.GetByUserNameAsync(It.IsAny<string>(), false));
		}

		[Fact]
		public void GetByUserNameNotDetailedAsync_ThrowsNotFoundExceptionIfUserDoesNotExist()
		{
			_userRepoMock.Setup(r => r.GetByUserNameAsync(It.IsAny<string>(), false)).ReturnsAsync(null as User);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.GetByUserNameAsync(It.IsAny<string>(), false)).Wait();
		}


		[Fact]
		public void GetByUserNameDetailedAsync_ReturnsUser()
		{
			_userRepoMock.Setup(r => r.GetByUserNameAsync(It.IsAny<string>(), true)).ReturnsAsync(user1);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var u = bl.GetByUserNameAsync(It.IsAny<string>(), true).Result;

			_userRepoMock.Verify(r => r.GetByUserNameAsync(It.IsAny<string>(), true));
		}

		[Fact]
		public void GetByUserNameDetailedAsync_ThrowsNotFoundExceptionIfUserDoesNotExist()
		{
			_userRepoMock.Setup(r => r.GetByUserNameAsync(It.IsAny<string>(), true)).ReturnsAsync(null as User);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.GetByUserNameAsync(It.IsAny<string>(), true)).Wait();
		}



		[Fact]
		public void GetProjectsOfUser_ThrowsNotFoundExceptionIfRequestedUserDoesNotExist()
		{
			_userRepoMock.Setup(r => r.ExistsWithId("no-user")).ReturnsAsync(false);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await bl.GetProjectsOfUser("user", "no-user")).Wait();
		}

		[Fact]
		public void GetProjectsOfUser_ForTheSameUser_CallsOnceGetProjectsFromRepo()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user1.Id)).ReturnsAsync(true);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user1.Id)).ReturnsAsync(projectsForUser1);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var projs = bl.GetProjectsOfUser(user1.Id, user1.Id).Result;

			_puRepoMock.Verify(r => r.GetProjectsOfUser(user1.Id), Times.Once);
		}

		[Fact]
		public void GetProjectsOfUser_ForTheSameUser_ReturnsAllItsProjects()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user1.Id)).ReturnsAsync(true);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user1.Id)).ReturnsAsync(projectsForUser1);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var projs = bl.GetProjectsOfUser(user1.Id, user1.Id).Result.ToList();

			Assert.Equal(projectsForUser1.Count(), projs.Count());
			for (int i = 0; i < projectsForUser1.Count(); ++i)
				Assert.Equal(projectsForUser1[i].Id, projs[i].Id);
		}

		[Fact]
		public void GetProjectsOfUser_ForTwoUsers_CallsGetProjectsFromRepoForTheseUsers()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user2.Id)).ReturnsAsync(true);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user1.Id)).ReturnsAsync(projectsForUser1);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user2.Id)).ReturnsAsync(projectsForUser2);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var projs = bl.GetProjectsOfUser(user1.Id, user2.Id).Result.ToList();

			_puRepoMock.Verify(r => r.GetProjectsOfUser(user1.Id), Times.Once);
			_puRepoMock.Verify(r => r.GetProjectsOfUser(user2.Id), Times.Once);
		}

		[Fact]
		public void GetProjectsOfUser_ForTwoUsers_ReturnsCommonProjects()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user2.Id)).ReturnsAsync(true);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user1.Id)).ReturnsAsync(projectsForUser1);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user2.Id)).ReturnsAsync(projectsForUser2);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var projs = bl.GetProjectsOfUser(user1.Id, user2.Id).Result.ToList();

			foreach (var pd in projs)
				Assert.True(projectsForUser1.Any(p => p.Id == pd.Id) && projectsForUser2.Any(p => p.Id == pd.Id));
		}

		[Fact]
		public void GetProjectsOfUser_ForTwoUsers_ReturnsEmptyResultIfNoCommonProjects()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user3.Id)).ReturnsAsync(true);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user1.Id)).ReturnsAsync(projectsForUser1);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user3.Id)).ReturnsAsync(projectsForUser3);

			UserBl bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var projs = bl.GetProjectsOfUser(user1.Id, user3.Id).Result.ToList();

			Assert.Empty(projs);
		}



		[Fact]
		public void UpdateAsync_CallsUpdateAsyncOnRepo()
		{
			UserDto ud = new UserDto { Id = user1.Id };
			User mappedUser = new User { Id = user1.Id };

			Mock<IMapper> mapperMock = new Mock<IMapper>();
			mapperMock.Setup(m => m.Map<User>(ud)).Returns(mappedUser);

			var bl = new UserBl(_userRepoMock.Object, mapperMock.Object, _puRepoMock.Object);
			bl.UpdateAsync(user1.Id, ud).Wait();

			_userRepoMock.Verify(r => r.UpdateAsync(mappedUser));
		}

		[Fact]
		public void UpdateAsync_ThrowsBadRequestIfUserIdIsNotTheSameAsInDto()
		{
			UserDto ud = new UserDto { Id = user2.Id };

			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<BadRequestResponseException>(async () =>
					await bl.UpdateAsync(user1.Id, ud)).Wait();
		}

		[Fact]
		public void DeleteAsync_CallsDeleteAsyncOnRepo()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user4.Id)).ReturnsAsync(true);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user4.Id)).ReturnsAsync(new List<Project>());

			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);
			bl.DeleteAsync(user4.Id).Wait();

			_userRepoMock.Verify(r => r.DeleteAsync(user4.Id));
		}

		[Fact]
		public void DeleteAsync_DoesntCallDeleteAsyncOnRepoIfUserHasProjects()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user3.Id)).ReturnsAsync(true);
			_puRepoMock.Setup(r => r.GetProjectsOfUser(user3.Id)).ReturnsAsync(projectsForUser3);

			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<ForbiddenResponseException>(async () =>
					await bl.DeleteAsync(user3.Id)).Wait();
		}

		[Fact]
		public void DeleteAsync_DoenstCallDeleteAsyncOnRepoIfUserDoesntExist()
		{
			_userRepoMock.Setup(r => r.ExistsWithId(user4.Id)).ReturnsAsync(false);

			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);
			bl.DeleteAsync(user4.Id).Wait();

			_userRepoMock.Verify(r => r.DeleteAsync(user4.Id), Times.Never);
		}




		[Theory]
		[InlineData((string)null)]
		[InlineData("")]
		[InlineData("qwe")]
		[InlineData("q    ee")]
		[InlineData("       ert     ")]
		[InlineData("@er")]
		[InlineData("qwe eeer tt")]
		[InlineData("0123456789012345678900123456789012345678901")] // 41 characters
		public void Find_ThrowsBadRequestIfBadTemplateGiven(string template)
		{
			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<BadRequestResponseException>(async () =>
					await bl.Find(template, 1)).Wait();
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public void Find_ThrowsBadRequestIfBadPageNumberGiven(int pageNumber)
		{
			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			Assert.ThrowsAsync<BadRequestResponseException>(async () =>
					await bl.Find("john", pageNumber)).Wait();
		}

		[Theory]
		[InlineData("john", "john")]
		[InlineData("  john", "john")]
		[InlineData("john  ", "john")]
		[InlineData("  john  ", "john")]
		public void Find_CallsFindAdvancedPlusOneRowAsync(string template, string searchTmpl)
		{
			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var res = bl.Find(template, 1).Result;
			_userRepoMock.Verify(r => r.FindAdvancedPlusOneRowAsync(searchTmpl, 1, UserBl.USER_FIND_PAGE_SIZE));
		}

		[Theory]
		[InlineData("@john", "john")]
		[InlineData("@joh", "joh")]
		public void Find_CallsFindAdvancedByUserNamePlusOneRowAsync(string template, string searchTmpl)
		{
			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var res = bl.Find(template, 1).Result;
			_userRepoMock.Verify(r => r.FindAdvancedByUserNamePlusOneRowAsync(searchTmpl, 1, UserBl.USER_FIND_PAGE_SIZE));
		}

		[Theory]
		[InlineData("john conn", "john", "conn")]
		[InlineData("jo co", "jo", "co")]
		[InlineData("joh c", "joh", "c")]
		public void Find_CallsFindAdvancedByFullNamePlusOneRowAsync(string template, string fName, string lName)
		{
			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var res = bl.Find(template, 1).Result;
			_userRepoMock.Verify(r => r.FindAdvancedByFullNamePlusOneRowAsync(fName, lName, 1, UserBl.USER_FIND_PAGE_SIZE));
		}

		[Fact]
		public void Find_ReturnsPageThatHasNextIfRepoFindReturnsOneRowMore()
		{
			var users = new List<UserFoundModel>();
			for (int i = 0; i <= UserBl.USER_FIND_PAGE_SIZE; ++i)
				users.Add(new UserFoundModel { Id = $"{i}" });

			_userRepoMock.Setup(r => r.FindAdvancedPlusOneRowAsync("john", 1, UserBl.USER_FIND_PAGE_SIZE)).ReturnsAsync(users);

			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var res = bl.Find("john", 1).Result;

			Assert.True(res.HasNext);
		}

		[Fact]
		public void Find_ReturnsPageThatDoesntHaveNextIfRepoFindReturnsNoMoreAsCount()
		{
			var users = new List<UserFoundModel>();
			for (int i = 0; i < UserBl.USER_FIND_PAGE_SIZE; ++i)
				users.Add(new UserFoundModel { Id = $"{i}" });

			_userRepoMock.Setup(r => r.FindAdvancedPlusOneRowAsync("john", 1, UserBl.USER_FIND_PAGE_SIZE)).ReturnsAsync(users);

			var bl = new UserBl(_userRepoMock.Object, _mapper, _puRepoMock.Object);

			var res = bl.Find("john", 1).Result;

			Assert.False(res.HasNext);
		}
	}
}
