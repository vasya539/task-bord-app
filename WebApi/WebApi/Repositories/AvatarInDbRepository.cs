using System;
using System.Threading.Tasks;
using System.IO;

using WebApi.Repositories.Interfaces;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Exceptions;


namespace WebApi.Repositories
{
	public class AvatarInDbRepository : IAvatarRepository
	{
		public const int MAX_STREAM_LENGTH = 30_000;

		private readonly AppDbContext _context;

		public AvatarInDbRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Stream> GetAvatarStreamAsync(string userId)
		{
			AvatarInDb avatar = await _context.Avatars.FindAsync(userId);
			if (avatar != null)
			{
				// if avatar exists extract from DB, store in memory and return
				return new MemoryStream(avatar.Avatar);
			}
			throw new NotFoundResponseException();
		}

		public async Task SaveAvatarStreamAsync(string userId, Stream stream)
		{
			if (stream.Length > MAX_STREAM_LENGTH)
				throw new ArgumentOutOfRangeException($"stream cannot contain more than {MAX_STREAM_LENGTH} bytes");

			AvatarInDb avatar = new AvatarInDb
			{
				UserId = userId
			};

			int length = (int) stream.Length;
			stream.Position = 0; // force reset position in stream before data coping

			avatar.Avatar = new byte[length];
			stream.Read(avatar.Avatar, 0, length);

			await CreateOrUpdateAvatarAsync(avatar);
		}

		private async Task CreateOrUpdateAvatarAsync(AvatarInDb avatar)
		{
			AvatarInDb a = await _context.Avatars.FindAsync(avatar.UserId);

			if(a != null)
				a.Avatar = avatar.Avatar; // if avatar already exists then update it
			else
				await _context.Avatars.AddAsync(avatar); // if avatar doesn't already exist then create new record in DB

			await _context.SaveChangesAsync();
		}

		public async Task DeleteAvatarAsync(string userId)
		{
			AvatarInDb avatar = await _context.Avatars.FindAsync(userId);
			if (avatar != null)
			{
				_context.Avatars.Remove(avatar);
				await _context.SaveChangesAsync();
			}
		}
	}
}
