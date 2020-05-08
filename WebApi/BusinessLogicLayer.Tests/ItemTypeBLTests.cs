using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.BLs;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class ItemTypeBLTests
    {
        private readonly Mock<IItemTypeRepository> mockRepo;
        private readonly Mock<IMapper> mockMapper;

        public ItemTypeBLTests()
        {
            mockRepo = new Mock<IItemTypeRepository>();
            mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public void GetAllAsync_Should_Returns_AllItemTypes()
        {
            // Arrange
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetSampleItemTypes());
            var itemTypeBL = new ItemTypeBl(mockRepo.Object, mockMapper.Object);
            // Act
            var result = itemTypeBL.GetAllAsync();
            // Assert
            var typeResult = Assert.IsType<Task<List<ItemTypeDto>>>(result);
            var model = Assert.IsAssignableFrom<List<ItemTypeDto>>(typeResult.Result);
            Assert.Equal(GetSampleItemTypes().Count, model.Count());
        }
        [Fact]
        public void Read_Should_Call_Once_ReadAsync()
        {
            int id = 1;
            // Arrange
            var itemTypeBL = new ItemTypeBl(mockRepo.Object, mockMapper.Object);
            // Act
            var result = itemTypeBL.Read(id);
            // Assert
            var typeResult = Assert.IsType<Task<ItemTypeDto>>(result);
            mockRepo.Verify(r => r.ReadAsync(id), Times.Once);
        }

        [Fact]
        public void Create_Should_Call_Once_CreateAsync()
        {
            var repo = new Mock<IStatusRepository>();
            // Arrange
            var itemTypeBL = new ItemTypeBl(mockRepo.Object, mockMapper.Object);
            // Act
            var itemType = new ItemTypeDto { Id = 1, Name = "New" };
            var res = itemTypeBL.Create(itemType);
            // Assert
            mockRepo.Verify(r => r.CreateAsync(null), Times.Once());
        }
        [Fact]
        public void Update_Should_Call_Once_CreateAsync()
        {
            var repo = new Mock<IStatusRepository>();
            // Arrange
            var itemTypeBL = new ItemTypeBl(mockRepo.Object, mockMapper.Object);
            // Act
            var itemTypeDto = new ItemTypeDto { Id = 1, Name = "Test" };
            var res = itemTypeBL.Update(itemTypeDto);
            // Assert
            mockRepo.Verify(r => r.UpdateAsync(null), Times.Once());
        }
        [Fact]
        public void Create_Should_Call_Once_DeleteAsync()
        {
            // Arrange
            var itemTypeBL = new ItemTypeBl(mockRepo.Object, mockMapper.Object);
            // Act
            var result = itemTypeBL.Delete(1);
            // Assert
            mockRepo.Verify(r => r.DeleteAsync(1), Times.Once());
        }
        public List<ItemType> GetSampleItemTypes()
        {
            List<ItemType> statuses = new List<ItemType>();
            ItemType itemType1 = new ItemType { Id = 1, Name = "Test" };
            ItemType itemType2 = new ItemType { Id = 2, Name = "Bag" };
            ItemType itemType3 = new ItemType { Id = 3, Name = "User Story" };
            statuses.AddRange(new[] { itemType1, itemType2, itemType3 });
            return statuses;
        }
    }
}
