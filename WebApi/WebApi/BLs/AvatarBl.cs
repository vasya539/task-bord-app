using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

using WebApi.BLs.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Exceptions;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace WebApi.BLs
{
	public class AvatarBl : IAvatarBl
	{
		public const int
				AVATAR_SIDE_LENGTH = 100,
				MAX_AVATAR_IMAGE_SIDE_LENGTH = 1000;

		private readonly IAvatarRepository _avatarRepository;
		private readonly IUserRepository _userRepository;

		public AvatarBl(IAvatarRepository avatarRepository, IUserRepository userRepository)
		{
			_avatarRepository = avatarRepository;
			_userRepository = userRepository;
		}


		public async Task<FileResult> GetAvatarAsync(string userId)
		{
			// return avatar as file (image in jpg format)
			return new FileStreamResult(await _avatarRepository.GetAvatarStreamAsync(userId), "image/jpeg");
		}

		public async Task UpdateAvatarAsync(string userId, Stream fileStream)
		{
			using (var image = Image.Load(fileStream))
			{
				PrepareImage(image);

				using (var outStream = new MemoryStream())
				{
					image.SaveAsJpeg(outStream);
					await _avatarRepository.SaveAvatarStreamAsync(userId, outStream);

					// set new tail when avatar has changed. Set new random value in range 0..9999 to ignore cache response
					await _userRepository.UpdateAvatarTailAsync(userId, (int)(DateTime.UtcNow.Ticks % 10_000));
				}
			}
		}

		private void PrepareImage(Image image)
		{
			int
				width = image.Width,
				height = image.Height;

			if (width < AVATAR_SIDE_LENGTH || height < AVATAR_SIDE_LENGTH)
				throw new BadRequestResponseException($"image's width and height cannot be less than {AVATAR_SIDE_LENGTH} px");

			if (width > MAX_AVATAR_IMAGE_SIDE_LENGTH || height > MAX_AVATAR_IMAGE_SIDE_LENGTH)
				throw new BadRequestResponseException($"image's width and height cannot be greater than {MAX_AVATAR_IMAGE_SIDE_LENGTH} px");

			image.Mutate(op =>
			{
				if (width != height)
				{
					int x, y, side;

					// get coords of start point of square
					if (width < height)
					{ x = 0; side = width; y = height / 2 - side / 2; }
					else
					{ y = 0; side = height; x = width / 2 - side / 2; }

					// crop image to get square
					op.Crop(new Rectangle(x, y, side, side));
				}

				op.Resize(AVATAR_SIDE_LENGTH, AVATAR_SIDE_LENGTH);
			});
		}

		public async Task DeleteAvatarAsync(string userId)
		{
			await _avatarRepository.DeleteAvatarAsync(userId);
			await _userRepository.UpdateAvatarTailAsync(userId, null);
		}
	}
}
