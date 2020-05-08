using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.BLs;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;
using Xunit;

namespace BusinessLogicLayer.Tests
{
    public class SprintBLTests
    {
        private readonly Mock<ISprintRepository> mockSprintRepo;
        private readonly IMapper mockMapper;
        private readonly Mock<IItemRepository> mockItemRepo;
        private readonly Mock<IProjectUserRepository> mockProjectUserRepo;

        public SprintBLTests()
        {
            var mapperConfig = new MapperConfiguration(cf => cf.AddProfile(new WebApi.Data.Profiles.AutoMapperProfiler()));
            mockMapper = mapperConfig.CreateMapper();
            mockSprintRepo = new Mock<ISprintRepository>();
            mockItemRepo = new Mock<IItemRepository>();
            mockProjectUserRepo = new Mock<IProjectUserRepository>();
        }

        [Fact]
        public void GetAllByProjectIdAsync_Should_Return_All_Sprints()
        {
            // Arrange
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(GetSampleSprints());
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.GetAllByProjectIdAsync(1, It.IsAny<string>());
            // Assert
            Assert.IsType<Task<IEnumerable<SprintDto>>>(result);
            Assert.Equal(GetSampleSprints().Count, result.Result.ToList().Count);
        }

        [Fact]
        public void GetByIdAsync_Should_Return_Correct_Sprint()
        {
            // Arrange
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(GetSampleSprints().FirstOrDefault());
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.GetByIdAsync(1, It.IsAny<string>());
            // Assert
            Assert.IsType<Task<SprintDto>>(result);
            Assert.Equal(GetSampleSprints().FirstOrDefault().Id, result.Result.Id);
        }

