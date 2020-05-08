using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.DTOs;
using WebApi.Interfaces.IRepositories;
using WebApi.Services;
using Xunit;

namespace WebApiTests.BLs
{
    public class ItemBLsTest
    {
        private int Id = 101;
        private readonly Mock<IItemRepository> mockRepository;
        private readonly Mock<IMapper> mockMapper;

        public ItemBLsTest()
        {
            mockRepository = new Mock<IItemRepository>();
            mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetAllUsersChech()
        {
            //Arrange
            ItemBl itemBl = new ItemBl(mockRepository.Object, mockMapper.Object);

            //Act
            var res = await itemBl.GetAllAsync();

            //Assert
            Assert.IsAssignableFrom<IEnumerable<ItemDto>>(res);
        }

        [Fact]
        public async Task GetNotExistedUserById()
        {
            //Arrange
            ItemBl itemBl = new ItemBl(mockRepository.Object, mockMapper.Object);

            //Act
            var res = await itemBl.ReadAsync(It.IsAny<int>());

            //Assert
            mockRepository.Verify(r => r.ReadAsync(It.IsAny<int>()));
            Assert.Null(res);
        }
    }
}