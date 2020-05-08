using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using WebApi.BLs;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Exceptions;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class CommentBlTests
    {
        private readonly IMapper _mockMapper;
        private readonly Mock<ICommentRepository> _mockCommentRepo;
        private readonly Mock<IProjectUserRepository> _mockProjectUserRepo;
        private readonly Mock<IProjectRepository> _mockProjectRepo;
        private readonly Mock<ISprintRepository> _mockSprintRepository;
        public CommentDto tempCommentDto;

        public CommentBlTests()
        {
            var cfg = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
            _mockMapper = cfg.CreateMapper();
            _mockCommentRepo = new Mock<ICommentRepository>();
            _mockProjectUserRepo = new Mock<IProjectUserRepository>();
            _mockProjectRepo = new Mock<IProjectRepository>();
            _mockSprintRepository = new Mock<ISprintRepository>();
            tempCommentDto = new CommentDto
            {
                Id = 5,
                Date = DateTime.Now,
                ItemId = 10,
                Text = "Comment TEMP",
                UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992"
            };
        }

        [Fact]
        public async Task CheckGetAllAsyncReturnListOfCommentDto()
        {
            _mockCommentRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(GetTestComments());
            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            var comments = await commentBl.GetAllAsync();
            Assert.IsAssignableFrom<IEnumerable<CommentDto>>(comments);
            var listComments = comments as List<CommentDto>;
            Assert.Equal(listComments.Count, GetTestComments().Count);
            _mockCommentRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [InlineData(10)]
        [Theory]
        public async Task CheckGetCommentByItemIdAsyncReturnListOfCommentDto(int itemId)
        {
            _mockCommentRepo.Setup(repo => repo.GetByItemIdAsync(itemId)).ReturnsAsync(GetTestComments());
            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            var comments = await commentBl.GetByItemIdAsync(itemId);
            Assert.IsAssignableFrom<IEnumerable<CommentDto>>(comments);
            var listComments = comments as List<CommentDto>;
            Assert.Equal(listComments.Count, GetTestComments().Count);
            _mockCommentRepo.Verify(r => r.GetByItemIdAsync(itemId), Times.Once);
        }

        [InlineData(1)]
        [Theory]
        public async Task CheckReadAsyncReturnCommentDto(int id)
        {
            _mockCommentRepo.Setup(repo => repo.ReadAsync(id)).ReturnsAsync(GetTestComments()[0]);
            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            var comment = await commentBl.ReadAsync(id);
            Assert.IsAssignableFrom<CommentDto>(comment);
            _mockCommentRepo.Verify(r => r.ReadAsync(id), Times.Once);
        }

        [Fact]
        public async Task CheckCreateCommentReturnPositiveItemResponse()
        {
            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await commentBl.CreateAsync(tempCommentDto);
            Assert.IsAssignableFrom<ItemResponse>(response);
            Assert.True(response.Success);
            _mockCommentRepo.Verify(repository => repository.CreateAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task CheckUpdateCommentReturnPositiveIfOk()
        {
            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await commentBl.UpdateAsync(tempCommentDto);
            Assert.IsAssignableFrom<ItemResponse>(response);
            Assert.True(response.Success);
            _mockCommentRepo.Verify(repository => repository.UpdateAsync(It.IsAny<Comment>()), Times.Once);
        }

        [InlineData("master")]
        [InlineData("owner")]
        [Theory]
        public async Task CheckScrumMasterOrOwnerCanDeleteAnyComment(string userId)
        {
            _mockCommentRepo.Setup(repository => repository.ReadAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestComments()[0]);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("owner", It.IsAny<int>())).ReturnsAsync(AppUserRole.Owner);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("master", It.IsAny<int>())).ReturnsAsync(AppUserRole.ScrumMaster);

            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestProjects()[0]);

            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await commentBl.DeleteAsync(It.IsAny<int>(), userId);
            Assert.IsAssignableFrom<ItemResponse>(response);
            Assert.True(response.Success);
            _mockCommentRepo.Verify(repository => repository.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [InlineData("dev1")]
        [Theory]
        public async Task CheckDevCantDeleteNotHimselfComment(string userId)
        {
            _mockCommentRepo.Setup(repository => repository.ReadAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestComments()[0]);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev1", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestProjects()[0]);

            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            await Assert.ThrowsAsync<ForbiddenResponseException>(() => commentBl.DeleteAsync(It.IsAny<int>(), userId));
            _mockCommentRepo.Verify(repository => repository.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [InlineData("dev")]
        [Theory]
        public async Task CheckDevCanDeleteHimselfComment(string userId)
        {
            _mockCommentRepo.Setup(repository => repository.ReadAsync(It.IsAny<int>()))
                .ReturnsAsync(GetTestComments()[0]);
            _mockProjectUserRepo.Setup(r => r.GetRoleOfMember("dev", It.IsAny<int>())).ReturnsAsync(AppUserRole.Developer);
            _mockSprintRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestSprints()[0]);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ItemBlTests.GetTestProjects()[0]);

            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            var response = await commentBl.DeleteAsync(It.IsAny<int>(), userId);
            Assert.IsAssignableFrom<ItemResponse>(response);
            Assert.True(response.Success);
            _mockCommentRepo.Verify(repository => repository.DeleteAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CheckDeleteNotExistCommentThrowException()
        {
            _mockCommentRepo.Setup(repository => repository.ReadAsync(It.IsAny<int>()))
                    .ReturnsAsync(null as Comment);
            var commentBl = new CommentBl(_mockCommentRepo.Object, _mockMapper, _mockProjectUserRepo.Object,
                _mockProjectRepo.Object, _mockSprintRepository.Object);
            await Assert.ThrowsAsync<NotFoundResponseException>(() => commentBl.DeleteAsync(It.IsAny<int>(), It.IsAny<string>()));
            _mockCommentRepo.Verify(repository => repository.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        public List<Comment> GetTestComments()
        {
            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = 1, Date = DateTime.Now, ItemId = 10, Text = "Comment one",
                    UserId = "dev", Item = new Item { Id = 10, SprintId = 1, AssignedUserId = "dev2", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 }
                },
                new Comment
                {
                    Id = 2, Date = DateTime.Now, ItemId = 10, Text = "Comment two",
                    UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992",Item= new Item { Id = 10, SprintId = 1, AssignedUserId = "dev2", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 }
                },
                new Comment
                {
                    Id = 3, Date = DateTime.Now, ItemId = 11, Text = "Comment three",
                    UserId = "421cb65f-a76d-4a73-8a1a-d792f37ef992", Item = new Item { Id = 10, SprintId = 1, AssignedUserId = "dev2", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1, IsArchived = false, ParentId = null, StoryPoint = 2 }
                }
            };
            return comments;
        }
    }
}