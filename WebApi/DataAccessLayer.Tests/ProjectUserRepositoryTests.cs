using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

using WebApi.Repositories;
using WebApi.Data;
using WebApi.Data.Models;
using DataAccessLayer.Tests.InMemoryDatabase;
using System.Linq;

namespace DataAccessLayer.Tests
{
	public class ProjectUserRepositoryTests
	{
		private AppDbContext GetContext()
		{
			var context = InMemoryAppDbContext.GetEmptyUniqueAppDbContext();

			User user1 = new User { Id = "2138b181-4cee-4b85-9f16-18df308f387d", UserName = "MyLogin1", NormalizedUserName = "MYLOGIN1", PasswordHash = "MyPass", FirstName = "Bart", LastName = "Simpson", Email = "bart@simpson.com", Info = "in-memory user" };
			User user2 = new User { Id = "2514591e-29f0-4a63-b0ad-87a3e7ebec3d", UserName = "MyLogin2", NormalizedUserName = "MYLOGIN2", PasswordHash = "MyPass", FirstName = "Lisa", LastName = "Simpson", Email = "lisa@simpson.com", Info = "in-memory user" };
			User user3 = new User { Id = "421cb65f-a76d-4a73-8a1a-d792f37ef992", UserName = "MyLogin3", NormalizedUserName = "MYLOGIN3", PasswordHash = "MyPass", FirstName = "Homer", LastName = "Simpson", Email = "homer@simpson.com", Info = "in-memory user" };
			User user4 = new User { Id = "54bfd1f9-d379-4930-9c3b-4c84992c028e", UserName = "MyLogin4", NormalizedUserName = "MYLOGIN4", PasswordHash = "MyPass", FirstName = "Marge", LastName = "Simpson", Email = "marge@simpson.com", Info = "in-memory user" };


			Project project1 = new Project { Id = 1, Name = "First Project", Description = "Some description to Project1" };
			Project project2 = new Project { Id = 2, Name = "Second Project", Description = "Some description to Project2" };


			ProjectUser pu1 = new ProjectUser { UserId = "2138b181-4cee-4b85-9f16-18df308f387d", ProjectId = 1, UserRoleId = AppUserRole.Owner.Id };
			ProjectUser pu2 = new ProjectUser { UserId = "2138b181-4cee-4b85-9f16-18df308f387d", ProjectId = 2, UserRoleId = AppUserRole.ScrumMaster.Id };
			ProjectUser pu3 = new ProjectUser { UserId = "2514591e-29f0-4a63-b0ad-87a3e7ebec3d", ProjectId = 2, UserRoleId = AppUserRole.Owner.Id };
			ProjectUser pu4 = new ProjectUser { UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992", ProjectId = 1, UserRoleId = AppUserRole.Developer.Id };

			context.Users.AddRange(new[] { user1, user2, user3, user4 });
			context.Projects.AddRange(new[] { project1, project2 });
			context.ProjectUsers.AddRange(new[] { pu1, pu2, pu3, pu4 });

			context.SaveChanges();
			return context;
		}

		[Fact]
		public void GetRecordAsync_ReturnsRecordIfRecordExists()
		{
			AppDbContext context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				ProjectUser rec = repo.GetRecordAsync("2138b181-4cee-4b85-9f16-18df308f387d", 1).Result;

				Assert.NotNull(rec);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetRecordAsync_ReturnsNullIfRecordDoesntExists()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				ProjectUser rec = repo.GetRecordAsync("no-user", 1).Result;

				Assert.Null(rec);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}



		[Fact]
		public void CreateRecordAsync_CreatesNewRecordIfItDoesntExists()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);
				ProjectUser rec = new ProjectUser
				{
					UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992",
					ProjectId = 2,
					UserRoleId = AppUserRole.Observer.Id
				};

				repo.CreateRecordAsync(rec).Wait();

				Assert.Equal(5, context.ProjectUsers.Count());
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void CreateRecordAsync_ThrowsExceptionRecordIfItDoesntExists()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);
				ProjectUser rec = new ProjectUser
				{
					UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992",
					ProjectId = 1,
					UserRoleId = AppUserRole.Observer.Id
				};

