using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApi.BLs;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class ItemRelationBlTests
    {
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly IMapper _mockMapper;
        private readonly Mock<IItemRelationRepository> _itemRelationRepo;
        private readonly Mock<IProjectUserRepository> _mockProjectUserRepo;
        private readonly Mock<IProjectRepository> _mockProjectRepo;
        private readonly Mock<ISprintRepository> _mockSprintRepository;

        public ItemRelationBlTests()
        {
            var cfg = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
            _mockItemRepo = new Mock<IItemRepository>();
            _mockMapper = cfg.CreateMapper();
            _itemRelationRepo = new Mock<IItemRelationRepository>();
            _mockProjectUserRepo = new Mock<IProjectUserRepository>();
            _mockProjectRepo = new Mock<IProjectRepository>();
            _mockSprintRepository = new Mock<ISprintRepository>();
        }

        [Theory]
        [InlineData(1, 2)]
        public async Task CheckGetRecordReturnItemRelation(int firstItemId, int secondItemId)
        {
            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(firstItemId, secondItemId))
                .ReturnsAsync(GetTestRelations()[0]);

            ItemRelationBl itemRelationBl = new ItemRelationBl(_mockItemRepo.Object, _mockMapper,
                _itemRelationRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object,
                _mockSprintRepository.Object);

            var response = await itemRelationBl.GetRecordAsync(firstItemId, secondItemId);
            Assert.IsAssignableFrom<ItemRelation>(response);
            Assert.NotNull(response);
        }

        [Theory]
        [InlineData(1)]
        public async Task CheckGetRelatedItemsReturnListOfItemDto(int itemId)
        {
            _itemRelationRepo.Setup(repo => repo.GetRelatedItems(itemId)).ReturnsAsync(GetTestRelations());

            ItemRelationBl itemRelationBl = new ItemRelationBl(_mockItemRepo.Object, _mockMapper,
                _itemRelationRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object,
                _mockSprintRepository.Object);

            var response = await itemRelationBl.GetRelatedItemsAsync(itemId);
            Assert.IsAssignableFrom<IEnumerable<ItemDto>>(response);
            Assert.NotNull(response);
        }

        [Theory]
        [InlineData(1)]
        public async Task CheckGetRelatedItemsIfNotExistThrowException(int itemId)
        {
            _itemRelationRepo.Setup(repo => repo.GetRelatedItems(itemId)).ReturnsAsync(null as List<ItemRelation>);

            ItemRelationBl itemRelationBl = new ItemRelationBl(_mockItemRepo.Object, _mockMapper,
                _itemRelationRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object,
                _mockSprintRepository.Object);

            await Assert.ThrowsAsync<NotFoundResponseException>(() => itemRelationBl.GetRelatedItemsAsync(itemId));
        }

        [Theory]
        [InlineData(1, 9, "dev")]
        public async Task CheckCreateRelationReturnOk(int firstItemId, int secondItemId, string userId)
        {
            _mockItemRepo.Setup(repo => repo.ReadAsync(firstItemId)).ReturnsAsync(ItemBlTests.GetTestItems()[0]);
            _mockItemRepo.Setup(repo => repo.ReadAsync(secondItemId)).ReturnsAsync(ItemBlTests.GetTestItems()[1]);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>()))
                .ReturnsAsync(AppUserRole.Developer);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ItemBlTests.GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestProjects()[0]);

            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(firstItemId, secondItemId))
                .ReturnsAsync(null as ItemRelation);
            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(secondItemId, firstItemId))
                .ReturnsAsync(null as ItemRelation);

            ItemRelationBl itemRelationBl = new ItemRelationBl(_mockItemRepo.Object, _mockMapper,
                _itemRelationRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object,
                _mockSprintRepository.Object);

            var response = await itemRelationBl.CreateRecordAsync(firstItemId, secondItemId, userId);

            Assert.True(response.Success);
        }

        [Theory]
        [InlineData(1, 2, "dev")]
        public async Task CheckCreateExistRelationThrowsException(int firstItemId, int secondItemId, string userId)
        {
            _mockItemRepo.Setup(repo => repo.ReadAsync(firstItemId)).ReturnsAsync(ItemBlTests.GetTestItems()[0]);
            _mockItemRepo.Setup(repo => repo.ReadAsync(secondItemId)).ReturnsAsync(ItemBlTests.GetTestItems()[1]);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>()))
                .ReturnsAsync(AppUserRole.Developer);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ItemBlTests.GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestProjects()[0]);

            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(firstItemId, secondItemId))
                .ReturnsAsync(GetTestRelations()[0]);
            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(secondItemId, firstItemId))
                .ReturnsAsync(GetTestRelations()[1]);

            ItemRelationBl itemRelationBl = new ItemRelationBl(_mockItemRepo.Object, _mockMapper,
                _itemRelationRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object,
                _mockSprintRepository.Object);

            await Assert.ThrowsAsync<ForbiddenResponseException>(() =>
                itemRelationBl.CreateRecordAsync(firstItemId, secondItemId, userId));
        }

        [Theory]
        [InlineData(1, 2, "dev")]
        public async Task CheckDeleteRelationReturnsOk(int firstItemId, int secondItemId, string userId)
        {
            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(firstItemId, secondItemId))
                .ReturnsAsync(GetTestRelations()[0]);
            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(secondItemId, firstItemId))
                .ReturnsAsync(GetTestRelations()[1]);

            ItemRelationBl itemRelationBl = new ItemRelationBl(_mockItemRepo.Object, _mockMapper,
                _itemRelationRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object,
                _mockSprintRepository.Object);

            var response = await itemRelationBl.DeleteRecordAsync(firstItemId, secondItemId, userId);

            Assert.True(response.Success);
        }

        [Theory]
        [InlineData(1, 5, "dev")]
        public async Task CheckDeleteNotExistRelationThrowsException(int firstItemId, int secondItemId, string userId)
        {
            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(firstItemId, secondItemId))
                .ReturnsAsync(null as ItemRelation);
            _itemRelationRepo.Setup(repo => repo.GetRecordAsync(secondItemId, firstItemId))
                .ReturnsAsync(null as ItemRelation);

            ItemRelationBl itemRelationBl = new ItemRelationBl(_mockItemRepo.Object, _mockMapper,
                _itemRelationRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object,
                _mockSprintRepository.Object);

            await Assert.ThrowsAsync<ForbiddenResponseException>(() =>
                itemRelationBl.DeleteRecordAsync(firstItemId, secondItemId, userId));
        }

        public static List<ItemRelation> GetTestRelations()
        {
            var items = new List<ItemRelation>
            {
                new ItemRelation {FirstItemId = 1, SecondItemId = 2},
                new ItemRelation {FirstItemId = 1, SecondItemId = 3},
                new ItemRelation {FirstItemId = 1, SecondItemId = 4},
                new ItemRelation {FirstItemId = 4, SecondItemId = 7},
                new ItemRelation {FirstItemId = 4, SecondItemId = 8},
                new ItemRelation {FirstItemId = 6, SecondItemId = 2},
                new ItemRelation {FirstItemId = 2, SecondItemId = 9}
            };

            return items;
        }
    }
}