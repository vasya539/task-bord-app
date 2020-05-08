using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

using DataAccessLayer.Tests.InMemoryDatabase;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;
using WebApi.Data.Models;

namespace DataAccessLayer.Tests
{
	public class UserRepositoryTests
	{
		[Fact]
		public void GetByIdAsync_ReturnsUserIfExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User expected = context.Users.Find("2138b181-4cee-4b85-9f16-18df308f387d");
				User user = repo.GetByIdAsync("2138b181-4cee-4b85-9f16-18df308f387d").Result;

				Assert.Equal(expected.Id, user.Id);
				Assert.Null(user.Info);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetByIdAsync_ReturnsNullIfUserDoesntExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User user = repo.GetByIdAsync("no-user").Result;

				Assert.Null(user);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void GetDetailedByIdAsync_ReturnsUserIfExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User user = repo.GetDetailedByIdAsync("2138b181-4cee-4b85-9f16-18df308f387d").Result;

				Assert.NotNull(user.Info);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetDetailedByIdAsync_ReturnsNullIfUserDoesntExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User user = repo.GetDetailedByIdAsync("no-user").Result;

				Assert.Null(user);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void GetByUserNameAsyncDetailed_ReturnsDetailedUserIfUserExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User user = repo.GetByUserNameAsync("MyLogin1", true).Result;

				Assert.NotNull(user);
				Assert.NotNull(user.Info);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetByUserNameAsyncNotDetailed_ReturnsUserIfUserExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User user = repo.GetByUserNameAsync("MyLogin1", false).Result;

				Assert.NotNull(user);
				Assert.Null(user.Info);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetByUserNameAsyncDetailed_ReturnsNullIfUserDoesntExist()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User user = repo.GetByUserNameAsync("no-user", true).Result;

				Assert.Null(user);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetByUserNameAsyncNotDetailed_ReturnsNullIfUserDoesntExist()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				User user = repo.GetByUserNameAsync("no-user", false).Result;

				Assert.Null(user);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void UpdateAsync_UpdatesValue()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);
				User oldUser = repo.GetByIdAsync("2138b181-4cee-4b85-9f16-18df308f387d").Result;
				User user = new User
				{
					Id = oldUser.Id,
					UserName = oldUser.UserName,
					Email = oldUser.Email,
					FirstName = "John"
				};


				repo.UpdateAsync(user).Wait();
				User newUser = repo.GetByIdAsync("2138b181-4cee-4b85-9f16-18df308f387d").Result;


				Assert.Equal(user.FirstName, newUser.FirstName);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void DeleteAsync_DeletesValueIfItDoesExist()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				repo.DeleteAsync("2138b181-4cee-4b85-9f16-18df308f387d").Wait();

				User user = repo.GetByIdAsync("2138b181-4cee-4b85-9f16-18df308f387d").Result;

				Assert.Null(user);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void DeleteAsync_DoesNothingIfItDoesNotExist()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				repo.DeleteAsync("no-user").Wait();

				// assert nothing
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}




		[Fact]
		public void ExistsWithId_ReturnsUserIfExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				bool res = repo.ExistsWithId("2138b181-4cee-4b85-9f16-18df308f387d").Result;

				Assert.True(res);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void ExistsWithId_ReturnsNullIfUserDoesntExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				bool res = repo.ExistsWithId("no-user").Result;

				Assert.False(res);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void ExistsWithUserName_ReturnsUserIfExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				bool res = repo.ExistsWithUserName("MyLogin1").Result;

				Assert.True(res);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void ExistsWithUserName_ReturnsNullIfUserDoesntExists()
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				bool res = repo.ExistsWithUserName("no-user").Result;

				Assert.False(res);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Theory]
		[InlineData(null)]
		[InlineData(12)]
		[InlineData(3929)]
		public void UpdateAvatarTailAsync_UpdatesAvatarTail(int? tail)
		{
			var context = InMemoryAppDbContext.GetUniqueAppDbContext();
			try
			{
				UserRepository repo = new UserRepository(context);

				repo.UpdateAvatarTailAsync("2138b181-4cee-4b85-9f16-18df308f387d", tail).Wait();

				User user = repo.GetByIdAsync("2138b181-4cee-4b85-9f16-18df308f387d").Result;

				Assert.Equal(tail, user.AvatarTail);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}
	}
}