        [Fact]
        public void CreateAsync_Should_Return_Correct_Response_When_Created()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 0,
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 4, 2),
                EndDate = new DateTime(2020, 5, 1)
            };
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(GetSampleSprints());
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.CreateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.True(result.Result.Success);
        }

        [Fact]
        public void CreateAsync_Should_Return_Error_Response_When_Start_Date_Is_Later_Then_End_Date()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 0,
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 5, 2),
                EndDate = new DateTime(2020, 4, 1)
            };
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(GetSampleSprints());
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.CreateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal("Sprint end date must be later than start date", result.Result.Message);
        }

        [Fact]
        public void CreateAsync_Should_Return_Error_Response_When_Sprints_Overlaps()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 0,
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 4, 1),
                EndDate = new DateTime(2020, 5, 1)
            };
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(GetSampleSprints());
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.CreateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal($"Sprint overlaps. Choose start date after {GetSampleSprints()[2].EndDate.Value.ToShortDateString()}", result.Result.Message);
        }

        [Fact]
        public void UpdateAsync_Should_Return_CorrectResponse()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 2,
                Name = "Sprint 2",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 2, 2),
                EndDate = new DateTime(2020, 2, 28)
            };
            var sprints = GetSampleSprints();
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(sprints[1]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(sprints);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.UpdateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.True(result.Result.Success);
        }

        [Fact]
        public void UpdateAsync_Should_Return_Error_Response_When_Start_Date_Is_Later_Than_End_Date()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 2,
                Name = "Sprint 2",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 2, 15),
                EndDate = new DateTime(2020, 2, 14)
            };
            var sprints = GetSampleSprints();
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(sprints[1]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(sprints);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.UpdateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal("Sprint end date must be later than start date", result.Result.Message);
        }

        [Fact]
        public void UpdateAsync_Should_Return_Error_Response_When_Start_Date_Is_Less_Than_End_Date_Of_The_Previous_Sprint()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 2,
                Name = "Sprint 2",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 1, 30),
                EndDate = new DateTime(2020, 3, 1)
            };
            var sprints = GetSampleSprints();
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(sprints[1]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(sprints);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.UpdateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal($"Error. Choose dates after {sprints[0].EndDate.Value.ToShortDateString()} and before {sprints[2].StartDate.Value.ToShortDateString()}", result.Result.Message);
        }

        [Fact]
        public void UpdateAsync_Should_Return_Error_Response_When_End_Date_Is_Greater_Than_Start_Date_Of_The_Next_Sprint()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 2,
                Name = "Sprint 2",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 2, 2),
                EndDate = new DateTime(2020, 3, 2)
            };
            var sprints = GetSampleSprints();
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(sprints[1]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(sprints);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.UpdateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal($"Error. Choose dates after {sprints[0].EndDate.Value.ToShortDateString()} and before {sprints[2].StartDate.Value.ToShortDateString()}", result.Result.Message);
        }

        [Fact]
        public void UpdateAsync_First_Sprint_Should_Return_Error_Response()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 1,
                Name = "Sprint 1",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2020, 2, 2)
            };
            var sprints = GetSampleSprints();
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(sprints[0]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(sprints);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.UpdateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal($"Error. Choose end date before {sprints[1].StartDate.Value.ToShortDateString()}", result.Result.Message);
        }

        [Fact]
        public void UpdateAsync_Last_Sprint_Should_Return_Error_Response()
        {
            // Arrange
            SprintDto newSprintDto = new SprintDto
            {
                Id = 3,
                Name = "Sprint 3",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 3, 1),
                EndDate = new DateTime(2020, 4, 1)
            };
            var sprints = GetSampleSprints();
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(sprints[2]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(sprints);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.UpdateAsync(newSprintDto, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
            Assert.Equal($"Error. Choose start date after {sprints[1].EndDate.Value.ToShortDateString()}", result.Result.Message);
        }

        [Fact]
        public void Delete_Should_Returns_Correct_Response_When_Deleted()
        {
            // Arrange
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(GetSampleSprints()[2]);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(GetSampleSprints());
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.DeleteAsync(3, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.True(result.Result.Success);
        }

        [Fact]
        public void Delete_Should_Returns_Correct_Response_When_Not_Found()
        {
            // Arrange
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(null as Sprint);
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetAllByProjectIdAsync(1)).ReturnsAsync(GetSampleSprints());
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.DeleteAsync(3, It.IsAny<string>());
            // Assert
            Assert.IsType<SprintResponse>(result.Result);
            Assert.False(result.Result.Success);
        }

        [Fact]
        public void GetAllSprintItemsAsync_Should_Return_All_Sprints()
        {
            // Arrange
            mockItemRepo.Setup(repo => repo.GetUnparentedAsync(1)).ReturnsAsync(new Item[] {
            new Item { Id = 1, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1 },
            new Item { Id = 2, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name2", Description = "Description Item2", StatusId = 2, TypeId = 2 },
            new Item { Id = 3, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name3", Description = "Description Item3", StatusId = 3, TypeId = 1 },
            new Item { Id = 4, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name4", Description = "Description Item4", StatusId = 1, TypeId = 2 }
        });
            mockItemRepo.Setup(repo => repo.GetUserStoriesAsync(1)).ReturnsAsync(new Item[] {
            new Item { Id = 5, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name1", Description = "Description Item1", StatusId = 1, TypeId = 1 },
            new Item { Id = 6, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name2", Description = "Description Item2", StatusId = 2, TypeId = 2 },
            new Item { Id = 7, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name3", Description = "Description Item3", StatusId = 3, TypeId = 1 },
            new Item { Id = 8, ParentId = null, SprintId = 1, AssignedUserId = "d7f1b614-bf60-4340-9daa-c8dce98fd400", Name = "Item Name4", Description = "Description Item4", StatusId = 1, TypeId = 2 }
        });
            mockProjectUserRepo.Setup(repo => repo.GetRoleOfMember(It.IsAny<string>(), 1)).ReturnsAsync(AppUserRole.Owner);
            mockSprintRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(GetSampleSprints()[0]);
            var sprintBL = new SprintBl(mockSprintRepo.Object, mockMapper, mockProjectUserRepo.Object, mockItemRepo.Object);
            // Act
            var result = sprintBL.GetAllSprintItemsAsync(1, It.IsAny<string>());
            // Assert
            Assert.IsType<Task<IEnumerable<ItemListDto>>>(result);
            Assert.Equal(8, result.Result.ToList().Count);
        }

        public List<Sprint> GetSampleSprints()
        {
            List<Sprint> sprints = new List<Sprint>();
            Sprint itemType1 = new Sprint { 
                Id = 1,
                Name = "Sprint 1",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2020, 2, 1) 
            };
            Sprint itemType2 = new Sprint { 
                Id = 2,
                Name = "Sprint 2",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 2, 2),
                EndDate = new DateTime(2020, 3, 1)
            };
            Sprint itemType3 = new Sprint {
                Id = 3,
                Name = "Sprint 3",
                Description = "Some description",
                Items = null,
                ProjectId = 1,
                StartDate = new DateTime(2020, 3, 2),
                EndDate = new DateTime(2020, 4, 1)
            };
            sprints.AddRange(new[] { itemType1, itemType2, itemType3 });
            return sprints;
        }
    }
}
