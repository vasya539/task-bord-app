using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;
using WebApi.BLs;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class ItemBlTests
    {
        private readonly Mock<IItemRepository> _mockItemRepo;
        private readonly IMapper _mockMapper;
        private readonly Mock<ICommentRepository> _mockCommentRepo;
        private readonly Mock<IProjectUserRepository> _mockProjectUserRepo;
        private readonly Mock<IProjectRepository> _mockProjectRepo;
        private readonly Mock<ISprintRepository> _mockSprintRepository;

        public ItemBlTests()
        {
            var cfg = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
            _mockItemRepo = new Mock<IItemRepository>();
            _mockMapper = cfg.CreateMapper();
            _mockCommentRepo = new Mock<ICommentRepository>();
            _mockProjectUserRepo = new Mock<IProjectUserRepository>();
            _mockProjectRepo = new Mock<IProjectRepository>();
            _mockSprintRepository = new Mock<ISprintRepository>();
        }

        #region Get

        [Fact]
        public async Task CheckGetAllAsyncReturnListOfItemDto()
        {
            _mockItemRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetTestItems());
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetAllAsync();
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Count);
            _mockItemRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CheckGetAllAsyncReturnEmptyListIfEmpty()
        {
            _mockItemRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(null as List<Item>);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetAllAsync();
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Empty(listItems);
            _mockItemRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        //Return empty list write test!

        [Theory]
        [InlineData(1)]
        public async Task CheckGetBySprintIdAsyncReturnListOfItemDto(int sprintId)
        {
            _mockItemRepo.Setup(repo => repo.GetBySprintIdAsync(sprintId)).ReturnsAsync(GetTestItems());
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetBySprintIdAsync(sprintId);
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Count);
            _mockItemRepo.Verify(r => r.GetBySprintIdAsync(sprintId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task CheckGetBySprintIdAsyncReturnEmptyListIfEmpty(int sprintId)
        {
            _mockItemRepo.Setup(repo => repo.GetBySprintIdAsync(sprintId)).ReturnsAsync(null as List<Item>);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetBySprintIdAsync(sprintId);
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Empty(listItems);
            _mockItemRepo.Verify(r => r.GetBySprintIdAsync(sprintId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task CheckGetArchivatedBySprintIdReturnListOfItemDto(int sprintId)
        {
            _mockItemRepo.Setup(repo => repo.GetArchivedBySprintIdAsync(sprintId)).ReturnsAsync(GetTestItems().Where(r => r.IsArchived == true));
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetArchivedBySprintIdAsync(sprintId);
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Where(r => r.IsArchived == true).ToList().Count);
            _mockItemRepo.Verify(r => r.GetArchivedBySprintIdAsync(sprintId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task CheckGetAllChildsForItemReturnList(int itemId)
        {
            _mockItemRepo.Setup(repo => repo.GetAllChildAsync(itemId)).ReturnsAsync(GetTestItems().Where(r => r.ParentId == itemId));
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetAllChildAsync(itemId);
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Where(r => r.ParentId == itemId).ToList().Count);
            _mockItemRepo.Verify(r => r.GetAllChildAsync(itemId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task CheckGetUserStoriesReturnOnlyStories(int itemTypeId)
        {
            _mockItemRepo.Setup(repo => repo.GetUserStoriesAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems().Where(r => r.TypeId == itemTypeId));
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetUserStoriesAsync(It.IsAny<int>());
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Where(r => r.TypeId == itemTypeId).ToList().Count);
            _mockItemRepo.Verify(r => r.GetUserStoriesAsync(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task CheckGetUnparentedReturnOnlyWirthoutParent(int itemTypeId)
        {
            _mockItemRepo.Setup(repo => repo.GetUnparentedAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems().Where(r => r.ParentId == null));
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetUnparentedAsync(It.IsAny<int>());
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Where(r => r.ParentId == null).ToList().Count);
            _mockItemRepo.Verify(r => r.GetUnparentedAsync(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task CheckGetChildWIthSpecificStatusReturnRightStatus(int itemStatus)
        {
            _mockItemRepo.Setup(repo => repo.GetChildWithSpecificStatusAsync(It.IsAny<int>(), itemStatus)).ReturnsAsync(GetTestItems().Where(r => r.StatusId == itemStatus));
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetChildWithSpecificStatusAsync(It.IsAny<int>(), itemStatus);
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Where(r => r.StatusId == itemStatus).ToList().Count);
            _mockItemRepo.Verify(r => r.GetChildWithSpecificStatusAsync(It.IsAny<int>(), itemStatus), Times.Once);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        public async Task CheckGetChildWIthSpecificStatusReturnRightSprintAndStatus(int sprintId, int itemStatus)
        {
            _mockItemRepo.Setup(repo => repo.GetChildWithSpecificStatusAsync(sprintId, itemStatus)).ReturnsAsync(GetTestItems().Where(r => r.StatusId == itemStatus && r.SprintId == sprintId));
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var items = await ItemBl.GetChildWithSpecificStatusAsync(sprintId, itemStatus);
            var viewResult = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(items);
            var listItems = items as List<ItemDto>;
            Assert.Equal(listItems.Count, GetTestItems().Where(r => r.StatusId == itemStatus && r.SprintId == sprintId).ToList().Count);
            _mockItemRepo.Verify(r => r.GetChildWithSpecificStatusAsync(sprintId, itemStatus), Times.Once);
        }

        #endregion Get

        #region Read

        [Fact]
        public async Task CheckReadAsyncCorrectReturnItemDto()
        {
            //Arrange
            _mockItemRepo.Setup(repo => repo.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            //Act
            var item = await ItemBl.ReadAsync(It.IsAny<int>());
            //Assert
            var viewResult = Assert.IsAssignableFrom<ItemDto>(item);
            _mockItemRepo.Verify(r => r.ReadAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CheckReadNotExistanceAsyncReturnNull()
        {
            //Arrange
            _mockItemRepo.Setup(repo => repo.ReadAsync(It.IsAny<int>())).ReturnsAsync(null as Item);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            //Act
            var item = await ItemBl.ReadAsync(It.IsAny<int>());
            //Assert
            Assert.Null(item);
            _mockItemRepo.Verify(r => r.ReadAsync(It.IsAny<int>()), Times.Once);
        }

        #endregion Read

        #region Create

        [Theory]
        [InlineData("owner", 1)]
        [InlineData("master", 2)]
        [InlineData("master", 3)]
        public async Task CheckScrumMasterOrOwnerCanCreateAnything(string userId, int itemType)
        {
            //Arrange
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "2138b181-4cee-4b85-9f16-18df308f387d", Name = "Item Name1", Description = "Description Item1", StatusId = 2, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 };
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            //Act
            var item = await ItemBl.CreateAsync(item1, userId);
            //Assert
            var viewResult = Assert.IsAssignableFrom<ItemResponse>(item);
            _mockItemRepo.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(viewResult.Success);
        }

        [Theory]
        [InlineData("developer")]
        [InlineData("observer")]
        [InlineData("none")]
        public async Task CheckNotScrumMasterOrOwnerCanNotCreateUserStory(string userId)
        {
            //Arrange
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "2138b181-4cee-4b85-9f16-18df308f387d", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 };
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("developer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("none", It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            //Act-Assert
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.CreateAsync(item1, userId));
            _mockItemRepo.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("dev1", 2)]
        [InlineData("dev1", 3)]
        [InlineData("dev1", 4)]
        public async Task CheckDevCanCreateItemAssignedToHim(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev1", Name = "Item Name1", Description = "Description Item1", StatusId = 2, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev1", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            //Act-Assert
            var response = await ItemBl.CreateAsync(item1, userId);
            _mockItemRepo.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("dev1", 2)]
        public async Task CheckDevCantCreateItemNotAssignedToHim(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev2", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev1", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            //Act-Assert
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.CreateAsync(item1, userId));
            _mockItemRepo.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("dev1", 2)]
        public async Task CheckItemWithStatusNotNewCantBeNotAssigned(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = null, Name = "Item Name1", Description = "Description Item1", StatusId = 2, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev1", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            //Act-Assert
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.CreateAsync(item1, userId));
            _mockItemRepo.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Never);
        }

        #endregion Create

        #region Update

        [Theory]
        [InlineData("obs1", 2)]
        [InlineData("none", 1)]
        public async Task CheckNotTeamMemberCantTryToUpdate(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev2", Name = "Item Name NEW1", Description = "Description Item1", StatusId = 1, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("obs1", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("none", It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(item1, userId));

            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("owner", 1)]
        [InlineData("master", 1)]
        public async Task CheckOwnersCanChangeItemAssigning(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev2 NEW", Name = "Item Name NEW1", Description = "Description Item1", StatusId = 1, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await ItemBl.UpdateAsync(item1, userId);
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("dev", 1)]
        public async Task CheckDevCantChangeItemAssigningIfAssigned(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev2 NEW", Name = "Item Name NEW1", Description = "Description Item1", StatusId = 1, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(item1, userId));
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("dev", 2)]
        public async Task CheckDevCanAssignItemIfAssignedNull(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev", Name = "Item Name NEW1", Description = "Description Item1", StatusId = 3, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[7]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await ItemBl.UpdateAsync(item1, userId);
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("dev", 1)]
        public async Task CheckDevCantAssignItemToAnotherUser(string userId, int itemTypeId)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev2312123123", Name = "Item Name NEW1", Description = "Description Item1", StatusId = 1, TypeId = itemTypeId, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[5]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(item1, userId));
        }

        [Theory]
        [InlineData("master", 4)]
        [InlineData("owner", 1)]
        public async Task CheckOwnersCanChangeItemsStatuses(string userId, int itemStatus)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev2312123123", Name = "Item Name NEW1", Description = "Description Item1", StatusId = itemStatus, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[5]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await ItemBl.UpdateAsync(item1, userId);
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("dev", 1)]
        public async Task CheckDevCantChangeStatusToNewIfAssigned(string userId, int itemStatus)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev", Name = "Item Name NEW1", Description = "Description Item1", StatusId = itemStatus, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[6]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(item1, userId));
        }

        [Theory]
        [InlineData("dev", 4)]
        public async Task CheckDevCanAssignAndChangeStatus(string userId, int itemStatus)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev", Name = "Item Name NEW1", Description = "Description Item1", StatusId = itemStatus, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[6]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await ItemBl.UpdateAsync(item1, userId);
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("dev", 4)]
        public async Task CheckDevCantChangeStatusIfAnotherUserAssigned(string userId, int itemStatus)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev3", Name = "Item Name NEW1", Description = "Description Item1", StatusId = itemStatus, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[1]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(item1, userId));
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("dev", 4)]
        [InlineData("owner", 4)]
        [InlineData("master", 4)]
        public async Task CheckItemTypeCantBeChanged(string userId, int itemType)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = "dev3", Name = "Item Name NEW1", Description = "Description Item1", StatusId = 3, TypeId = itemType, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[3]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(item1, userId));
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("owner", 1)]
        [InlineData("master", 1)]
        public async Task CheckItemMustHaveUserToBeActive(string userId, int itemType)
        {
            ItemDto item1 = new ItemDto { Id = 1, SprintId = 1, AssignedUserId = null, Name = "Item Name NEW1", Description = "Description Item1", StatusId = 2, TypeId = itemType, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[3]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(item1, userId));
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("owner", 1)]
        [InlineData("master", 1)]
        public async Task CheckIfAssignedUserSetNullThenMoveItemToNew(string userId, int itemType)
        {
            ItemDto item1 = new ItemDto { Id = 7, SprintId = 2, AssignedUserId = null, Name = "Item Name7", Description = "Description Item5", StatusId = 3, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 };

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);

            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[6]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);
            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await ItemBl.UpdateAsync(item1, userId);
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(response.Success);
        }

        #endregion Update

        [Theory]
        [InlineData("master", 4)]
        [InlineData("owner", 1)]
        public async Task Check_StoryCanHaveParentStoryFromAnotherSprint(string userId, int itemStatus)
        {
            Item first = GetTestItems()[0];
            var firstItemDto = _mockMapper.Map<ItemDto>(first);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            _mockItemRepo.Setup(r => r.ReadAsync(first.Id)).ReturnsAsync(GetTestItems()[0]);
            _mockItemRepo.Setup(r => r.ReadAsync(GetTestItems()[8].Id)).ReturnsAsync(GetTestItems()[8]);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(GetTestSprints()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(GetTestSprints()[1]);

            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            firstItemDto.ParentId = GetTestItems()[8].Id;

            var response = await ItemBl.UpdateAsync(firstItemDto, userId);
            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Once);
            Assert.True(response.Success);
        }

        [Theory]
        [InlineData("master", 4)]
        [InlineData("owner", 1)]
        public async Task Check_NotStoryCantHaveParentFromAnotherSprint(string userId, int itemStatus)
        {
            Item first = GetTestItems()[1];
            var firstItemDto = _mockMapper.Map<ItemDto>(first);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            _mockItemRepo.Setup(r => r.ReadAsync(first.Id)).ReturnsAsync(GetTestItems()[1]);
            _mockItemRepo.Setup(r => r.ReadAsync(GetTestItems()[8].Id)).ReturnsAsync(GetTestItems()[8]);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(GetTestSprints()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(GetTestSprints()[1]);

            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            firstItemDto.ParentId = GetTestItems()[8].Id;

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(firstItemDto, userId));

            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("master", 4)]
        [InlineData("owner", 1)]
        public async Task Check_ItemCantBeParentAndChildAtOneTime(string userId, int itemStatus)
        {
            Item first = GetTestItems()[0];
            var firstItemDto = _mockMapper.Map<ItemDto>(first);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            _mockItemRepo.Setup(r => r.ReadAsync(first.Id)).ReturnsAsync(GetTestItems()[0]);
            _mockItemRepo.Setup(r => r.ReadAsync(GetTestItems()[7].Id)).ReturnsAsync(GetTestItems()[7]);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(GetTestSprints()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(GetTestSprints()[1]);

            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            firstItemDto.ParentId = GetTestItems()[7].Id;

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(firstItemDto, userId));

            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("master", 4)]
        [InlineData("owner", 1)]
        public async Task Check_ItemTaskCantHaveParentAnotherTask(string userId, int itemStatus)
        {
            Item first = GetTestItems()[5];
            var firstItemDto = _mockMapper.Map<ItemDto>(first);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            _mockItemRepo.Setup(r => r.ReadAsync(first.Id)).ReturnsAsync(GetTestItems()[5]);
            _mockItemRepo.Setup(r => r.ReadAsync(GetTestItems()[6].Id)).ReturnsAsync(GetTestItems()[6]);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(GetTestSprints()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(GetTestSprints()[1]);

            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            firstItemDto.ParentId = GetTestItems()[6].Id;

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(firstItemDto, userId));

            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        [Theory]
        [InlineData("dev1", 4)]
        public async Task Check_DevCantChangeParentForNotHimselfItem(string userId, int itemStatus)
        {
            Item first = GetTestItems()[6];
            var firstItemDto = _mockMapper.Map<ItemDto>(first);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev1", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);

            _mockItemRepo.Setup(r => r.ReadAsync(first.Id)).ReturnsAsync(GetTestItems()[5]);
            _mockItemRepo.Setup(r => r.ReadAsync(GetTestItems()[6].Id)).ReturnsAsync(GetTestItems()[6]);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(GetTestSprints()[0]);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(GetTestSprints()[1]);

            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            firstItemDto.ParentId = GetTestItems()[7].Id;

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.UpdateAsync(firstItemDto, userId));

            _mockItemRepo.Verify(r => r.UpdateAsync(It.IsAny<Item>()), Times.Never);
        }

        #region Delete

        [Theory]
        [InlineData("master")]
        [InlineData("owner")]
        public async Task CheckOwnerOrMasterCanDeleteItem(string userId)
        {
            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);

            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            var response = await ItemBl.DeleteAsync(It.IsAny<int>(), userId);
            Assert.True(response.Success);
            Assert.IsType<ItemResponse>(response);
            _mockItemRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData("dev")]
        public async Task CheckDevCantDeleteItem(string userId)
        {
            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);

            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.DeleteAsync(It.IsAny<int>(), userId));
            _mockItemRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData("dev")]
        public async Task CheckDeleteNotExistItemReturnException(string userId)
        {
            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(null as Item);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);

            await Assert.ThrowsAsync<NotFoundResponseException>(() => ItemBl.DeleteAsync(It.IsAny<int>(), userId));
            _mockItemRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion Delete

        #region Archivating

        [Theory]
        [InlineData("owner", 1)]
        [InlineData("master", 1)]
        public async Task CheckOwnersCanArchivateItems(string userId, int itemId)
        {
            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);
            _mockItemRepo.Setup(r => r.GetAllChildAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems());
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await ItemBl.ArchivingAsync(itemId, userId);
            Assert.True(response.Success);
            Assert.IsType<ItemResponse>(response);
        }

        [Theory]
        [InlineData("dev", 1)]
        [InlineData("none", 1)]
        [InlineData("observer", 1)]
        public async Task CheckNotOwnersCantArchivateItems(string userId, int itemId)
        {
            _mockItemRepo.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems()[0]);
            _mockItemRepo.Setup(r => r.GetAllChildAsync(It.IsAny<int>())).ReturnsAsync(GetTestItems());
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(GetTestProjects()[0]);

            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("observer", It.IsAny<int>())).ReturnsAsync(AppUserRole.Observer);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("none", It.IsAny<int>())).ReturnsAsync(AppUserRole.None);

            var ItemBl = new ItemBl(_mockItemRepo.Object, _mockMapper, _mockCommentRepo.Object, _mockProjectUserRepo.Object, _mockProjectRepo.Object, _mockSprintRepository.Object);
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => ItemBl.ArchivingAsync(itemId, userId));
        }

        #endregion Archivating

        public static List<Item> GetTestItems()
        {
            var items = new List<Item>
            {
                new Item { Id = 1, SprintId = 1, AssignedUserId = "dev2", Name = "Item Name1", Description = "Parent Test 1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 },
                new Item { Id = 2, SprintId = 1, AssignedUserId = "dev3", Name = "Item Name2", Description = "Description Item2", StatusId = 2, TypeId = 2, IsArchived = false, ParentId = 1, StoryPoint = 2 },
                new Item { Id = 3, SprintId = 2, AssignedUserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992", Name = "Item Name3", Description = "Description Item3", StatusId = 3, TypeId = 1, IsArchived = true, ParentId = 1, StoryPoint = 2 },
                new Item { Id = 4, SprintId = 2, AssignedUserId = "54bfd1f9-d379-4930-9c3b-4c84992c028e", Name = "Item Name4", Description = "Description Item4", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = 2, StoryPoint = 2 },
                new Item { Id = 5, SprintId = 2, AssignedUserId = "54bfd1f9-d379-4930-9c3b-4c84992c028e", Name = "Item Name5", Description = "Description Item5", StatusId = 3, TypeId = 1, IsArchived = true, ParentId = null, StoryPoint = 2 },
                new Item { Id = 6, SprintId = 2, AssignedUserId = null, Name = "Item Name6", Description = "Description Item5", StatusId = 2, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 },
                new Item { Id = 7, SprintId = 2, AssignedUserId = "dev", Name = "Item Name7", Description = "Description Item5", StatusId = 3, TypeId = 2, IsArchived = false, ParentId = null, StoryPoint = 2 },
                new Item { Id = 8, SprintId = 2, AssignedUserId = null, Name = "Item Name7", Description = "Description Item5", StatusId = 1, TypeId = 2, IsArchived = false, ParentId = 1, StoryPoint = 2 },
                new Item { Id = 9, SprintId = 2, AssignedUserId = null, Name = "ParentTest", Description = "Parent test 2", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 }
            };
            return items;
        }

        public static List<Project> GetTestProjects()
        {
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "First Project", Description = "Some description to Project1" },
                new Project { Id = 2, Name = "Second Project", Description = "Some description to Project2" }
            };
            return projects;
        }

        public static List<Sprint> GetTestSprints()
        {
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, ProjectId = 1, StartDate = DateTime.Today, Name = "Sprint 1", Description = "Sprint descr 1", EndDate = DateTime.Today.AddDays(7) },
                new Sprint { Id = 2, ProjectId = 1, StartDate = DateTime.Today, Name = "Sprint 1", Description = "Sprint descr 1", EndDate = DateTime.Today.AddDays(7) }
            };
            return sprints;
        }
    }
}