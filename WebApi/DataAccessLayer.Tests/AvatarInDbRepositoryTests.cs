using DataAccessLayer.Tests.InMemoryDatabase;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using System.IO;

using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Repositories;
using WebApi.Exceptions;

namespace DataAccessLayer.Tests
{
	/*
		Task DeleteAvatarAsync(string userId);
	 */
	public class AvatarInDbRepositoryTests
	{
		private AppDbContext GetContext()
		{
			var context = InMemoryAppDbContext.GetEmptyUniqueAppDbContext();

			context.Avatars.Add(new AvatarInDb { Avatar = new byte[100], UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992" });

			
			context.SaveChanges();
			return context;
		}

		[Fact]
		public void GetAvatarStreamAsync_ReturnsStreamIfAvatarExists()
		{
			var context = GetContext();
			try
			{
				AvatarInDbRepository repo = new AvatarInDbRepository(context);

				var avatar = repo.GetAvatarStreamAsync("421cb65f-a76d-4a73-8a1a-d792f37ef992").Result;

				Assert.NotNull(avatar);
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}

		[Fact]
		public void GetAvatarStreamAsync_ThrowsNotFoundResponseExceptionIfAvatarDoesntExist()
		{
			var context = GetContext();
			try
			{
				AvatarInDbRepository repo = new AvatarInDbRepository(context);

				//var avatar = repo.GetAvatarStreamAsync("421cb65f-a76d-4a73-8a1a-d792f37ef992").Result;

				Assert.ThrowsAsync<NotFoundResponseException>(async () =>
					await repo.GetAvatarStreamAsync("no-user")).Wait();
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void SaveAvatarStreamAsync_UpdatesStreamIfAvatarExists()
		{
			var context = GetContext();
			try
			{
				AvatarInDbRepository repo = new AvatarInDbRepository(context);

				Stream stream = new MemoryStream(new byte[200]);
				repo.SaveAvatarStreamAsync("421cb65f-a76d-4a73-8a1a-d792f37ef992", stream).Wait();

				var a = repo.GetAvatarStreamAsync("421cb65f-a76d-4a73-8a1a-d792f37ef992").Result;
				Assert.Equal(200, a.Length);

			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void SaveAvatarStreamAsync_CreatesNewStreamIfAvatarDoesntExist()
		{
			var context = GetContext();
			try
			{
				AvatarInDbRepository repo = new AvatarInDbRepository(context);

				Stream stream = new MemoryStream(new byte[300]);
				repo.SaveAvatarStreamAsync("2138b181-4cee-4b85-9f16-18df308f387d", stream).Wait();

				var a = repo.GetAvatarStreamAsync("2138b181-4cee-4b85-9f16-18df308f387d").Result;
				Assert.Equal(300, a.Length);

			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void SaveAvatarStreamAsync_ThrowsExceptionIfStreamIsTooLarge()
		{
			var context = GetContext();
			try
			{
				AvatarInDbRepository repo = new AvatarInDbRepository(context);

				Stream stream = new MemoryStream(new byte[40_000]);

				Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
					await repo.SaveAvatarStreamAsync("2138b181-4cee-4b85-9f16-18df308f387d", stream)).Wait();
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


		[Fact]
		public void DeleteAvatarAsync_DeletesAvatar()
		{
			var context = GetContext();
			try
			{
				AvatarInDbRepository repo = new AvatarInDbRepository(context);

				repo.DeleteAvatarAsync("2138b181-4cee-4b85-9f16-18df308f387d").Wait();

				Assert.False(context.Avatars.Any(a => a.UserId == "2138b181-4cee-4b85-9f16-18df308f387d"));
			}
			finally
			{
				context.Database.EnsureDeleted();
				context.Dispose();
			}
		}


	}
}