				Assert.ThrowsAsync<InvalidOperationException>(async () =>
					await repo.CreateRecordAsync(rec)).Wait();
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void UpdateRecordAsync_UpdatesExistingRecordIfItDoesntExists()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);
				ProjectUser rec = new ProjectUser
				{
					UserId = "2138b181-4cee-4b85-9f16-18df308f387d",
					ProjectId = 2,
					UserRoleId = AppUserRole.Observer.Id
				};

				repo.UpdateRecordAsync(rec).Wait();

				var rc = repo.GetRecordAsync("2138b181-4cee-4b85-9f16-18df308f387d", 2).Result;

				Assert.Equal(4, context.ProjectUsers.Count());
				Assert.Equal(AppUserRole.Observer.Id, rc.UserRoleId);

			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void DeleteRecordAsync_DeletesNewRecordIfItExists()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);
				ProjectUser rec = repo.GetRecordAsync("2138b181-4cee-4b85-9f16-18df308f387d", 2).Result;

				repo.DeleteRecordAsync(rec).Wait();

				Assert.Equal(3, context.ProjectUsers.Count());
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void DeleteRecordAsync_ThrowsExceptionRecordIfItDoesntExists()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);
				ProjectUser rec = new ProjectUser
				{
					UserId = "no-user",
					ProjectId = 1,
					UserRoleId = AppUserRole.Observer.Id
				};

				Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>(async () =>
					await repo.DeleteRecordAsync(rec)).Wait();
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void DeleteRecordAsync_DeletesNewRecordIfUserAndProjExist()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				repo.DeleteRecordAsync("2138b181-4cee-4b85-9f16-18df308f387d", 2).Wait();

				Assert.Equal(3, context.ProjectUsers.Count());
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}



		[Fact]
		public void GetProjectsOfUser_ReturnsListOfProjects()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var res = repo.GetProjectsOfUser("2138b181-4cee-4b85-9f16-18df308f387d").Result;

				Assert.Equal(2, res.Count());
				Assert.True(res.Any(p => p.Id == 1) && res.Any(p => p.Id == 2));
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetProjectsOfUser_ReturnsEmptyResIfUserHasntProjects()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var res = repo.GetProjectsOfUser("no-user").Result;

				Assert.Empty(res);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void GetMembersOfProject_ReturnsListOfMembers()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var res = repo.GetMembersOfProject(2).Result;

				Assert.Equal(2, res.Count());
				Assert.True(
						res.Any(m => m.Id == "2138b181-4cee-4b85-9f16-18df308f387d") &&
						res.Any(m => m.Id == "2514591e-29f0-4a63-b0ad-87a3e7ebec3d"));
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetMembersOfProject_ReturnsEmptyResIfNoProjects()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var res = repo.GetMembersOfProject(123).Result;

				Assert.Empty(res);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetOwnerOfProject_ReturnsOwnerIfProjExists()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var user = repo.GetOwnerOfProject(1).Result;

				Assert.Equal("2138b181-4cee-4b85-9f16-18df308f387d", user.Id);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetOwnerOfProject_ReturnsNullIfProjDoesntExist()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var user = repo.GetOwnerOfProject(123).Result;

				Assert.Null(user);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}



		[Fact]
		public void GetRoleOfMember_ReturnsRole_Owner()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var role = repo.GetRoleOfMember("2138b181-4cee-4b85-9f16-18df308f387d", 1).Result;

				Assert.Equal(AppUserRole.Owner.Id, role.Id);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetRoleOfMember_ReturnsRole_Developer()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var role = repo.GetRoleOfMember("421cb65f-a76d-4a73-8a1a-d792f37ef992", 1).Result;

				Assert.Equal(AppUserRole.Developer.Id, role.Id);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void GetRoleOfMember_ReturnsRole_NoMember()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var role = repo.GetRoleOfMember("no-user-in-project", 1).Result;

				Assert.Equal(AppUserRole.None.Id, role.Id);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void DoesExistScrumMasterInProjectAsync_ReturnsTrueIfScrumMasterDoesExist()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var doesExist = repo.DoesExistScrumMasterInProjectAsync(2).Result;

				Assert.True(doesExist);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void DoesExistScrumMasterInProjectAsync_ReturnsFalseIfScrumMasterDoesNotExist()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var doesExist = repo.DoesExistScrumMasterInProjectAsync(1).Result;

				Assert.False(doesExist);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void DoesExistMemberOfProject_ReturnsTrueIfMemberDoesExist()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var doesExist = repo.DoesExistMemberOfProject("421cb65f-a76d-4a73-8a1a-d792f37ef992", 1).Result;

				Assert.True(doesExist);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void DoesExistMemberOfProject_ReturnsFalseIfMemberDoesNotExist()
		{
			var context = GetContext();
			try
			{
				ProjectUserRepository repo = new ProjectUserRepository(context);

				var doesExist = repo.DoesExistMemberOfProject("421cb65f-a76d-4a73-8a1a-d792f37ef992", 123).Result;

				Assert.False(doesExist);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}
	}
}
