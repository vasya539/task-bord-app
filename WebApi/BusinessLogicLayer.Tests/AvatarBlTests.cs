using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using AutoMapper;

using WebApi.BLs;
using WebApi.Repositories.Interfaces;

namespace BusinessLogicLayer.Tests
{
	public class AvatarBlTests
	{
		private readonly IMapper _mapper;
		private readonly Mock<IUserRepository> _userRepoMock;
		private readonly Mock<IAvatarRepository> _avaRepoMock;


		public AvatarBlTests()
		{
			var mapperConfiguration = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
			_mapper = mapperConfiguration.CreateMapper();

			_userRepoMock = new Mock<IUserRepository>();
			_avaRepoMock = new Mock<IAvatarRepository>();
		}


		[Fact]
		public void GetAvatarAsync_ReturnsAvatar()
		{
			_avaRepoMock.Setup(r => r.GetAvatarStreamAsync(It.IsAny<string>())).ReturnsAsync(new System.IO.MemoryStream(0));
			var bl = new AvatarBl(_avaRepoMock.Object, _userRepoMock.Object);

			var res = bl.GetAvatarAsync(It.IsAny<string>()).Result;

			_avaRepoMock.Verify(r => r.GetAvatarStreamAsync(It.IsAny<string>()));
		}

		[Fact]
		public void DeleteAvatar_Calls()
		{
			var bl = new AvatarBl(_avaRepoMock.Object, _userRepoMock.Object);

			bl.DeleteAvatarAsync(It.IsAny<string>()).Wait();

			_avaRepoMock.Verify(r => r.DeleteAvatarAsync(It.IsAny<string>()));
			_userRepoMock.Verify(r => r.UpdateAvatarTailAsync(It.IsAny<string>(), null));
		}
	}
}
